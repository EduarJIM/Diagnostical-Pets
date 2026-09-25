using System.Drawing;
using System.Drawing.Drawing2D;

namespace DiagnosticaTuMascota.Theme;

public static class AppTheme
{
    // === COLOR PALETTE - LIGHT MODE ===
    public static readonly Color BackgroundLight = Color.FromArgb(241, 245, 249);       // #F1F5F9
    public static readonly Color ForegroundLight = Color.FromArgb(2, 6, 23);             // #020617
    public static readonly Color CardLight = Color.White;                                 // #FFFFFF
    public static readonly Color CardForegroundLight = Color.FromArgb(2, 6, 23);         // #020617
    public static readonly Color MutedLight = Color.FromArgb(226, 232, 240);             // #E2E8F0
    public static readonly Color MutedForegroundLight = Color.FromArgb(51, 65, 85);      // #334155
    public static readonly Color BorderLight = Color.FromArgb(30, 30, 30);               // rgba(0,0,0,0.12)
    public static readonly Color InputBgLight = Color.FromArgb(226, 232, 240);           // #E2E8F0
    public static readonly Color SidebarBgLight = Color.White;                           // #FFFFFF
    public static readonly Color SidebarAccentLight = Color.FromArgb(241, 245, 249);     // #F1F5F9

    // === COLOR PALETTE - DARK MODE ===
    public static readonly Color BackgroundDark = Color.FromArgb(6, 9, 16);              // #060910
    public static readonly Color ForegroundDark = Color.FromArgb(248, 250, 252);         // #F8FAFC
    public static readonly Color CardDark = Color.FromArgb(15, 18, 25);                  // #0F1219
    public static readonly Color CardForegroundDark = Color.FromArgb(248, 250, 252);     // #F8FAFC
    public static readonly Color MutedDark = Color.FromArgb(30, 37, 54);                 // #1E2536
    public static readonly Color MutedForegroundDark = Color.FromArgb(148, 163, 184);    // #94A3B8
    public static readonly Color BorderDark = Color.FromArgb(25, 25, 30);                // rgba(255,255,255,0.08)
    public static readonly Color InputBgDark = Color.FromArgb(26, 31, 46);               // #1A1F2E
    public static readonly Color SidebarBgDark = Color.FromArgb(10, 14, 20);             // #0A0E14
    public static readonly Color SidebarAccentDark = Color.FromArgb(26, 31, 46);         // #1A1F2E

    // === BRAND COLORS (always the same) ===
    public static readonly Color Primary = Color.FromArgb(16, 185, 129);                 // #10B981 Emerald
    public static readonly Color PrimaryDark = Color.FromArgb(16, 185, 129);             // #10B981
    public static readonly Color Secondary = Color.FromArgb(20, 184, 166);               // #14B8A6 Teal
    public static readonly Color SecondaryDark = Color.FromArgb(20, 184, 166);           // #14B8A6
    public static readonly Color Destructive = Color.FromArgb(239, 68, 68);              // #EF4444
    public static readonly Color DestructiveDark = Color.FromArgb(255, 107, 107);        // #FF6B6B
    public static readonly Color Coral = Color.FromArgb(255, 107, 107);                  // #FF6B6B
    public static readonly Color NeonCoral = Color.FromArgb(255, 82, 82);                // #FF5252

    // === SEVERITY COLORS ===
    public static readonly Color Critical = Color.FromArgb(239, 68, 68);                 // #EF4444
    public static readonly Color CriticalBg = Color.FromArgb(254, 226, 226);            // #FEE2E2
    public static readonly Color Moderate = Color.FromArgb(245, 158, 11);                // #F59E0B
    public static readonly Color ModerateBg = Color.FromArgb(254, 243, 199);            // #FEF3C7
    public static readonly Color Mild = Color.FromArgb(16, 185, 129);                    // #10B981
    public static readonly Color MildBg = Color.FromArgb(209, 250, 229);                // #D1FAE5

    // === GRADIENT ACCENT COLORS ===
    public static readonly Color Blue500 = Color.FromArgb(59, 130, 246);                 // #3B82F6
    public static readonly Color Indigo500 = Color.FromArgb(99, 102, 241);               // #6366F1
    public static readonly Color Purple500 = Color.FromArgb(168, 85, 247);               // #A855F7
    public static readonly Color Cyan500 = Color.FromArgb(6, 182, 212);                  // #06B6D4
    public static readonly Color Orange500 = Color.FromArgb(249, 115, 22);               // #F97316
    public static readonly Color Amber500 = Color.FromArgb(245, 158, 11);                // #F59E0B

    // === STATIC HELPERS ===
    public static bool IsDarkMode { get; set; } = false;

