using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Panel con esquinas redondeadas, color de relleno y borde configurables.
/// Releye el tema actual automáticamente.
/// </summary>
public class RoundedPanel : Panel
{
    private int _cornerRadius = 16;
    private Color? _fillColor;
    private Color? _borderColor;

    public RoundedPanel()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; UpdateRegion(); Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color? FillColor
    {
        get => _fillColor;
        set { _fillColor = value; Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color? BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool ShowBorder { get; set; } = true;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public float BorderWidth { get; set; } = 1f;

    public Color EffectiveFill => _fillColor ?? AppTheme.Card;

    public void UpdateRegion()
    {
        if (Width <= 0 || Height <= 0) return;
        // Región ligeramente mayor para conservar el antialias de las esquinas
        // (una región exacta recorta los píxeles de suavizado → esquinas dentadas).
        using var path = DrawingHelpers.RoundedRect(new Rectangle(-1, -1, Width + 2, Height + 2), _cornerRadius);
        Region = new Region(path);
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        UpdateRegion();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        DrawingHelpers.FillRounded(e.Graphics, EffectiveFill, rect, _cornerRadius);
        if (ShowBorder)
        {
            DrawingHelpers.DrawRounded(e.Graphics, _borderColor ?? AppTheme.Border, BorderWidth, rect, _cornerRadius);
        }
    }
}

/// <summary>Panel con relleno degradado lineal.</summary>
public class GradientPanel : Panel
{
    private int _cornerRadius = 16;

    public GradientPanel()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; UpdateRegion(); Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color StartColor { get; set; } = AppTheme.Primary;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Color EndColor { get; set; } = AppTheme.Secondary;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public float Angle { get; set; } = 45f;

    public void UpdateRegion()
    {
        // Región ligeramente mayor para conservar el antialias de las esquinas.
        using var path = DrawingHelpers.RoundedRect(new Rectangle(-1, -1, Width + 2, Height + 2), _cornerRadius);
        Region = new Region(path);
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        UpdateRegion();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        DrawingHelpers.FillGradientRounded(e.Graphics, StartColor, EndColor, rect, _cornerRadius, Angle);
    }
}