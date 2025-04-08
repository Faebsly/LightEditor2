// CurrentStateService.cs

namespace LightEditor2.Core.Services
{
    public class CurrentStateService
    {
        // Speichert den aktuellen Projektnamen
        public string CurrentProjectName { get; set; } = string.Empty;

        // Speichert den aktuellen Untergruppennamen
        public string CurrentSubgroupName { get; set; } = string.Empty;

        // Speichert die Id ders aktuellen Untergruppenname
        public int CurrentSubgroupId { get; set; } = 0;

        // Berechnet den Headertext
        public string HeaderText => $"{CurrentProjectName} -> {CurrentSubgroupName}";

        // Event, das aufgerufen wird, wenn sich der globale Zustand ändert
        public event Action? OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
