// LightEditor2.Maui/MauiProgram.cs
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui; // Für Toolkit (FolderPicker etc.)

// Namespaces aus Ihren Projekten
using LightEditor2.Core.Data;
using LightEditor2.Core.Services;
using LightEditor2.RazorLib.Components;
using LightEditor2.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using LightEditor2.Maui.Services;

namespace LightEditor2.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Community Toolkit initialisieren (für FolderPicker, FileSaver, Snackbar...)
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Globale Pfade
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "lighteditor2.db"); // Eindeutiger DB-Name

            // === Abhängigkeitsregistrierung ===

            // 1. Logging
            builder.Logging.AddDebug();
#if DEBUG
            // Entwicklertools für BlazorWebView nur im Debug-Modus
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            // 2. Datenbankkontext Factory
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));

            // 3. Eigene Services aus LightEditor2.Core registrieren
            // Singleton Services (nur eine Instanz pro App)
            builder.Services.AddSingleton<CurrentStateService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<AppInfoService>();

            // Transient Services (neue Instanz bei jeder Anforderung)
            builder.Services.AddTransient<ISettingService, SettingService>();
            builder.Services.AddTransient<IProjectService, ProjectService>();
            builder.Services.AddTransient<ISubGroupService, SubGroupService>();
            builder.Services.AddTransient<ISlideService, SlideService>();
            builder.Services.AddTransient<IDataManagementService, DataManagementService>();
            builder.Services.AddTransient<IFilePickerService, MauiFilePickerService>();
            builder.Services.AddTransient<IFolderPickerService, MauiFolderPickerService>();
            builder.Services.AddTransient<IFileSaverService, MauiFileSaverService>();
            builder.Services.AddTransient<IDialogService, MauiDialogService>();

            // 4. MAUI Blazor WebView Service hinzufügen
            builder.Services.AddMauiBlazorWebView();

            // 5. MAUI Community Toolkit Services (automatisch durch UseMauiCommunityToolkit(), aber hier explizit für Klarheit)
            // Diese Interfaces können jetzt injiziert werden: IFilePicker, IFolderPicker, IFileSaver, ISnackbar etc.
            // builder.Services.AddSingleton(FilePicker.Default); // Beispiel, nicht nötig bei Use...Toolkit()
            // builder.Services.AddSingleton(FolderPicker.Default);
            // builder.Services.AddSingleton(FileSaver.Default);


            return builder.Build();
        }
    }
}