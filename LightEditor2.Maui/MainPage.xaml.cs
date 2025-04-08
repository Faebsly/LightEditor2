// LightEditor2.Maui/MainPage.xaml.cs
// using Microsoft.Maui.Controls; // Ist oft schon durch globale Usings oder SDK da

namespace LightEditor2.Maui
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Standardkonstruktor für die Hauptseite.
        /// </summary>
        public MainPage()
        {
            InitializeComponent(); // Initialisiert die XAML-Definition (MainPage.xaml)
                                   // und erstellt den BlazorWebView.
        }

        // Hier sollte KEIN Code für Datenbankmigrationen stehen!
    }
}