using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public enum ButtonVariant
{
    Primary,     // Degradado emerald→teal, texto blanco
    Secondary,   // Fondo sólido celeste (acciones tipo blue-500)
    Muted,       // Fondo Muted, texto MutedForeground
    Destructive, // Fondo Destructive, texto blanco
    Outline,     // Borde Border, fondo Card
    Ghost        // Transparente, texto MutedForeground
}

/// <summary>
/// Botón redondeado con estados hover/pressed/disabled, variantes y opción de
/// icono emoji a la izquierda del texto. Replica los estilos de la web.
/// </summary>
public class RoundedButton : Button
{
    private bool _hovered;
    private bool _pressed;

    public RoundedButton()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // sin reborde de color de sistema alrededor del botón
        Cursor = Cursors.Hand;
        Variant = ButtonVariant.Primary;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    public ButtonVariant Variant { get; set; }
    public int CornerRadius { get; set; } = 12;
    public string? Icon { get; set; }
    public float IconSize { get; set; } = 11f;
    /// <summary>Resaltado activo (nav). Pinta con fondo Primary y texto blanco.</summary>
    public bool NavActive { get; set; }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs mevent) { base.OnMouseDown(mevent); _pressed = true; Invalidate(); }
    protected override void OnMouseUp(MouseEventArgs mevent) { base.OnMouseUp(mevent); _pressed = false; Invalidate(); }

    private Color TextColor()
    {
        if (Variant is ButtonVariant.Primary or ButtonVariant.Secondary or ButtonVariant.Destructive)
            return Color.White;
        if (Variant == ButtonVariant.Muted) return AppTheme.MutedForeground;
        if (Variant == ButtonVariant.Outline) return AppTheme.Foreground;
        return AppTheme.MutedForeground;
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        int radius = Math.Min(CornerRadius, rect.Height / 2);

        // Sombra suave para variantes con color de marca
        if ((Variant == ButtonVariant.Primary || Variant == ButtonVariant.Secondary || Variant == ButtonVariant.Destructive) && Enabled && !_pressed)
        {
            DrawingHelpers.DrawSoftShadow(g, AppTheme.Primary, new Rectangle(2, 3, rect.Width, rect.Height), radius, 4);
        }

        Color bg = Color.Transparent;
        switch (Variant)
        {
            case ButtonVariant.Primary:
                if (NavActive)
                {
                    bg = AppTheme.Primary;
                    DrawingHelpers.FillRounded(g, bg, rect, radius);
                }
                else
                {
                    DrawingHelpers.FillGradientRounded(g, AppTheme.Primary, AppTheme.Secondary, rect, radius);
                }
                break;
            case ButtonVariant.Secondary:
                bg = Color.FromArgb(59, 130, 246); // blue-500
                DrawingHelpers.FillRounded(g, bg, rect, radius);
                break;
            case ButtonVariant.Muted:
                bg = _hovered ? AppTheme.SidebarAccent : AppTheme.Muted;
                DrawingHelpers.FillRounded(g, bg, rect, radius);
                break;
            case ButtonVariant.Destructive:
                DrawingHelpers.FillRounded(g, AppTheme.Destructive, rect, radius);
                break;
            case ButtonVariant.Outline:
                DrawingHelpers.FillRounded(g, AppTheme.Card, rect, radius);
                DrawingHelpers.DrawRounded(g, _hovered ? AppTheme.Primary : AppTheme.Border, 1f, rect, radius);
                break;
            case ButtonVariant.Ghost:
                if (_hovered) DrawingHelpers.FillRounded(g, AppTheme.Muted, rect, radius);
                break;
        }

        if (IsDisposed) return;

        // Estados hover/pressed mediante capas translúcidas
        if (Enabled && (_hovered || _pressed))
        {
            Color overlay = _pressed ? Color.FromArgb(35, Color.Black) : Color.FromArgb(28, Color.White);
            using var path = DrawingHelpers.RoundedRect(rect, radius);
            using var brush = new SolidBrush(overlay);
            g.FillPath(brush, path);
        }

        if (!Enabled)
        {
            using var path = DrawingHelpers.RoundedRect(rect, radius);
            using var brush = new SolidBrush(Color.FromArgb(90, AppTheme.Muted));
            g.FillPath(brush, path);
        }

        var textColor = Enabled ? TextColor() : AppTheme.MutedForeground;
        var flag = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        if (string.IsNullOrEmpty(Icon))
        {
            TextRenderer.DrawText(g, Text, Font, rect, textColor, flag);
        }
        else
        {
            var iconFont = AppTheme.Emoji(IconSize);
            // Medir con GDI+ (los emoji a color son más anchos que su versión GDI monocroma)
            var iconSize = g.MeasureString(Icon, iconFont).ToSize();
            var full = TextRenderer.MeasureText(g, Text, Font);
            var gap = 6;
            var total = iconSize.Width + gap + full.Width;
            var left = (Width - total) / 2f;
            var iconRect = new Rectangle((int)left, (Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
            var textRect = new Rectangle(iconRect.Right + gap, 0, full.Width + 4, Height);

            DrawingHelpers.DrawEmoji(g, Icon, iconFont, iconRect, textColor);
            TextRenderer.DrawText(g, Text, Font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding);
        }
    }
}