using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class RoundedPanel : Panel
{
    public int CornerRadius { get; set; } = 24;
    public Color BorderColor { get; set; } = Color.Transparent;
    public int BorderWidth { get; set; } = 1;
    public Color? FillColor { get; set; } = null;

    public RoundedPanel()
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
        Color bg = FillColor ?? AppTheme.Card;

        if (CornerRadius > 0)
        {
            AppTheme.DrawRoundedRectangle(g, bounds, CornerRadius, bg, BorderColor, BorderWidth);
        }
        else
        {
            using SolidBrush brush = new(bg);
            g.FillRectangle(brush, bounds);
            if (BorderColor != Color.Transparent && BorderColor.A > 0)
            {
                using Pen pen = new(BorderColor, BorderWidth);
                g.DrawRectangle(pen, bounds);
            }
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
