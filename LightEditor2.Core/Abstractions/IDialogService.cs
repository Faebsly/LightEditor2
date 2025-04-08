// LightEditor2.Core/Abstractions/IDialogService.cs
using System.Threading.Tasks;

namespace LightEditor2.Core.Abstractions
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel);
        Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);

        // --- NEUE METHODE HINZUFÜGEN ---
        /// <summary>
        /// Zeigt eine Eingabeaufforderung an.
        /// </summary>
        /// <param name="title">Der Titel des Dialogs.</param>
        /// <param name="message">Die anzuzeigende Nachricht.</param>
        /// <param name="accept">Text für den OK-Button (Standard: "OK").</param>
        /// <param name="cancel">Text für den Abbrechen-Button (Standard: "Abbrechen").</param>
        /// <param name="placeholder">Platzhaltertext im Eingabefeld (Standard: null).</param>
        /// <param name="initialValue">Vorausgefüllter Wert im Eingabefeld (Standard: null).</param>
        /// <param name="maxLength">Maximale Länge der Eingabe (Standard: -1, unbegrenzt).</param>
        /// <param name="keyboardType">Der zu verwendende Tastaturtyp (Standard: "Default").</param>
        /// <returns>Den eingegebenen Text oder null, wenn der Benutzer abbricht.</returns>
        Task<string?> ShowPromptAsync(
            string title,
            string message,
            string accept = "OK",
            string cancel = "Abbrechen",
            string? placeholder = null,
            string? initialValue = null,
            int maxLength = -1,
            string keyboardType = "Default"); // Wir verwenden string für Keyboard, da Keyboard-Enum MAUI-spezifisch ist

        /// <summary>
        /// Zeigt eine Liste von Optionen an, aus denen der Benutzer wählen kann.
        /// </summary>
        /// <param name="title">Der Titel des Dialogs.</param>
        /// <param name="cancel">Text für den Abbruch-Button.</param>
        /// <param name="destruction">Text für einen optionalen "destruktiven" Button (wird oft hervorgehoben).</param>
        /// <param name="buttons">Die Liste der normalen Auswahlmöglichkeiten.</param>
        /// <returns>Den Text des ausgewählten Buttons oder null, wenn abgebrochen wurde.</returns>
        Task<string?> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons);
    }
}