    // === THEME-AWARE ACCESSORS ===
    public static Color Background => IsDarkMode ? BackgroundDark : BackgroundLight;
    public static Color Foreground => IsDarkMode ? ForegroundDark : ForegroundLight;
    public static Color Card => IsDarkMode ? CardDark : CardLight;
    public static Color CardForeground => IsDarkMode ? CardForegroundDark : CardForegroundLight;
    public static Color Muted => IsDarkMode ? MutedDark : MutedLight;
    public static Color MutedForeground => IsDarkMode ? MutedForegroundDark : MutedForegroundLight;
    public static Color Border => IsDarkMode ? BorderDark : BorderLight;
    public static Color InputBg => IsDarkMode ? InputBgDark : InputBgLight;
    public static Color SidebarBg => IsDarkMode ? SidebarBgDark : SidebarBgLight;
    public static Color SidebarAccent => IsDarkMode ? SidebarAccentDark : SidebarAccentLight;
    public static Color DestructiveCurrent => IsDarkMode ? DestructiveDark : Destructive;

    // === FONTS ===
    public static Font FontRegular(float size = 16f) => new("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel);
    public static Font FontMedium(float size = 16f) => new("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel);
    public static Font FontBold(float size = 16f) => new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Pixel);
    public static Font FontBlack(float size = 16f) => new("Segoe UI", size, FontStyle.Bold, GraphicsUnit.Pixel);

    // Named sizes
    public static Font TitleHero => FontBlack(40f);
    public static Font TitleH1 => FontBold(32f);
    public static Font TitleH2 => FontBold(24f);
    public static Font TitleH3 => FontBold(20f);
    public static Font TitleH4 => FontMedium(18f);
    public static Font Body => FontRegular(16f);
    public static Font BodySmall => FontRegular(14f);
    public static Font Small => FontRegular(12f);
    public static Font Tiny => FontRegular(10f);
    public static Font Helper => FontRegular(11f);
    public static Font Label => FontMedium(16f);
    public static Font ButtonFont => FontBold(16f);
    public static Font BadgeFont => new("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Pixel);

    // === GRADIENT HELPERS ===
    public static LinearGradientBrush CreatePrimaryGradient(Rectangle bounds)
    {
        return new LinearGradientBrush(bounds, Primary, Secondary, LinearGradientMode.Horizontal);
    }

    public static LinearGradientBrush CreateCardGradient(Rectangle bounds, Color color1, Color color2)
    {
        return new LinearGradientBrush(bounds, Color.FromArgb(25, color1), Color.FromArgb(25, color2), LinearGradientMode.ForwardDiagonal);
    }

    public static LinearGradientBrush CreateHeroGradient(Rectangle bounds)
    {
        return new LinearGradientBrush(bounds, Primary, Blue500, LinearGradientMode.ForwardDiagonal);
    }

    // === DRAWING HELPERS ===
    public static GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void DrawRoundedRectangle(Graphics g, Rectangle bounds, int radius, Color fillColor, Color? borderColor = null, int borderWidth = 1)
    {
        using GraphicsPath path = GetRoundedRectPath(bounds, radius);
        using SolidBrush brush = new(fillColor);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillPath(brush, path);

        if (borderColor.HasValue && borderColor.Value != Color.Transparent && borderColor.Value.A > 0)
        {
            using Pen pen = new(borderColor.Value, borderWidth);
            g.DrawPath(pen, path);
        }
    }

    public static void DrawRoundedRectangleGradient(Graphics g, Rectangle bounds, int radius, Color color1, Color color2, LinearGradientMode mode = LinearGradientMode.Horizontal, Color? borderColor = null, int borderWidth = 1)
    {
        using GraphicsPath path = GetRoundedRectPath(bounds, radius);
        using LinearGradientBrush brush = new(bounds, color1, color2, mode);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillPath(brush, path);

        if (borderColor.HasValue && borderColor.Value != Color.Transparent && borderColor.Value.A > 0)
        {
            using Pen pen = new(borderColor.Value, borderWidth);
            g.DrawPath(pen, path);
        }
    }

    public static void DrawGlow(Graphics g, Rectangle bounds, Color color, int glowSize = 20)
    {
        for (int i = glowSize; i > 0; i -= 2)
        {
            int alpha = (int)(40 * ((float)i / glowSize));
            using SolidBrush brush = new(Color.FromArgb(alpha, color));
            using GraphicsPath path = GetRoundedRectPath(bounds, bounds.Height / 2);
            g.FillPath(brush, path);
            bounds.Inflate(2, 2);
        }
    }

    public static void ApplyFormStyle(Form form, string title)
    {
        form.Text = title;
        form.BackColor = Background;
        form.ForeColor = Foreground;
        form.Font = Body;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.FormBorderStyle = FormBorderStyle.None;
        typeof(Control).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null, form, new object[] { true });
    }

    public static void DrawPlaceholderText(Graphics g, Control control, string text, Color? color = null)
    {
        if (!control.Focused && string.IsNullOrEmpty(control.Text))
        {
            Color c = color ?? MutedForeground;
            using SolidBrush brush = new(c);
            TextRenderer.DrawText(g, text, control.Font,
                new Rectangle(control.Padding.Left, 0, control.Width - control.Padding.Horizontal, control.Height),
                c, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }
}
