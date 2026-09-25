using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// ComboBox redondeado (estilo DropDownList) con items owner-draw y flecha propia.
/// </summary>
public class RoundedComboBox : UserControl
{
    private readonly ComboBox _inner;
    private bool _down;

    public RoundedComboBox()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

        _inner = new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            DrawMode = DrawMode.OwnerDrawFixed,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = AppTheme.Background,
            ForeColor = AppTheme.Foreground,
            Font = AppTheme.Body(10.5f),
            IntegralHeight = true
        };
        _inner.DrawItem += InnerOnDrawItem;
        _inner.DropDown += (_, _) => _down = true;
        _inner.DropDownClosed += (_, _) => { _down = false; Invalidate(); };
        _inner.SelectedIndexChanged += (_, _) => { SelectedIndexChanged?.Invoke(this, EventArgs.Empty); Invalidate(); };

        Controls.Add(_inner);
        AppTheme.ThemeChanged += (_, _) => { _inner.BackColor = AppTheme.Background; _inner.ForeColor = AppTheme.Foreground; Invalidate(); };
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int CornerRadius { get; set; } = 12;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int FieldHeight { get; set; } = 44;

    /// <summary>Tamaño preferido real (evita que colapse en filas AutoSize de TableLayoutPanel).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    /// <summary>Se dispara al cambiar el elemento seleccionado.</summary>
    public event EventHandler? SelectedIndexChanged;

    public ComboBox.ObjectCollection Items => _inner.Items;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public object? SelectedItem { get => _inner.SelectedItem; set => _inner.SelectedItem = value; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int SelectedIndex { get => _inner.SelectedIndex; set => _inner.SelectedIndex = value; }
    public string SelectedText => _inner.SelectedItem?.ToString() ?? "";

    public override string Text => SelectedText;

    public void SetItems(IEnumerable<string> values)
    {
        _inner.Items.Clear();
        foreach (var v in values) _inner.Items.Add(v);
        if (_inner.Items.Count > 0) _inner.SelectedIndex = 0;
    }

    public void SetItems<T>(IEnumerable<T> values, Func<T, string> display)
    {
        _inner.Items.Clear();
        foreach (var v in values) _inner.Items.Add(display(v));
        if (_inner.Items.Count > 0) _inner.SelectedIndex = 0;
    }

    protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); LayoutChildren(); }
    protected override void OnResize(EventArgs e) { base.OnResize(e); LayoutChildren(); }

    private void LayoutChildren()
    {
        if (_inner == null) return;
        // No aplastar el alto con FieldHeight cuando la página ya asignó un alto
        // explícito (mismo fix que RoundedTextBox: "Height = 44, FieldHeight = 44"
        // en ese orden aplastaba el control a 0 al leer FieldHeight antes de asignarlo).
        if (Height <= 0) Height = FieldHeight;
        _inner.Location = new Point(3, 3);
        _inner.Width = Width - 6;
        _inner.Height = Height - 6;
    }

    private void InnerOnDrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;
        if (e.Index >= _inner.Items.Count) return;

        e.DrawBackground();
        bool selectedItemFace = !_down && e.Index == _inner.SelectedIndex;
        var bg = selectedItemFace ? AppTheme.Background : (e.State.HasFlag(DrawItemState.Selected) ? AppTheme.SidebarAccent : AppTheme.Background);
        var rect = e.Bounds;
        using var brush = new SolidBrush(bg);
        e.Graphics.FillRectangle(brush, rect);

        var text = _inner.Items[e.Index].ToString() ?? "";
        var color = e.State.HasFlag(DrawItemState.Selected) && !selectedItemFace
            ? AppTheme.Foreground
            : AppTheme.Foreground;

        if (selectedItemFace)
        {
            // Texto de la cara cerrada + flecha
            TextRenderer.DrawText(e.Graphics, text, _inner.Font, new Rectangle(rect.X + 14, rect.Y, rect.Width - 46, rect.Height),
                color, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            using var arrowFont = AppTheme.Symbol(8f);
            TextRenderer.DrawText(e.Graphics, "\uE70D", arrowFont, new Rectangle(rect.Right - 36, rect.Y, 24, rect.Height),
                AppTheme.MutedForeground, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
        }
        else
        {
            TextRenderer.DrawText(e.Graphics, text, _inner.Font, new Rectangle(rect.X + 14, rect.Y, rect.Width - 28, rect.Height),
                color, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        e.DrawFocusRectangle();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        DrawingHelpers.FillRounded(e.Graphics, AppTheme.Background, rect, CornerRadius);
        DrawingHelpers.StrokeRounded(e.Graphics, AppTheme.Border, 1f, rect, CornerRadius);
    }
}