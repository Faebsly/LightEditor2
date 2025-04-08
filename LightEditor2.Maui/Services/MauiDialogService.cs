// LightEditor2.Maui/Services/MauiDialogService.cs
using LightEditor2.Core.Abstractions; // Das Interface aus Core
using Microsoft.Maui.Controls;       // Für Page und MainThread

namespace LightEditor2.Maui.Services // Namespace des MAUI-Projekts
{
    public class MauiDialogService : IDialogService
    {
        // Helper-Funktion, um die aktuelle Seite zu bekommen (vereinfacht)
        private Page? GetCurrentPage()
        {
            if (Application.Current?.MainPage != null)
            {
                // Versucht, die tiefste navigierte Seite zu finden
                if (Application.Current.MainPage is NavigationPage navPage && navPage.CurrentPage != null)
                {
                    return navPage.CurrentPage;
                }
                // Füge hier ggf. Prüfung für Shell hinzu, falls du Shell verwendest
                // if(Application.Current.MainPage is Shell shell && shell.CurrentPage != null) {
                //     return shell.CurrentPage;
                // }
                return Application.Current.MainPage;
            }
            Console.WriteLine("[MauiDialogService] GetCurrentPage: Application.Current oder MainPage ist null.");
            return null;
        }

        public async Task ShowAlertAsync(string title, string message, string cancel)
        {
            Page? currentPage = GetCurrentPage();
            if (currentPage != null)
            {
                // Sicherstellen, dass es auf dem UI-Thread läuft
                if (MainThread.IsMainThread)
                {
                    await currentPage.DisplayAlert(title, message, cancel);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await currentPage.DisplayAlert(title, message, cancel);
                    });
                }
            }
            else
            {
                Console.WriteLine($"[MauiDialogService] ShowAlertAsync: CurrentPage ist null. Title: {title}");
            }
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            Page? currentPage = GetCurrentPage();
            if (currentPage != null)
            {
                bool result = false;
                // Sicherstellen, dass es auf dem UI-Thread läuft
                if (MainThread.IsMainThread)
                {
                    result = await currentPage.DisplayAlert(title, message, accept, cancel);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        result = await currentPage.DisplayAlert(title, message, accept, cancel);
                    });
                }
                return result;
            }
            else
            {
                Console.WriteLine($"[MauiDialogService] ShowConfirmationAsync: CurrentPage ist null. Title: {title}");
                return false; // Standard-Antwort bei Fehler
            }
        }

        // --- NEUE METHODE IMPLEMENTIEREN ---
        public async Task<string?> ShowPromptAsync(
            string title,
            string message,
            string accept = "OK",
            string cancel = "Abbrechen",
            string? placeholder = null,
            string? initialValue = null,
            int maxLength = -1,
            string keyboardType = "Default")
        {
            Page? currentPage = GetCurrentPage();
            if (currentPage != null)
            {
                string? result = null;
                await MainThread.InvokeOnMainThreadAsync(async () => {
                    // Konvertiere den Keyboard-String in das MAUI Keyboard Enum
                    Keyboard keyboard = keyboardType.ToLowerInvariant() switch
                    {
                        "default" => Keyboard.Default,
                        "text" => Keyboard.Text,
                        "numeric" => Keyboard.Numeric,
                        "telephone" => Keyboard.Telephone,
                        "url" => Keyboard.Url,
                        "email" => Keyboard.Email,
                        "chat" => Keyboard.Chat,
                        "plain" => Keyboard.Plain,
                        _ => Keyboard.Default // Fallback
                    };

                    result = await currentPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
                });
                return result;
            }
            else
            {
                Console.WriteLine($"DialogService Error: CurrentPage is null. Prompt: {title} - {message}");
                return null; // Fehler oder Abbruch signalisieren
            }
        }

        // --- IMPLEMENTIERUNG DER NEUEN METHODE ---
        public async Task<string?> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons)
        {
            Page? currentPage = GetCurrentPage();
            if (currentPage != null)
            {
                string? result = null;
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // Ruft die MAUI DisplayActionSheet API auf
                    result = await currentPage.DisplayActionSheet(title, cancel, destruction, buttons);
                });
                // Das Ergebnis von DisplayActionSheet ist der Text des geklickten Buttons
                // oder der 'cancel'-Text, wenn abgebrochen wird.
                // Wir geben null zurück, wenn der 'cancel'-Button geklickt wurde,
                // um es konsistenter mit anderen "Abbruch"-Aktionen zu machen.
                if (result == cancel)
                {
                    return null;
                }
                return result; // Gibt den Text des geklickten Buttons (oder destruction) zurück
            }
            else
            {
                Console.WriteLine($"[MauiDialogService] ShowActionSheetAsync: CurrentPage ist null. Title: {title}");
                return null; // Fehler oder Abbruch signalisieren
            }
        }
    }
}