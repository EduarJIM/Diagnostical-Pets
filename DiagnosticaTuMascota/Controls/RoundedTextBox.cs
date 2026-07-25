using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class RoundedTextBox : TextBox
{
    public int CornerRadius { get; set; } = 12;
    public new string PlaceholderText { get; set; } = "";
    public Color BorderColor { get; set; } = Color.FromArgb(200, AppTheme.Muted);
    public Color FocusBorderColor { get; set; } = AppTheme.Primary;
    public Color PlaceholderColor { get; set; } = AppTheme.MutedForeground;

    private bool _isFocused = false;

    public RoundedTextBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BorderStyle = BorderStyle.None;
        BackColor = AppTheme.InputBg;
        ForeColor = AppTheme.Foreground;
        Font = AppTheme.Body;
        Padding = new Padding(12, 8, 12, 8);
        Height = 48;
        MinimumSize = new Size(0, 48);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        _isFocused = true;
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        _isFocused = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Rectangle bounds = new(0, 0, Width, Height);
        Color border = _isFocused ? FocusBorderColor : BorderColor;
        Color bg = AppTheme.InputBg;

        AppTheme.DrawRoundedRectangle(g, bounds, CornerRadius, bg, border, _isFocused ? 2 : 1);

        if (!Focused && string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(PlaceholderText))
        {
            Color c = PlaceholderColor;
            using SolidBrush brush = new(c);
            TextRenderer.DrawText(g, PlaceholderText, Font,
                new Rectangle(Padding.Left, 0, Width - Padding.Horizontal, Height),
                c, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        MinimumSize = new Size(0, 48);
        if (Width > 0 && Height > 0)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(path);
        }
    }
}
