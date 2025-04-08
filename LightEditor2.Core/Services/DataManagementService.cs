// Components/Services/DataManagementService.cs
using LightEditor2.Core.Data;
using LightEditor2.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json; // Für JSON Serialisierung
using System.Text.Json.Serialization; // Für ReferenceHandler

namespace LightEditor2.Core.Services
{
    public class DataManagementService : IDataManagementService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ILogger<DataManagementService> _logger;

        public DataManagementService(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<DataManagementService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        // Rückgabetyp angepasst auf FullExportData?
        public async Task<FullExportData?> GetAllDataForExportAsync()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // 1. Projekte laden (wie bisher, mit allen Includes)
                var projects = await dbContext.Projects
                    .Include(p => p.SubGroups)
                        .ThenInclude(s => s.Slides)
                    .OrderBy(p => p.Name)
                    .AsNoTracking()
                    .ToListAsync();

                // 2. Settings laden
                var settings = await dbContext.Settings
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Alle {ProjectCount} Projekte und {SettingsCount} Einstellungen für den Export geladen.", projects.Count, settings.Count);

                // 3. DTO erstellen und zurückgeben
                var exportData = new FullExportData
                {
                    Projects = projects,
                    Settings = settings
                };
                return exportData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden aller Daten (Projekte & Settings) für den Export.");
                return null;
            }
        }

        public async Task<bool> ImportAllDataAsync(string jsonData)
        {
            List<Project> importedProjects;
            try
            {
                // JSON Deserialisieren
                var options = new JsonSerializerOptions
                {
                    // Wichtig, falls Sie komplexe Beziehungen hätten, die Zyklen verursachen könnten
                    // Für das aktuelle Modell wahrscheinlich nicht nötig, schadet aber nicht.
                    ReferenceHandler = ReferenceHandler.Preserve,
                    PropertyNameCaseInsensitive = true // Flexibler beim Einlesen
                };
                importedProjects = JsonSerializer.Deserialize<List<Project>>(jsonData, options) ?? new List<Project>();
                _logger.LogInformation("JSON-Daten erfolgreich deserialisiert. {ProjectCount} Projekte gefunden.", importedProjects.Count);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Fehler beim Deserialisieren der JSON-Daten für den Import.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Allgemeiner Fehler beim Vorbereiten der Importdaten.");
                return false;
            }


            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            // --- WICHTIG: Transaktion verwenden, um sicherzustellen, dass alles oder nichts passiert ---
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                _logger.LogWarning("Beginne Import: LÖSCHE ALLE VORHANDENEN DATEN!");

                // 1. Alle vorhandenen Daten löschen (Reihenfolge wichtig wegen Constraints, obwohl Cascade helfen sollte)
                // Alternative zu ExecuteDeleteAsync (EF Core 7+): RemoveRange + SaveChanges
                // dbContext.Slides.RemoveRange(dbContext.Slides);
                // dbContext.SubGroups.RemoveRange(dbContext.SubGroups);
                // dbContext.Projects.RemoveRange(dbContext.Projects);
                // await dbContext.SaveChangesAsync(); // Einmal speichern nach dem Löschen

                // Oder mit ExecuteDeleteAsync (einfacher für komplettes Löschen)
                int deletedSlides = await dbContext.Slides.ExecuteDeleteAsync();
                int deletedSubGroups = await dbContext.SubGroups.ExecuteDeleteAsync();
                int deletedProjects = await dbContext.Projects.ExecuteDeleteAsync();
                _logger.LogInformation("Daten gelöscht: {Projects} Projekte, {SubGroups} Untergruppen, {Slides} Slides.", deletedProjects, deletedSubGroups, deletedSlides);


                // 2. Neue Daten hinzufügen
                // Wichtig: Sicherstellen, dass die IDs nicht explizit gesetzt werden,
                // wenn sie von der DB generiert werden sollen (AutoIncrement).
                // EF Core sollte das bei AddRange normalerweise korrekt handhaben,
                // indem es Entitäten als 'Added' markiert.
                // Falls importierte Daten IDs haben, könnte es zu Konflikten kommen,
                // wenn die DB diese IDs neu generieren will. Sicherer ist oft,
                // die IDs vor dem AddRange zu entfernen/nullen, wenn sie nicht erhalten bleiben sollen.
                // HIER GEHEN WIR DAVON AUS, DASS DIE IDs ÜBERSCHRIEBEN/IGNORIERT WERDEN.
                if (importedProjects.Any())
                {
                    await dbContext.Projects.AddRangeAsync(importedProjects);
                    await dbContext.SaveChangesAsync(); // Speichern der neuen Daten
                    _logger.LogInformation("{ProjectCount} Projekte erfolgreich importiert.", importedProjects.Count);
                }
                else
                {
                    _logger.LogInformation("Keine Projekte in den Importdaten gefunden.");
                }


                // 3. Transaktion bestätigen
                await transaction.CommitAsync();
                _logger.LogInformation("Datenimport erfolgreich abgeschlossen und Transaktion committed.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FEHLER während des Datenbank-Importvorgangs. Transaktion wird zurückgerollt.");
                // Bei Fehlern Transaktion zurückrollen
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}