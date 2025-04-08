// ProjectService.cs
using LightEditor2.Core.Data;
using LightEditor2.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics; // Beibehalten oder durch einen Logger ersetzen

namespace LightEditor2.Core.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory; // Änderung hier! Siehe Erklärung unten.
        private readonly ILogger<ProjectService> _logger; // Logger hinzufügen

        public ProjectService(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<ProjectService> logger) // Konstruktor anpassen
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        // ... GetProjectsAsync und GetProjectByIdAsync (kommen gleich) ...

        /// <summary>
        /// Neues Projekt hinzufügen
        /// </summary>
        /// <param name="project"></param>
        /// <returns>True bei Erfolg, False bei Fehler.</returns>
        public async Task<bool> AddProjectAsync(Project project)
        {
            // Korrekte Verwendung mit Factory für Scoped DbContext
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                await dbContext.Projects.AddAsync(project);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Projekt '{ProjectName}' erfolgreich hinzugefügt.", project.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen des Projekts '{ProjectName}'.", project.Name);
                return false;
            }
        }

        /// <summary>
        /// Ein bestehendes Projekt aktualisieren
        /// </summary>
        /// <param name="project"></param>
        /// <returns>True bei Erfolg, False bei Fehler.</returns>
        public async Task<bool> UpdateProjectAsync(Project project)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Sicherstellen, dass die Entität getrackt wird, falls sie detached ist
                var existingProject = await dbContext.Projects.FindAsync(project.Id);
                if (existingProject == null)
                {
                    _logger.LogWarning("Projekt mit ID {ProjectId} zum Aktualisieren nicht gefunden.", project.Id);
                    return false; // Oder Exception werfen? Hier: false
                }

                // Nur relevante Properties aktualisieren (optional, aber oft besser als .Update)
                dbContext.Entry(existingProject).CurrentValues.SetValues(project);

                // Oder einfacher:
                // dbContext.Projects.Update(project); // Markiert alle Properties als geändert

                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Projekt '{ProjectName}' (ID: {ProjectId}) erfolgreich aktualisiert.", project.Name, project.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException ex) // Spezieller Fehler für Parallelitätskonflikte
            {
                _logger.LogError(ex, "Parallelitätskonflikt beim Aktualisieren des Projekts '{ProjectName}' (ID: {ProjectId}).", project.Name, project.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Projekts '{ProjectName}' (ID: {ProjectId}).", project.Name, project.Id);
                return false;
            }
        }

        /// <summary>
        /// Ein Projekt anhand der ID löschen
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>True bei Erfolg, False bei Fehler oder wenn nicht gefunden.</returns>
        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var project = await dbContext.Projects.FindAsync(projectId);
                if (project != null)
                {
                    dbContext.Projects.Remove(project);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Projekt mit ID {ProjectId} erfolgreich gelöscht.", projectId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Projekt mit ID {ProjectId} zum Löschen nicht gefunden.", projectId);
                    return false; // Nicht gefunden ist kein Fehler, aber die Operation war nicht "erfolgreich" im Sinne von Löschen
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Projekts mit ID {ProjectId}.", projectId);
                return false;
            }
        }

        // --- Anpassung GetProjectByIdAsync ---

        /// <summary>
        /// Ein einzelnes Projekt anhand seiner ID laden (inkl. SubGroups)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>Das Projekt oder null, wenn nicht gefunden.</returns>
        public async Task<Project?> GetProjectByIdAsync(int projectId) // Rückgabetyp auf Project? geändert
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Explizit null zurückgeben, wenn nichts gefunden wird.
                Project? project = await dbContext.Projects
                    .Include(p => p.SubGroups) // Behalten, wenn SubGroups benötigt werden
                    .FirstOrDefaultAsync(p => p.Id == projectId);
                return project; // Kann null sein
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Projekts mit ID {ProjectId}.", projectId);
                return null; // Im Fehlerfall auch null zurückgeben
            }
        }

        // --- Anpassung GetProjectsAsync (mit Fehlerbehandlung & Logging) ---
        public async Task<List<Project>> GetProjectsAsync()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Überlegung zum Overfetching: Nur SubGroups laden, keine Slides?
                // Alternative: Ohne ThenInclude laden, wenn Slides nicht immer gebraucht werden.
                return await dbContext.Projects
                    .Include(p => p.SubGroups)
                    // .ThenInclude(s => s.Slides) // Entfernen oder nur bei Bedarf laden! (Siehe Punkt 1)
                    .OrderBy(p => p.Name) // Optional: Sortieren
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Projekte.");
                return new List<Project>(); // Leere Liste im Fehlerfall
            }
        }
    }
}