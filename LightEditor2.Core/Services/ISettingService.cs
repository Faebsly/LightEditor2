// LightEditor2.Core/Services/ISettingService.cs

using System.Threading.Tasks; // Für Task

// Sicherstellen, dass der Namespace korrekt ist
namespace LightEditor2.Core.Services
{
    /// <summary>
    /// Definiert einen Dienst zum Verwalten von Anwendungseinstellungen (Schlüssel-Wert-Paare).
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Ruft den Wert einer Einstellung asynchron ab.
        /// </summary>
        /// <param name="key">Der eindeutige Schlüssel der Einstellung (siehe auch SettingKeys-Klasse).</param>
        /// <returns>
        /// Eine Aufgabe, die das Ergebnis repräsentiert.
        /// Das Ergebnis ist der Wert der Einstellung als string,
        /// oder null, wenn die Einstellung nicht gefunden wurde oder ein Fehler aufgetreten ist.
        /// </returns>
        Task<string?> GetSettingAsync(string key);

        /// <summary>
        /// Setzt oder aktualisiert den Wert einer Einstellung asynchron.
        /// Wenn der Schlüssel bereits existiert, wird der Wert aktualisiert.
        /// Wenn der Schlüssel nicht existiert, wird ein neuer Eintrag erstellt.
        /// </summary>
        /// <param name="key">Der eindeutige Schlüssel der Einstellung (siehe auch SettingKeys-Klasse).</param>
        /// <param name="value">Der neue Wert für die Einstellung.</param>
        /// <returns>
        /// Eine Aufgabe, die das Ergebnis repräsentiert.
        /// Das Ergebnis ist true, wenn das Speichern/Aktualisieren erfolgreich war, andernfalls false.
        /// </returns>
        Task<bool> SetSettingAsync(string key, string value);
    }
}