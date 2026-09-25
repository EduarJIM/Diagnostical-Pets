using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class RoundedComboBox : ComboBox
{
    public int CornerRadius { get; set; } = 12;
    public Color BorderColor { get; set; } = Color.FromArgb(200, AppTheme.Muted);
    public Color FocusBorderColor { get; set; } = AppTheme.Primary;
    public string PlaceholderText { get; set; } = "";

    private bool _isFocused = false;

    public RoundedComboBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = AppTheme.InputBg;
        ForeColor = AppTheme.Foreground;
        Font = AppTheme.Body;
        DropDownStyle = ComboBoxStyle.DropDownList;
        Height = 48;
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 28;
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

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Rectangle bounds = new(0, 0, Width, Height);
        Color border = _isFocused ? FocusBorderColor : BorderColor;
        AppTheme.DrawRoundedRectangle(g, bounds, CornerRadius, BackColor, border, _isFocused ? 2 : 1);

        // Dropdown arrow
        Rectangle arrowBounds = new(Width - 36, 0, 36, Height);
        string arrow = "\u25BC";
        using Font arrowFont = new("Segoe UI", 10f);
        using SolidBrush arrowBrush = new(AppTheme.MutedForeground);
        SizeF arrowSize = g.MeasureString(arrow, arrowFont);
        g.DrawString(arrow, arrowFont, arrowBrush,
            arrowBounds.X + (arrowBounds.Width - arrowSize.Width) / 2,
            (Height - arrowSize.Height) / 2);

        // Selected text
        if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
        {
            Rectangle textBounds = new(Padding.Left + 8, 0, Width - Padding.Horizontal - 36, Height);
            using SolidBrush textBrush = new(ForeColor);
            TextRenderer.DrawText(g, Items[SelectedIndex].ToString(), Font, textBounds, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
        else if (!string.IsNullOrEmpty(PlaceholderText))
        {
            AppTheme.DrawPlaceholderText(g, this, PlaceholderText, AppTheme.MutedForeground);
        }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle bounds = e.Bounds;

        Color bgColor = (e.State & DrawItemState.Selected) != 0
            ? Color.FromArgb(30, AppTheme.Primary)
            : AppTheme.Card;

        using SolidBrush bgBrush = new(bgColor);
        e.Graphics.FillRectangle(bgBrush, bounds);

        using SolidBrush textBrush = new(AppTheme.Foreground);
        e.Graphics.DrawString(Items[e.Index].ToString(), Font, textBrush, bounds.X + 12, bounds.Y + (bounds.Height - Font.Height) / 2);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        Height = 48;
        if (Width > 0)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region = new Region(path);
        }
    }
}
