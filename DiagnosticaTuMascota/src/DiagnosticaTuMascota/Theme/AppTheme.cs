using System.Drawing;
using DiagnosticaTuMascota.Core;

namespace DiagnosticaTuMascota.Theme;

/// <summary>
/// Paleta de colores centralizada (tema claro/oscuro) portada de src/styles/theme.css.
/// Emite <see cref="ThemeChanged"/> al alternar el tema para que todos los controles se repinten.
/// </summary>
public static class AppTheme
{
    public static bool IsDark { get; private set; } = true; // La web arranca en tema oscuro.

    public static event EventHandler? ThemeChanged;

    public static void SetDark(bool dark)
    {
        if (dark == IsDark) return;
        IsDark = dark;
        ThemeChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Toggle() => SetDark(!IsDark);

    // ---- Colores ----
    public static Color Background => IsDark ? Color.FromArgb(6, 9, 16) : Color.FromArgb(241, 245, 249);
    public static Color Foreground => IsDark ? Color.FromArgb(248, 250, 252) : Color.FromArgb(2, 6, 23);
    public static Color Card => IsDark ? Color.FromArgb(15, 18, 25) : Color.White;
    public static Color Primary => Color.FromArgb(16, 185, 129);
    public static Color Secondary => Color.FromArgb(20, 184, 166);
    public static Color Muted => IsDark ? Color.FromArgb(30, 37, 54) : Color.FromArgb(226, 232, 240);
    public static Color MutedForeground => IsDark ? Color.FromArgb(148, 163, 184) : Color.FromArgb(51, 65, 85);
    public static Color Destructive => IsDark ? Color.FromArgb(255, 107, 107) : Color.FromArgb(239, 68, 68);
    public static Color ForegroundMuted => Color.FromArgb(IsDark ? 220 : 180, Foreground); // texto con transparencia
    public static Color Border => IsDark ? Color.FromArgb(255, 255, 255, 21) : Color.FromArgb(0, 0, 0, 31);
    public static Color SidebarBg => IsDark ? Color.FromArgb(10, 14, 20) : Color.FromArgb(255, 255, 255);
    public static Color SidebarAccent => IsDark ? Color.FromArgb(26, 31, 46) : Color.FromArgb(241, 245, 249);

    // ---- Colores de severidad ----
    public static Color SeverityCritical => Color.FromArgb(239, 68, 68);   // red-500
    public static Color SeverityModerate => Color.FromArgb(245, 158, 11);  // amber-500
    public static Color SeverityLeve => Color.FromArgb(16, 185, 129);      // emerald-500

    public static Color SeverityFor(SeverityLevel level) => level switch
    {
        SeverityLevel.CRITICO => SeverityCritical,
        SeverityLevel.MODERADO => SeverityModerate,
        _ => SeverityLeve
    };

    /// <summary>Color con transparencia (para fondos suaves tipo bg-primary/10).</summary>
    public static Color WithAlpha(Color color, int alpha) => Color.FromArgb(alpha, color);

    // ---- Tipografía (Segoe UI) ----
    public static Font Heading(float size = 22f) => new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
    public static Font SubHeading(float size = 14f) => new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
    public static Font Body(float size = 10f) => new("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);
    public static Font Medium(float size = 10f) => new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Point);
    public static Font Small(float size = 8.5f) => new("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);
    public static Font Tiny(float size = 7.5f) => new("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Fuente para iconos emoji.</summary>
    public static Font Emoji(float size = 10f) => new("Segoe UI Emoji", size, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>Fuente para texto de iconos de la barra lateral (MDL2/Symbol).</summary>
    public static Font Symbol(float size = 10f) => new("Segoe MDL2 Assets", size, FontStyle.Regular, GraphicsUnit.Point);
}