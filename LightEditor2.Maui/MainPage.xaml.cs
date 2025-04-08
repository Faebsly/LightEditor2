// LightEditor2.Maui/App.xaml.cs
using LightEditor2.Core.Data; // Für AppDbContext
using Microsoft.EntityFrameworkCore; // Für Migrate

namespace LightEditor2.Maui
{
    public partial class App : Application
    {
        // Konstruktor überarbeiten, um IServiceProvider zu verwenden
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Datenbank beim Start migrieren
            InitializeDatabase(serviceProvider);

            MainPage = new MainPage();
        }

        private void InitializeDatabase(IServiceProvider serviceProvider)
        {
            try
            {
                // Scope erstellen, um DbContext zu erhalten (Factory wird verwendet)
                using var scope = serviceProvider.CreateScope();
                // Wichtig: Da wir AddDbContextFactory verwenden, brauchen wir den Context selbst.
                // Die Factory ist für Services gut, hier brauchen wir eine Instanz.
                // Lösung 1: DbContext direkt anfordern (wenn AddDbContext auch registriert wäre - was es nicht ist)
                // Lösung 2: Factory verwenden, um Context zu erstellen
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var dbContext = dbContextFactory.CreateDbContext();

                // Migration durchführen
                dbContext.Database.Migrate();
                Console.WriteLine("Datenbankmigration beim Start erfolgreich durchgeführt."); // Logging
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FEHLER bei der Datenbankmigration beim Start: {ex}");
                // Hier evtl. Fehlerbehandlung für den Benutzer einbauen?
                // Oder zumindest ins Log schreiben.
            }
        }
    }
}