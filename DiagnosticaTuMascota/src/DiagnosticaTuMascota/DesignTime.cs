using System.ComponentModel;
using System.Text.Json;
using System.Windows.Forms;
using DiagnosticaTuMascota.Pages;
using DiagnosticaTuMascota.Storage;

namespace DiagnosticaTuMascota;

/// <summary>
/// Almacenamiento en memoria (nunca toca el disco). Se usa únicamente cuando el
/// Diseñador de Visual Studio instancia la aplicación para mostrar su diseño.
/// La aplicación real sigue usando <see cref="AppStorage.CreateDefault"/>.
/// </summary>
internal sealed class MemoryJsonStore : IJsonStore
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
    private readonly Dictionary<string, string> _data = new(StringComparer.OrdinalIgnoreCase);

    public T? Load<T>(string key) =>
        _data.TryGetValue(key, out var json) ? JsonSerializer.Deserialize<T>(json, Options) : default;

    public void Save<T>(string key, T value) => _data[key] = JsonSerializer.Serialize(value, Options);

    public void Delete(string key) => _data.Remove(key);

    public bool Exists(string key) => _data.ContainsKey(key);
}

/// <summary>
/// Navegador falso para el Diseñador: implementa <see cref="INavigator"/> sin hacer
/// nada. Permite que las páginas se construyan completas en tiempo de diseño
/// (con los datos de ejemplo de <c>DemoData</c>) sin tocar la navegación real.
/// </summary>
internal sealed class DesignTimeNavigator : INavigator
{
    public static readonly DesignTimeNavigator Instance = new();

    public AppStorage Storage { get; } = new(new MemoryJsonStore());

    public void Navigate(AppPage page, NavState? state = null)
    {
        // En diseño no se navega.
    }

    public void ShowToast(string message, ToastKind kind = ToastKind.Info)
    {
        // En diseño no se muestran avisos.
    }

    public void Logout()
    {
        // En diseño no se cierra sesión.
    }
}

/// <summary>Utilidades que solo actúan dentro del Diseñador de Visual Studio.</summary>
internal static class DesignTime
{
    /// <summary>Tamaño de lienzo para las páginas (el que ve el usuario en ejecución).</summary>
    public static readonly Size PageCanvas = new(1150, 800);

    /// <summary>Tamaño de lienzo de la ventana principal.</summary>
    public static readonly Size MainCanvas = new(1400, 900);

    public static AppStorage Storage { get; } = new(new MemoryJsonStore());

    public static bool IsActive => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    /// <summary>
    /// Dispara los manejadores <c>Resize</c> de la jerarquía para que el Diseñador
    /// muestre los controles en la misma posición que en ejecución (el proyecto
    /// posiciona parte de la interfaz desde esos manejadores).
    /// </summary>
    public static void PrimeLayout(Control root)
    {
        if (!IsActive) return;

        var children = new Control[root.Controls.Count];
        root.Controls.CopyTo(children, 0);

        foreach (var child in children)
        {
            try
            {
                // Un cambio de tamaño de 1 px dispara Resize; se restaura de inmediato.
                int width = child.Width;
                int height = child.Height;
                if (width > 1) child.Width = width - 1;
                if (height > 1) child.Height = height - 1;
                if (width > 1) child.Width = width;
                if (height > 1) child.Height = height;
                child.PerformLayout();
            }
            catch
            {
                // Un manejador que falle no debe impedir abrir el diseño.
            }
        }

        foreach (var child in children) PrimeLayout(child);
    }
}
