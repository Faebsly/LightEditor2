// ISlideService.cs
using LightEditor2.Core.Models;

namespace LightEditor2.Core.Services
{
    public interface ISlideService
    {
        // Alle Slides laden
        Task<List<Slide>> GetSlidesAsync();

        // Slides eines bestimmten SubGroups laden
        Task<List<Slide>> GetSlidesBySubGroupIdAsync(int subGroupId);

        // Einen einzelnen Slide anhand seiner ID abrufen
        Task<Slide?> GetSlideByIdAsync(int slideId);

        // Einen neuen Slide hinzufügen
        Task<bool> AddSlideAsync(Slide slide);

        // Einen bestehenden Slide aktualisieren
        Task<bool> UpdateSlideAsync(Slide slide);

        // Einen Slide anhand der ID löschen
        Task<bool> DeleteSlideAsync(int slideId);
    }
}
