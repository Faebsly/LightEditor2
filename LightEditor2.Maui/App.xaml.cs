// LightEditor2.Maui/App.xaml.cs
using LightEditor2.Core.Data; // Für AppDbContext
using Microsoft.EntityFrameworkCore; // Für Migrate
using Microsoft.Extensions.DependencyInjection; // Für IServiceProvider, CreateScope etc.

namespace LightEditor2.Maui
{
    public partial class App : Application
    {
        // KORREKTER KONSTRUKTOR mit IServiceProvider
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Datenbank beim Start migrieren (ruft den ServiceProvider auf)
            InitializeDatabase(serviceProvider);

            // MainPage wird erstellt, NACHDEM die Basis-Services konfiguriert sind
            MainPage = new MainPage();
        }

        // Methode zur Datenbankinitialisierung
        private void InitializeDatabase(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                // Wichtig: Da wir AddDbContextFactory verwenden, brauchen wir den Context selbst.
                // Die Factory ist für Services gut, hier brauchen wir eine Instanz.
                // Lösung 1: DbContext direkt anfordern (wenn AddDbContext auch registriert wäre - was es nicht ist)
                // Lösung 2: Factory verwenden, um Context zu erstellen
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var dbContext = dbContextFactory.CreateDbContext();

                // Migration durchführen
                dbContext.Database.Migrate();
                Console.WriteLine("Datenbankmigration beim Start erfolgreich durchgeführt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FEHLER bei der Datenbankmigration beim Start: {ex}");
                // Ggf. Fehler anzeigen? Vorsicht beim App-Start.
                // Task.Run(async () => await ShowErrorAlert(ex)); // Beispiel für versuchte Anzeige
            }
        }

        /* // Beispiel für Fehleranzeige (optional, mit Vorsicht verwenden)
        private async Task ShowErrorAlert(Exception ex) {
            // Warte kurz, bis die UI evtl. bereit ist
            await Task.Delay(1000);
            if(MainPage != null) {
                 await MainPage.Dispatcher.DispatchAsync(async () => {
                     await MainPage.DisplayAlert("DB Fehler", $"Fehler bei der Datenbankinitialisierung: {ex.Message}", "OK");
                 });
            }
        }
        */
    }
}