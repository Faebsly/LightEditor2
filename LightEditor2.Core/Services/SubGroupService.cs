// SubGroupService.cs
using LightEditor2.Core.Data;
using LightEditor2.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Hinzufügen

namespace LightEditor2.Core.Services
{
    public class SubGroupService : ISubGroupService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory; 
        private readonly ILogger<SubGroupService> _logger; 

        public SubGroupService(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<SubGroupService> logger) 
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<List<SubGroup>> GetSubGroupsAsync()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Optional: Include Project? Normalerweise nicht nötig für eine reine Liste.
                return await dbContext.SubGroups.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden aller SubGroups.");
                return new List<SubGroup>();
            }
        }

        public async Task<List<SubGroup>> GetSubGroupsByProjectIdAsync(int projectId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Optional: Include Slides? Eher nicht für eine Liste.
                return await dbContext.SubGroups
                               .Where(s => s.ProjectId == projectId)
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der SubGroups für ProjectId {ProjectId}.", projectId);
                return new List<SubGroup>();
            }
        }

        public async Task<SubGroup?> GetSubGroupByIdAsync(int subGroupId) // Rückgabetyp SubGroup?
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Optional: Include Project oder Slides, falls benötigt
                SubGroup? subGroup = await dbContext.SubGroups
                                        // .Include(s => s.Project) // Beispiel
                                        // .Include(s => s.Slides) // Beispiel
                                        .FirstOrDefaultAsync(s => s.Id == subGroupId);
                return subGroup; // Kann null sein
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der SubGroup mit ID {SubGroupId}.", subGroupId);
                return null;
            }
        }

        public async Task<bool> AddSubGroupAsync(SubGroup subGroup) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                await dbContext.SubGroups.AddAsync(subGroup);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("SubGroup '{SubGroupName}' (ProjectId: {ProjectId}) erfolgreich hinzugefügt.", subGroup.Name, subGroup.ProjectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen der SubGroup '{SubGroupName}' (ProjectId: {ProjectId}).", subGroup.Name, subGroup.ProjectId);
                return false;
            }
        }

        public async Task<bool> UpdateSubGroupAsync(SubGroup subGroup) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var existingSubGroup = await dbContext.SubGroups.FindAsync(subGroup.Id);
                if (existingSubGroup == null)
                {
                    _logger.LogWarning("SubGroup mit ID {SubGroupId} zum Aktualisieren nicht gefunden.", subGroup.Id);
                    return false;
                }
                dbContext.Entry(existingSubGroup).CurrentValues.SetValues(subGroup);
                // Oder: dbContext.SubGroups.Update(subGroup);

                await dbContext.SaveChangesAsync();
                _logger.LogInformation("SubGroup '{SubGroupName}' (ID: {SubGroupId}) erfolgreich aktualisiert.", subGroup.Name, subGroup.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Parallelitätskonflikt beim Aktualisieren der SubGroup '{SubGroupName}' (ID: {SubGroupId}).", subGroup.Name, subGroup.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der SubGroup '{SubGroupName}' (ID: {SubGroupId}).", subGroup.Name, subGroup.Id);
                return false;
            }
        }

        public async Task<bool> DeleteSubGroupAsync(int subGroupId) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var subGroup = await dbContext.SubGroups.FindAsync(subGroupId);
                if (subGroup != null)
                {
                    dbContext.SubGroups.Remove(subGroup);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("SubGroup mit ID {SubGroupId} erfolgreich gelöscht.", subGroupId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("SubGroup mit ID {SubGroupId} zum Löschen nicht gefunden.", subGroupId);
                    return false;
                }
            }
            // Optional: Catch DbUpdateException für ForeignKey-Probleme, falls Cascade Delete nicht aktiv ist
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen der SubGroup mit ID {SubGroupId}.", subGroupId);
                return false;
            }
        }
    }
}