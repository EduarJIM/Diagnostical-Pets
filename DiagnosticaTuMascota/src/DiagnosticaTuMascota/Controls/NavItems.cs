using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Ítem de navegación de la barra lateral, replicando NavItem.tsx.
/// Estado activo = fondo primary + texto blanco; inactivo = al pasar el cursor fondo sidebar-accent.
/// </summary>
public class NavItem : UserControl
{
    private bool _hovered;

    public NavItem()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        Height = 44;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    public string? IconText { get; set; } = "🏠";
    public bool Active { get; set; }
    /// <summary>Color de acento para ítems especiales (ej. Cerrar Sesión = rojo).</summary>
    public Color? AccentColor { get; set; }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        if (Active)
        {
            DrawingHelpers.FillRounded(g, AppTheme.Primary, rect, 10);
            DrawingHelpers.DrawSoftShadow(g, AppTheme.Primary, new Rectangle(2, 3, rect.Width, rect.Height), 10, 3);
        }
        else if (_hovered)
        {
            DrawingHelpers.FillRounded(g, AppTheme.SidebarAccent, rect, 10);
        }

        Color textColor;
        if (Active) textColor = Color.White;
        else if (AccentColor is { } accent) textColor = accent;
        else textColor = _hovered ? AppTheme.Foreground : AppTheme.MutedForeground;

        using var emoji = AppTheme.Emoji(9.5f);
        var iconRect = new Rectangle(12, (Height - 20) / 2, 22, 20);
        DrawingHelpers.DrawEmoji(g, IconText, emoji, iconRect, textColor);

        using var font = AppTheme.Medium(9.5f);
        var textRect = new Rectangle(40, 0, Width - 46, Height);
        TextRenderer.DrawText(g, Text, font, textRect, textColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }
}

/// <summary>
/// Píldora (tab) con estado activo relleno de color, como las pestañas de History.tsx.
/// </summary>
public class PillButton : UserControl
{
    private bool _hovered;

    public PillButton()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        Height = 40;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    public string? IconText { get; set; }
    public bool Active { get; set; }
    /// <summary>Color de relleno cuando está activo (primary, blue, amber...).</summary>
    public Color ActiveColor { get; set; } = AppTheme.Primary;

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        Color textColor;
        if (Active)
        {
            DrawingHelpers.FillRounded(g, ActiveColor, rect, rect.Height / 2);
            DrawingHelpers.DrawSoftShadow(g, ActiveColor, new Rectangle(2, 3, rect.Width, rect.Height), rect.Height / 2, 3);
            textColor = Color.White;
        }
        else
        {
            DrawingHelpers.FillRounded(g, AppTheme.Card, rect, rect.Height / 2);
            DrawingHelpers.DrawRounded(g, AppTheme.Border, 1f, rect, rect.Height / 2);
            textColor = _hovered ? AppTheme.Foreground : AppTheme.MutedForeground;
        }

        int x = 0;
        if (!string.IsNullOrEmpty(IconText))
        {
            using var emoji = AppTheme.Emoji(8.5f);
            var iconRect = new Rectangle(14, (Height - 18) / 2, 20, 18);
            DrawingHelpers.DrawEmoji(g, IconText, emoji, iconRect, textColor);
            x = 38;
        }

        using var font = AppTheme.Medium(9f);
        var textRect = new Rectangle(x, 0, Width - x - 12, Height);
        TextRenderer.DrawText(g, Text, font, textRect, textColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
    }
}