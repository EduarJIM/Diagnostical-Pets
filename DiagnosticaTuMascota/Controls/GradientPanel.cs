using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class GradientPanel : Panel
{
    public int CornerRadius { get; set; } = 24;
    public Color GradientColor1 { get; set; } = Color.FromArgb(25, 16, 185, 129);
    public Color GradientColor2 { get; set; } = Color.FromArgb(25, 20, 184, 166);
    public LinearGradientMode GradientMode { get; set; } = LinearGradientMode.ForwardDiagonal;
    public Color? BorderColor { get; set; } = null;
    public int BorderWidth { get; set; } = 1;

    public GradientPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
    }

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle bounds = new(0, 0, Width, Height);

        using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, CornerRadius);
        using LinearGradientBrush brush = new(bounds, GradientColor1, GradientColor2, GradientMode);
        g.FillPath(brush, path);

        if (BorderColor.HasValue && BorderColor.Value != Color.Transparent && BorderColor.Value.A > 0)
        {
            using Pen pen = new(BorderColor.Value, BorderWidth);
            g.DrawPath(pen, path);
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (CornerRadius > 0 && Width > 0 && Height > 0)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(path);
        }
    }
}
