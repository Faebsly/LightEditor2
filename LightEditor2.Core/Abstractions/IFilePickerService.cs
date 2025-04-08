// LightEditor2.Core/Abstractions/IFilePickerService.cs
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LightEditor2.Core.Abstractions
{
    // Vereinfachtes Ergebnisobjekt
    public class FilePickerResult
    {
        public bool IsSuccessful => !string.IsNullOrEmpty(FullPath);
        public string? FullPath { get; set; }
        public string? FileName { get; set; }
        public Exception? Exception { get; set; } // Optional für Fehler
    }

    // Vereinfachtes Optionenobjekt
    public class FilePickerOptions
    {
        public string? PickerTitle { get; set; }
        // Wir verwenden eine einfache Liste von erlaubten Endungen statt dem komplexen Typ
        public IEnumerable<string>? AllowedExtensions { get; set; }
    }

    public interface IFilePickerService
    {
        Task<FilePickerResult> PickFileAsync(FilePickerOptions? options = null);
    }
}