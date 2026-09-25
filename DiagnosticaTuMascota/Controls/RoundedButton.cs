using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class RoundedButton : Button
{
    public int CornerRadius { get; set; } = 16;
    public Color ButtonColor1 { get; set; } = AppTheme.Primary;
    public Color ButtonColor2 { get; set; } = AppTheme.Secondary;
    public bool UseGradient { get; set; } = true;
    public Color? SolidColor { get; set; } = null;
    public Color TextColor { get; set; } = Color.White;

    private bool _isHovered = false;
    private bool _isPressed = false;

    public RoundedButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;
        ForeColor = Color.White;
        Font = AppTheme.ButtonFont;
        Cursor = Cursors.Hand;
        Height = 48;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _isPressed = true;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _isPressed = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Rectangle bounds = new(0, 0, Width, Height);
        Color c1 = ButtonColor1;
        Color c2 = ButtonColor2;

        if (_isHovered)
        {
            c1 = ControlPaint.Light(ButtonColor1, 0.15f);
            c2 = ControlPaint.Light(ButtonColor2, 0.15f);
        }

        if (_isPressed)
        {
            c1 = ControlPaint.Dark(ButtonColor1, 0.1f);
            c2 = ControlPaint.Dark(ButtonColor2, 0.1f);
        }

        using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, CornerRadius);
        if (UseGradient)
        {
            using LinearGradientBrush brush = new(bounds, c1, c2, LinearGradientMode.Horizontal);
            g.FillPath(brush, path);
        }
        else
        {
            Color fill = SolidColor ?? c1;
            using SolidBrush brush = new(fill);
            g.FillPath(brush, path);
        }

        string displayText = Text;
        SizeF textSize = g.MeasureString(displayText, Font);
        float x = (Width - textSize.Width) / 2;
        float y = (Height - textSize.Height) / 2;

        using SolidBrush textBrush = new(TextColor);
        g.DrawString(displayText, Font, textBrush, x, y);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        if (Width > 0 && Height > 0)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(path);
        }
    }
}
