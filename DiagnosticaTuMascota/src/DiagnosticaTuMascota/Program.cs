namespace DiagnosticaTuMascota;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var storage = Storage.AppStorage.CreateDefault();
        Application.Run(new MainForm(storage));
    }
}