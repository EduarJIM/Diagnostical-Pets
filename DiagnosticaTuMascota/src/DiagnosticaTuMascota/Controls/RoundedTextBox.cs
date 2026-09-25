using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Campo de texto redondeado con icono opcional (emoji) y placeholder.
/// Compuesto: caja interior sin borde + fondo/borde dibujados con GDI+.
/// </summary>
public class RoundedTextBox : UserControl
{
    private readonly TextBox _inner;
    private readonly Label _placeholder;
    private readonly Label _iconLabel;
    private bool _focused;

    public RoundedTextBox()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

        _inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor = AppTheme.Background,
            ForeColor = AppTheme.Foreground,
            Font = AppTheme.Body(11f),
            Location = new Point(2, 2)
        };
        _inner.GotFocus += (_, _) => { _focused = true; UpdatePlaceholder(); Invalidate(); };
        _inner.LostFocus += (_, _) => { _focused = false; UpdatePlaceholder(); Invalidate(); };
        _inner.KeyDown += (_, e) => OnKeyDown(e);

        _placeholder = new Label
        {
            AutoSize = true,
            BackColor = AppTheme.Background,
            ForeColor = AppTheme.MutedForeground,
            Font = AppTheme.Body(11f),
            Cursor = Cursors.IBeam
        };
        _placeholder.Click += (_, _) => _inner.Focus();
        _iconLabel.Click += (_, _) => _inner.Focus();

        _iconLabel = new Label
        {
            AutoSize = true,
            BackColor = Color.Transparent,
            Font = AppTheme.Emoji(11f),
            ForeColor = AppTheme.MutedForeground,
            Cursor = Cursors.Default,
            UseCompatibleTextRendering = true // emoji a color (GDI+), no monocromo
        };

        Controls.Add(_inner);
        Controls.Add(_iconLabel);
        Controls.Add(_placeholder);

        AutoScaleMode = AutoScaleMode.None;
        AppTheme.ThemeChanged += (_, _) => ApplyColors();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public new string Text
    {
        get => _inner.Text;
        set => _inner.Text = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string PlaceholderText
    {
        get => _placeholder.Text;
        set { _placeholder.Text = value; UpdatePlaceholder(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string? IconText
    {
        get => _iconLabel.Text;
        set { _iconLabel.Text = value ?? ""; LayoutChildren(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public char PasswordChar
    {
        get => _inner.PasswordChar;
        set => _inner.PasswordChar = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool Multiline
    {
        get => _inner.Multiline;
        set
        {
            _inner.Multiline = value;
            _inner.AcceptsReturn = value;
            _inner.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
            LayoutChildren();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string[] Lines
    {
        get => _inner.Lines;
        set => _inner.Lines = value ?? Array.Empty<string>();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int CornerRadius { get; set; } = 12;

    private int _fieldHeight = 44;

    /// <summary>
    /// Alto por defecto para campos de una línea. Solo se aplica desde
    /// <see cref="LayoutChildren"/> cuando el control aún no recibió un alto explícito
    /// (Height &lt;= 0); así un inicializador "Height = 44, FieldHeight = 44" no
    /// aplasta el campo a alto 0 (antes, asignar Height disparaba el layout cuando
    /// FieldHeight aún valía 0 y el campo quedaba invisible = login inservible).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int FieldHeight
    {
        get => _fieldHeight;
        set { _fieldHeight = Math.Max(0, value); LayoutChildren(); }
    }

    internal TextBox Inner => _inner;

    /// <summary>
    /// Tamaño preferido real: imprescindible para que las filas AutoSize de los
    /// TableLayoutPanel midan el alto correcto. Sin esto, los UserControl reportan
    /// (0,0) y los campos colapsan/pisan a los controles vecinos.
    /// </summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); UpdatePlaceholder(); }
    protected override void OnFontChanged(EventArgs e) { base.OnFontChanged(e); _inner.Font = Font; LayoutChildren(); }
    protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); LayoutChildren(); }
    protected override void OnForeColorChanged(EventArgs e) { base.OnForeColorChanged(e); _inner.ForeColor = ForeColor; }
    protected override void OnResize(EventArgs e) { base.OnResize(e); LayoutChildren(); }

    private void LayoutChildren()
    {
        if (_inner == null) return;
        // Solo rellenar el alto con FieldHeight si la página no lo asignó ya
        // (Height <= 0). Antes era "Height = FieldHeight" incondicional: al iniciar
        // un campo con "Height = 44, FieldHeight = 44", asignar Height ejecutaba
        // este método cuando FieldHeight aún era 0 y aplastaba el campo a alto 0
        // (invisible → el login nunca capturaba lo escrito).
        if (!_inner.Multiline && Height <= 0 && FieldHeight > 0) Height = FieldHeight;
        int y = (Height - _inner.Height) / 2; // márgenes verticales
        int inset = y > 0 ? y : 2;
        _inner.Location = new Point(inset, inset);
        _inner.Width = Width - inset * 2;
        _inner.Height = Height - inset * 2;
        _inner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _inner.BringToFront();

        if (string.IsNullOrEmpty(_iconLabel.Text))
        {
            _iconLabel.Visible = false;
            _inner.Location = new Point(inset + 10, inset);
            _inner.Width = Width - (inset + 10) * 2;
        }
        else
        {
            _iconLabel.Visible = true;
            int iconLeft = 14;
            _iconLabel.Location = new Point(iconLeft, (Height - _iconLabel.Height) / 2);
            int textLeft = iconLeft + _iconLabel.Width + 6;
            _inner.Location = new Point(textLeft, inset);
            _inner.Width = Width - textLeft - 10;
        }
    }

    private void UpdatePlaceholder()
    {
        _placeholder.Visible = !_focused && string.IsNullOrEmpty(_inner.Text) && !string.IsNullOrEmpty(_placeholder.Text);

        // Z-order: cuando el campo está vacío, el placeholder y el icono se dibujan
        // por encima (pista visual visible); el TextBox real pasa al frente solo con
        // foco o texto (antes el TextBox opaco tapaba placeholder e icono siempre).
        if (_placeholder.Visible)
        {
            _iconLabel.BringToFront();
            _placeholder.BringToFront();
            _inner.SendToBack();
        }
        else
        {
            _inner.BringToFront();
        }

        int left = string.IsNullOrEmpty(_iconLabel.Text) ? 24 : 14 + (_iconLabel.Visible ? _iconLabel.Width + 6 : 0);
        _placeholder.Location = new Point(left, (Height - _placeholder.Height) / 2);
    }

    private void ApplyColors()
    {
        _inner.BackColor = AppTheme.Background;
        _inner.ForeColor = AppTheme.Foreground;
        _placeholder.BackColor = AppTheme.Background;
        _iconLabel.ForeColor = _focused ? AppTheme.Primary : AppTheme.MutedForeground;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        DrawingHelpers.FillRounded(e.Graphics, AppTheme.Background, rect, CornerRadius);
        var borderColor = _focused ? Color.FromArgb(150, AppTheme.Primary) : AppTheme.Border;
        DrawingHelpers.StrokeRounded(e.Graphics, borderColor, _focused ? 1.6f : 1f, rect, CornerRadius);
    }
}