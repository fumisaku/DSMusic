using DSMusic;
using DSMusic.Core;

namespace DSMusic;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        var settingsService = new SettingsService();
        settingsService.Load();

        Application.Run(new Forms.F000_Connect(settingsService));
    }
}
