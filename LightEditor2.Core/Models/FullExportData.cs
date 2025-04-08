// LightEditor2.Core/Models/FullExportData.cs
using System.Collections.Generic;

namespace LightEditor2.Core.Models // Sicherstellen, dass der Namespace korrekt ist
{
    /// <summary>
    /// Repräsentiert die vollständigen Daten für den Export und Import.
    /// </summary>
    public class FullExportData
    {
        /// <summary>
        /// Version des Exportformats, um zukünftige Änderungen handhaben zu können.
        /// </summary>
        public int FormatVersion { get; set; } = 1;

        /// <summary>
        /// Liste aller Projekte mit ihren Untergruppen und Slides.
        /// </summary>
        public List<Project> Projects { get; set; } = new List<Project>();

        /// <summary>
        /// Liste aller gespeicherten Einstellungen.
        /// </summary>
        public List<Setting> Settings { get; set; } = new List<Setting>();
    }
}