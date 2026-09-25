using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        AppTheme.IsDarkMode = false;
        Application.Run(new Forms.Login.LoginForm());
    }
}
