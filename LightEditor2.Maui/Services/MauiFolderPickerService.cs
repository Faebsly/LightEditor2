// LightEditor2.Maui/Services/MauiFolderPickerService.cs
using LightEditor2.Core.Abstractions;       // Das Interface aus Core
using CommunityToolkit.Maui.Storage;       // Für IFolderPicker aus Toolkit
using System.Threading;
using System.Threading.Tasks;


namespace LightEditor2.Maui.Services // Namespace des MAUI-Projekts
{
    public class MauiFolderPickerService : IFolderPickerService
    {
        private readonly IFolderPicker _folderPicker;
        public MauiFolderPickerService(IFolderPicker folderPicker) // Konstruktor-Injection
        {
            _folderPicker = folderPicker ?? throw new ArgumentNullException(nameof(folderPicker));
        }

        public async Task<FolderPickerResult> PickFolderAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Verwende die injizierte Instanz
                var result = await _folderPicker.PickAsync(cancellationToken);
                // Wandle das Toolkit-Ergebnis in unser Abstraktionsergebnis um
                return new FolderPickerResult
                {
                    IsSuccessful = result.IsSuccessful,
                    Path = result.IsSuccessful ? result.Folder.Path : null,
                    Exception = result.Exception
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiFolderPickerService] PickFolderAsync Fehler: {ex}");
                return new FolderPickerResult { IsSuccessful = false, Exception = ex };
            }
        }
    }
}