// LightEditor2.Core/Abstractions/IFolderPickerService.cs
using System.Threading;
using System.Threading.Tasks;

namespace LightEditor2.Core.Abstractions
{
    // Vereinfachtes Ergebnisobjekt
    public class AppFolderPickerResult
    {
        public bool IsSuccessful { get; set; }
        public string? Path { get; set; }
        public Exception? Exception { get; set; }
    }

    public interface IFolderPickerService
    {
        Task<AppFolderPickerResult> PickFolderAsync(CancellationToken cancellationToken = default);
    }
}