// LightEditor2.Maui/Services/MauiFileSaverService.cs
using LightEditor2.Core.Abstractions;       // Das Interface aus Core
using CommunityToolkit.Maui.Storage;       // Für IFileSaver aus Toolkit
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LightEditor2.Maui.Services // Namespace des MAUI-Projekts
{
    public class MauiFileSaverService : IFileSaverService
    {
        // Injizieren Sie IFileSaver direkt. Das Toolkit registriert die Standardimplementierung.
        private readonly IFileSaver _fileSaver;
        public MauiFileSaverService(IFileSaver fileSaver) // Konstruktor-Injection
        {
            _fileSaver = fileSaver ?? throw new ArgumentNullException(nameof(fileSaver));
        }

        public async Task<AppFileSaverResult> SaveAsync(string initialPath, Stream stream, CancellationToken cancellationToken = default)
        {
            try
            {
                // Verwende die injizierte Instanz
                var result = await _fileSaver.SaveAsync(initialPath, stream, cancellationToken);
                // Wandle das Toolkit-Ergebnis in unser Abstraktionsergebnis um
                return new AppFileSaverResult
                {
                    IsSuccessful = result.IsSuccessful,
                    FilePath = result.FilePath,
                    Exception = result.Exception
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiFileSaverService] SaveAsync Fehler: {ex}");
                return new AppFileSaverResult { IsSuccessful = false, Exception = ex };
            }
        }

        public async Task<AppFileSaverResult> SaveAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            // Standardnamen verwenden, wenn kein initialPath gegeben ist
            string defaultName = $"Datei_{DateTime.Now:yyyyMMdd_HHmmss}.bin"; // Beispiel Dateiname
            try
            {
                var result = await _fileSaver.SaveAsync(defaultName, stream, cancellationToken);
                return new AppFileSaverResult
                {
                    IsSuccessful = result.IsSuccessful,
                    FilePath = result.FilePath,
                    Exception = result.Exception
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiFileSaverService] SaveAsync (ohne Pfad) Fehler: {ex}");
                return new AppFileSaverResult { IsSuccessful = false, Exception = ex };
            }
        }
    }
}