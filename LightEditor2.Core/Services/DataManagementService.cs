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
            FullExportData? importedData; // Korrekter Typ für das deserialisierte Objekt
            try
            {
                // JSON Deserialisieren in das FullExportData DTO
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve, // Konsistent mit Export halten
                    PropertyNameCaseInsensitive = true
                };
                // --- KORREKTER TYP HIER ---
                importedData = JsonSerializer.Deserialize<FullExportData>(jsonData, options);

                // Prüfen, ob Deserialisierung erfolgreich war und Daten enthält
                if (importedData == null)
                {
                    _logger.LogError("Fehler beim Deserialisieren der JSON-Daten: Ergebnis ist null.");
                    return false;
                }
                // --- Log anpassen, um alle Daten zu erwähnen ---
                _logger.LogInformation("JSON-Daten erfolgreich deserialisiert. FormatVersion: {FormatVersion}, Projekte: {ProjectCount}, Settings: {SettingsCount}",
                    importedData.FormatVersion, importedData.Projects?.Count ?? 0, importedData.Settings?.Count ?? 0);

                // Optional: FormatVersion prüfen
                if (importedData.FormatVersion != 1)
                {
                    _logger.LogWarning("Importierte Daten haben eine unerwartete FormatVersion: {FormatVersion}", importedData.FormatVersion);
                    // Entscheiden, ob Import abgebrochen werden soll
                    // return false;
                }
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

            // Sicherstellen, dass Listen nicht null sind (falls JSON leer war)
            var projectsToImport = importedData.Projects ?? new List<Project>();
            var settingsToImport = importedData.Settings ?? new List<Setting>();

            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                _logger.LogWarning("Beginne Import: LÖSCHE ALLE VORHANDENEN PROJEKTE, UNTERGRUPPEN, SLIDES UND EINSTELLUNGEN!");

                // 1. Alle vorhandenen Daten löschen (inkl. Settings)
                int deletedSettings = await dbContext.Settings.ExecuteDeleteAsync(); // <-- Settings löschen
                int deletedSlides = await dbContext.Slides.ExecuteDeleteAsync();
                int deletedSubGroups = await dbContext.SubGroups.ExecuteDeleteAsync();
                int deletedProjects = await dbContext.Projects.ExecuteDeleteAsync();
                _logger.LogInformation("Daten gelöscht: {Projects} Projekte, {SubGroups} Untergruppen, {Slides} Slides, {Settings} Einstellungen.",
                    deletedProjects, deletedSubGroups, deletedSlides, deletedSettings);


                // 2. Neue Daten hinzufügen (Projekte und Settings)
                if (projectsToImport.Any())
                {
                    // --- Projekte aus importedData verwenden ---
                    await dbContext.Projects.AddRangeAsync(projectsToImport);
                    await dbContext.SaveChangesAsync(); // Projekte speichern
                    _logger.LogInformation("{ProjectCount} Projekte erfolgreich importiert.", projectsToImport.Count);
                }
                else
                {
                    _logger.LogInformation("Keine Projekte in den Importdaten gefunden.");
                }

                // --- SETTINGS IMPORTIEREN ---
                if (settingsToImport.Any())
                {
                    // --- Settings aus importedData verwenden ---
                    // Da Settings einen festen Key haben, ist AddRange hier sicher,
                    // nachdem wir vorher alles gelöscht haben.
                    await dbContext.Settings.AddRangeAsync(settingsToImport);
                    await dbContext.SaveChangesAsync(); // Settings speichern
                    _logger.LogInformation("{SettingsCount} Einstellungen erfolgreich importiert.", settingsToImport.Count);
                }
                else
                {
                    _logger.LogInformation("Keine Einstellungen in den Importdaten gefunden.");
                }
                // ---------------------------

                // 3. Transaktion bestätigen
                await transaction.CommitAsync();
                _logger.LogInformation("Datenimport erfolgreich abgeschlossen und Transaktion committed.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FEHLER während des Datenbank-Importvorgangs. Transaktion wird zurückgerollt.");
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}