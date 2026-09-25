using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class ToggleSwitch : Control
{
    public bool IsOn { get; set; } = false;
    public Color OnColor { get; set; } = Color.FromArgb(251, 191, 36);
    public Color OffColor { get; set; } = Color.FromArgb(148, 163, 184);
    public Color ThumbColor { get; set; } = Color.White;

    public event EventHandler? Toggled;

    public ToggleSwitch()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Size = new Size(56, 28);
        Cursor = Cursors.Hand;
    }

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        IsOn = !IsOn;
        Toggled?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Track
        Rectangle trackBounds = new(0, (Height - 24) / 2, 48, 24);
        Color trackColor = IsOn ? OnColor : (AppTheme.IsDarkMode ? Color.FromArgb(30, 41, 59) : Color.FromArgb(203, 213, 225));

        using GraphicsPath trackPath = AppTheme.GetRoundedRectPath(trackBounds, 12);
        using SolidBrush trackBrush = new(trackColor);
        g.FillPath(trackBrush, trackPath);

        // Thumb
        int thumbX = IsOn ? 28 : 4;
        Rectangle thumbBounds = new(thumbX, (Height - 20) / 2, 20, 20);

        using GraphicsPath thumbPath = AppTheme.GetRoundedRectPath(thumbBounds, 10);
        using SolidBrush thumbBrush = new(ThumbColor);
        g.FillPath(thumbBrush, thumbPath);

        // Sun/Moon icon on thumb
        string icon = IsOn ? "\u2600" : "\u263D";
        using Font iconFont = new("Segoe UI Symbol", 10f);
        using SolidBrush iconBrush = new(IsOn ? Color.FromArgb(120, Color.Black) : Color.FromArgb(120, Color.Gray));
        SizeF iconSize = g.MeasureString(icon, iconFont);
        g.DrawString(icon, iconFont, iconBrush,
            thumbX + (20 - iconSize.Width) / 2,
            (Height - iconSize.Height) / 2);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        Height = 28;
    }
}
