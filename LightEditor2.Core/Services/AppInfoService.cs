// Components/Services/AppInfoService.cs
using System.Reflection; // Wichtig für Reflection

namespace LightEditor2.Core.Services
{
    public class AppInfoService
    {
        public string DisplayVersion { get; }

        public AppInfoService()
        {
            // Versuche, die 'ApplicationDisplayVersion' aus der Assembly zu lesen.
            // AssemblyInformationalVersionAttribute wird normalerweise dafür verwendet.
            try
            {
                var assembly = typeof(AppInfoService).Assembly; // Verwende die Assembly des Services selbst
                var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                string fullVersion = versionAttribute?.InformationalVersion ?? "v0.1a";

                // Den String am '+' aufteilen und den ersten Teil nehmen
                string[] versionParts = fullVersion.Split('+');
                DisplayVersion = versionParts[0].Trim(); // Trim() entfernt Leerzeichen

                if (string.IsNullOrWhiteSpace(DisplayVersion))
                {
                    DisplayVersion = "v0.1a"; // Fallback, falls der erste Teil leer ist
                }
            }
            catch (Exception ex)
            {
                // Fehler beim Auslesen loggen (optional)
                Console.WriteLine($"Fehler beim Auslesen der Assembly-Version: {ex.Message}"); // Einfaches Logging
                DisplayVersion = "Fehler";
            }
        }
    }
}