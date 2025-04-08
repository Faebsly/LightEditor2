// SlideService.cs
using LightEditor2.Core.Models;
using LightEditor2.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Hinzufügen
using System.Diagnostics; // Kann entfernt werden, wenn Logger verwendet wird

namespace LightEditor2.Core.Services
{
    public class SlideService : ISlideService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory; // Änderung hier
        private readonly ILogger<SlideService> _logger; // Logger hinzufügen

        public SlideService(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<SlideService> logger) // Konstruktor anpassen
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        // Alle Slides laden
        public async Task<List<Slide>> GetSlidesAsync()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return await dbContext.Slides.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden aller Slides.");
                return new List<Slide>(); // Leere Liste im Fehlerfall
            }
        }

        // Slides eines bestimmten SubGroups laden
        public async Task<List<Slide>> GetSlidesBySubGroupIdAsync(int subGroupId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                return await dbContext.Slides
                    .Where(s => s.SubGroupId == subGroupId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden der Slides für SubGroupId {SubGroupId}.", subGroupId);
                return new List<Slide>(); // Leere Liste im Fehlerfall
            }
        }

        // Einen einzelnen Slide anhand seiner ID abrufen
        public async Task<Slide?> GetSlideByIdAsync(int slideId) // Rückgabetyp Slide?
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // FirstOrDefaultAsync gibt bereits null zurück, wenn nichts gefunden wird.
                return await dbContext.Slides
                    .FirstOrDefaultAsync(s => s.Id == slideId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Laden des Slides mit ID {SlideId}.", slideId);
                return null; // Null im Fehlerfall
            }
        }

        // Einen neuen Slide hinzufügen
        public async Task<bool> AddSlideAsync(Slide slide) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                await dbContext.Slides.AddAsync(slide);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Slide '{SlideTitle}' (SubGroupId: {SubGroupId}) erfolgreich hinzugefügt.", slide.Title, slide.SubGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen des Slides '{SlideTitle}' (SubGroupId: {SubGroupId}).", slide.Title, slide.SubGroupId);
                return false;
            }
        }

        // Einen bestehenden Slide aktualisieren
        public async Task<bool> UpdateSlideAsync(Slide slide) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                // Optional: Prüfen, ob Slide existiert, bevor Update versucht wird
                var existingSlide = await dbContext.Slides.FindAsync(slide.Id);
                if (existingSlide == null)
                {
                    _logger.LogWarning("Slide mit ID {SlideId} zum Aktualisieren nicht gefunden.", slide.Id);
                    return false;
                }
                // Übernimmt Werte vom übergebenen 'slide' in den getrackten 'existingSlide'
                dbContext.Entry(existingSlide).CurrentValues.SetValues(slide);
                // Oder einfacher: dbContext.Slides.Update(slide);

                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Slide '{SlideTitle}' (ID: {SlideId}) erfolgreich aktualisiert.", slide.Title, slide.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException ex) // Spezieller Fehler für Parallelitätskonflikte
            {
                _logger.LogError(ex, "Parallelitätskonflikt beim Aktualisieren des Slides '{SlideTitle}' (ID: {SlideId}).", slide.Title, slide.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Slides '{SlideTitle}' (ID: {SlideId}).", slide.Title, slide.Id);
                return false;
            }
        }

        // Einen Slide anhand der ID löschen
        public async Task<bool> DeleteSlideAsync(int slideId) // Rückgabetyp bool
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                var slide = await dbContext.Slides.FindAsync(slideId);
                if (slide != null)
                {
                    dbContext.Slides.Remove(slide);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Slide mit ID {SlideId} erfolgreich gelöscht.", slideId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Slide mit ID {SlideId} zum Löschen nicht gefunden.", slideId);
                    return false; // Nicht gefunden, aber kein Fehler
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Slides mit ID {SlideId}.", slideId);
                return false;
            }
        }
    }
}