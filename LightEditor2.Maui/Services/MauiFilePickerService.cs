// LightEditor2.Maui/Services/MauiFilePickerService.cs
using LightEditor2.Core.Abstractions;
using Microsoft.Maui.Storage; // Original MAUI FilePicker

namespace LightEditor2.Maui.Services
{
    public class MauiFilePickerService : IFilePickerService
    {
        public async Task<FilePickerResult> PickFileAsync(FilePickerOptions? abstractOptions = null)
        {
            try
            {
                var mauiOptions = new PickOptions();
                if (abstractOptions != null)
                {
                    mauiOptions.PickerTitle = abstractOptions.PickerTitle;
                    if (abstractOptions.AllowedExtensions?.Any() ?? false)
                    {
                        // MAUI FilePickerFileType erstellen
                        var allowedTypes = new Dictionary<DevicePlatform, IEnumerable<string>>();
                        var extensions = abstractOptions.AllowedExtensions.Select(e => e.StartsWith(".") ? e : "." + e).ToList(); // Sicherstellen, dass Punkt davor ist

                        // Wir fügen einfach für alle bekannten Plattformen die gleichen Endungen hinzu
                        // Dies ist eine Vereinfachung gegenüber dem komplexen MAUI-Typ
                        allowedTypes.Add(DevicePlatform.WinUI, extensions);
                        allowedTypes.Add(DevicePlatform.macOS, extensions.Select(e => e.TrimStart('.'))); // macOS will Endungen ohne Punkt
                        allowedTypes.Add(DevicePlatform.iOS, new[] { "public.data" }); // iOS ist komplizierter, ggf. spezifischere UTIs
                        allowedTypes.Add(DevicePlatform.Android, extensions.Select(e => GetMimeType(e)).Where(m => m != null)!); // MIME-Typen

                        mauiOptions.FileTypes = new FilePickerFileType(allowedTypes);
                    }
                }

                var result = await FilePicker.PickAsync(mauiOptions);

                return new FilePickerResult
                {
                    FullPath = result?.FullPath,
                    FileName = result?.FileName,
                    Exception = null // PickAsync wirft eher Exceptions
                };
            }
            catch (Exception ex)
            {
                return new FilePickerResult { Exception = ex };
            }
        }

        // Simple Hilfsfunktion für MIME-Typen (nicht vollständig!)
        private string? GetMimeType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".json": return "application/json";
                case ".txt": return "text/plain";
                // Weitere Typen hier hinzufügen
                default: return null;
            }
        }
    }
}