using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

/// <summary>Páginas de la aplicación.</summary>
public enum AppPage
{
    Login, Register, ForgotPassword, Dashboard, Pets,
    SymptomAnalysis, ConsultationStep2, DiagnosticResult, History, Settings
}

/// <summary>Estado que acompaña una navegación (síntomas seleccionados, nombre de mascota, etc.).</summary>
public sealed class NavState
{
    public List<string>? SelectedSymptoms { get; set; }
    public string? PetName { get; set; }
    public string? AdditionalInfo { get; set; }
    public object? Extra { get; set; }
}

/// <summary>Permite a las páginas navegar, leer el storage y mostrar toasts.</summary>
public interface INavigator
{
    AppStorage Storage { get; }
    void Navigate(AppPage page, NavState? state = null);
    void ShowToast(string message, ToastKind kind = ToastKind.Info);
    void Logout();
}

/// <summary>Base de todas las páginas: fondo del tema y notificación de navegación.</summary>
public abstract class PageBase : UserControl
{
    protected PageBase()
    {
        Dock = DockStyle.Fill;
        BackColor = AppTheme.Background;
        AppTheme.ThemeChanged += (_, _) => ApplyTheme();
    }

    public virtual void OnNavigatedTo(NavState? state) { ApplyTheme(); }
    public virtual void ApplyTheme() => BackColor = AppTheme.Background;
}