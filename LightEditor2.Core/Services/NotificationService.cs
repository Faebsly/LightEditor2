using System.Timers;

namespace LightEditor2.Core.Services
{
    public class NotificationService : IDisposable
    {
        // Das Event, das die Komponente zum Aktualisieren auffordert
        public event Action? OnShow;
        // Das Event zum Ausblenden (optional, aber gut für sauberes Handling)
        public event Action? OnHide;

        // Die anzuzeigende Nachricht
        public string? CurrentMessage { get; private set; }
        // Flag, ob die Nachricht sichtbar sein soll
        public bool IsVisible { get; private set; }

        private System.Timers.Timer? _timer;

        /// <summary>
        /// Zeigt eine Benachrichtigung für eine bestimmte Dauer an.
        /// </summary>
        /// <param name="message">Die anzuzeigende Nachricht.</param>
        /// <param name="durationMilliseconds">Dauer in Millisekunden (Standard: 4000 = 4 Sekunden).</param>
        public void ShowMessage(string message, int durationMilliseconds = 4000)
        {
            CurrentMessage = message;
            IsVisible = true;

            // Benachrichtige Listener (die Komponente), dass sie sich anzeigen soll
            OnShow?.Invoke();

            // Alten Timer stoppen und verwerfen, falls einer läuft
            _timer?.Stop();
            _timer?.Dispose();

            // Neuen Timer starten, um die Nachricht nach der Dauer auszublenden
            _timer = new System.Timers.Timer(durationMilliseconds);
            _timer.Elapsed += HideMessageTimerCallback;
            _timer.AutoReset = false; // Nur einmal auslösen
            _timer.Start();
        }

        private void HideMessageTimerCallback(object? sender, ElapsedEventArgs e)
        {
            IsVisible = false;
            CurrentMessage = null; // Nachricht zurücksetzen
            // Benachrichtige Listener, dass sie sich ausblenden sollen
            OnHide?.Invoke();
            // Wichtig: Da der Timer-Callback in einem anderen Thread laufen kann,
            // stellen wir sicher, dass StateHasChanged im UI-Thread aufgerufen wird,
            // indem wir OnHide/OnShow verwenden, die von der Blazor-Komponente behandelt werden.
        }

        // Aufräumen, wenn der Service nicht mehr benötigt wird (bei Singleton eher am App-Ende)
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
