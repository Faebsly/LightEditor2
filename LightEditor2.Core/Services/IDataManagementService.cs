// Components/Services/IDataManagementService.cs
using LightEditor2.Core.Models;

namespace LightEditor2.Core.Services
{
    public interface IDataManagementService
    {
        /// <summary>
        /// Ruft alle Daten (Projekte, Untergruppen, Slides) für den Export ab.
        /// </summary>
        /// <returns>Eine Liste aller Projekte mit ihren Abhängigkeiten oder null bei Fehlern.</returns>
        Task<FullExportData?> GetAllDataForExportAsync();

        /// <summary>
        /// Importiert Daten aus einem JSON-String, ersetzt dabei alle vorhandenen Daten.
        /// </summary>
        /// <param name="jsonData">Der JSON-String, der die Projektdaten enthält.</param>
        /// <returns>True bei Erfolg, False bei Fehlern.</returns>
        Task<bool> ImportAllDataAsync(string jsonData);

        
    }
}