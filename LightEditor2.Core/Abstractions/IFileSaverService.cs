// LightEditor2.Core/Abstractions/IFileSaverService.cs
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LightEditor2.Core.Abstractions
{
    // Vereinfachtes Ergebnisobjekt
    public class FileSaverResult
    {
        public bool IsSuccessful { get; set; }
        public string? FilePath { get; set; } // Tatsächlicher Speicherpfad (kann null sein)
        public Exception? Exception { get; set; }
    }

    public interface IFileSaverService
    {
        Task<FileSaverResult> SaveAsync(string initialPath, Stream stream, CancellationToken cancellationToken = default);
        // Optional: Überladung ohne initialPath, falls nicht immer sinnvoll
        Task<FileSaverResult> SaveAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}