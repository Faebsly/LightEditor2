// ISubGroupService.cs
using LightEditor2.Core.Models;

namespace LightEditor2.Core.Services
{
    public interface ISubGroupService
    {
        // Alle Untergruppen laden
        Task<List<SubGroup>> GetSubGroupsAsync();

        // Alle Untergruppen eines bestimmten Projekts laden
        Task<List<SubGroup>> GetSubGroupsByProjectIdAsync(int projectId);

        // Eine einzelne Untergruppe anhand der ID laden
        Task<SubGroup?> GetSubGroupByIdAsync(int subGroupId);

        // Eine neue Untergruppe hinzufügen
        Task<bool> AddSubGroupAsync(SubGroup subGroup);

        // Eine bestehende Untergruppe aktualisieren
        Task<bool> UpdateSubGroupAsync(SubGroup subGroup);

        // Eine Untergruppe anhand der ID löschen
        Task<bool> DeleteSubGroupAsync(int subGroupId);
    }
}
