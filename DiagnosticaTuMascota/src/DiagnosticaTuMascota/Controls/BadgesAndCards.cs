using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Etiqueta de estado (CRÍTICO/MODERADO/LEVE) con fondo suave del color correspondiente.
/// </summary>
public class SeverityBadge : Control
{
    public SeverityBadge()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        Height = 22;
        AutoSize = false;
        BackColor = Color.Transparent;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string? Label { get; set; } = "LEVE";
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public SeverityColor ColorKind { get; set; } = SeverityColor.Leve;

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    public enum SeverityColor { Leve, Moderado, Critico, Blue, Amber }

    private Color BaseColor() => ColorKind switch
    {
        SeverityColor.Critico => AppTheme.SeverityCritical,
        SeverityColor.Moderado => AppTheme.SeverityModerate,
        SeverityColor.Blue => Color.FromArgb(59, 130, 246),
        SeverityColor.Amber => AppTheme.SeverityModerate,
        _ => AppTheme.SeverityLeve
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        var baseColor = BaseColor();

        using var bg = new SolidBrush(Color.FromArgb(24, baseColor));
        using var path = DrawingHelpers.RoundedRect(rect, rect.Height / 2);
        g.FillPath(bg, path);

        using var font = AppTheme.Medium(7.5f);
        TextRenderer.DrawText(g, Label, font, rect, Color.FromArgb(230, baseColor),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}

/// <summary>
/// Tarjeta de funcionalidad clicable (FeatureCard.tsx): degradado de fondo, icono
/// en caja, título y descripción. Al pasar el cursor se ilumina y eleva.
/// </summary>
public class FeatureCard : UserControl
{
    private bool _hovered;

    public FeatureCard()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        Height = 168;
        BackColor = Color.Transparent; // los rellenos son traslúcidos: requiere fondo del padre
        Cursor = Cursors.Hand;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string? IconText { get; set; } = "🩺";
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color TintStart { get; set; } = AppTheme.WithAlpha(AppTheme.Primary, 26);
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color TintEnd { get; set; } = AppTheme.WithAlpha(AppTheme.Secondary, 26);
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string CardTitle { get; set; } = "Título";
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string CardDescription { get; set; } = "";

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        if (_hovered)
        {
            DrawingHelpers.DrawSoftShadow(g, AppTheme.Primary, new Rectangle(3, 5, rect.Width, rect.Height), 20, 6);
            rect = new Rectangle(0, -2, Width - 1, Height - 1); // efecto de elevación
        }

        using var path = DrawingHelpers.RoundedRect(rect, 18);
        using var brush = new LinearGradientBrush(rect, TintStart, TintEnd, 45f);
        g.FillPath(brush, path);
        DrawingHelpers.StrokeRounded(g, Color.FromArgb(120, AppTheme.Border), 1f, rect, 18);

        // Caja del icono
        var iconBox = new Rectangle(24, 24, 48, 48);
        using (var ipath = DrawingHelpers.RoundedRect(iconBox, 10))
        using (var ib = new SolidBrush(AppTheme.Card))
        {
            g.FillPath(ib, ipath);
        }
        DrawingHelpers.StrokeRounded(g, AppTheme.Border, 1f, iconBox, 10);
        using (var emoji = AppTheme.Emoji(14f))
        {
            DrawingHelpers.DrawEmoji(g, IconText, emoji, iconBox);
        }

        using (var titleFont = AppTheme.Medium(12f))
        {
            TextRenderer.DrawText(g, CardTitle, titleFont, new Rectangle(24, 86, Width - 48, 26),
                _hovered ? AppTheme.Primary : AppTheme.Foreground,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        using (var descFont = AppTheme.Small(9f))
        {
            TextRenderer.DrawText(g, CardDescription, descFont, new Rectangle(24, 114, Width - 48, 44),
                AppTheme.MutedForeground,
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }
    }
}

/// <summary>
/// Anfitrión centrado: centra su contenido y permite scroll vertical.
/// Con MaxWidth define un área de contenido de ancho máximo (max-w-6xl).
/// </summary>
public class PageBody : Panel
{
    public const int MarginX = 40;

    public PageBody()
    {
        AutoScroll = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    /// <summary>Ancho máximo del contenido (0 = sin límite).</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int MaxWidth { get; set; } = 1080;

    public void CenterChild()
    {
        int y = MarginX;
        foreach (Control c in Controls)
        {
            if (!c.Visible) continue;
            // Quitar Dock para que el posicionamiento manual (centrado + MaxWidth)
            // realmente aplique; antes los hijos con Dock=Fill se estiraban a todo el ancho.
            c.Dock = DockStyle.None;
            int target = MaxWidth <= 0 ? ClientSize.Width - MarginX * 2 : Math.Min(MaxWidth, ClientSize.Width - MarginX * 2);
            c.Width = Math.Max(320, target);
            c.Left = Math.Max(0, (ClientSize.Width - c.Width) / 2);
            c.Top = y;
            y += c.Height + 18; // varios hijos se apilan en vertical (antes todos en el mismo Top)
        }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        e.Control.Anchor = AnchorStyles.None;
        CenterChild();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        CenterChild();
    }
}