using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Ventana modal con tarjeta redondeada y fondo oscuro translúcido
/// (replica los <div className="fixed inset-0 ... bg-black/40"> de la web).
/// </summary>
public class ModalHost
{
    public static DialogResult ShowDialog(Form owner, ModalForm modal, bool closeOnBackdrop = true)
    {
        var overlay = new OverlayForm(owner)
        {
            TopMost = true,
            CloseOnClick = closeOnBackdrop,
            ModalToClose = modal
        };
        overlay.Show(owner);

        modal.Owner = owner;
        modal.TopMost = true;
        var result = modal.ShowDialog(owner);
        modal.TopMost = false;

        if (!overlay.IsDisposed) overlay.Close();
        return result;
    }
}

internal class OverlayForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool CloseOnClick { get; set; } = true;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Form? ModalToClose { get; set; }

    public OverlayForm(Form target)
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.Black;
        Opacity = 0.45;
        Bounds = target.Bounds;
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        if (CloseOnClick && ModalToClose is not null && !ModalToClose.IsDisposed)
        {
            ModalToClose.DialogResult = DialogResult.Cancel;
        }
    }
}

/// <summary>
/// Base para los diálogos: tarjeta redondeada con encabezado, botón de cierre y
/// área de contenido desplazable.
/// </summary>
public class ModalForm : Form
{
    private readonly Panel _workspace;
    private readonly Panel _contentHost;
    private bool _hoverClose;

    public ModalForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.Card;
        DoubleBuffered = true;
        Padding = new Padding(0);
        AppTheme.ThemeChanged += (_, _) =>
        {
            BackColor = AppTheme.Card;
            Invalidate();
            UpdateRegion();
        };

        _workspace = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

        _contentHost = new Panel
        {
            AutoScroll = true,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Padding = new Padding(32, 10, 32, 28)
        };
        _workspace.Controls.Add(_contentHost);
        Controls.Add(_workspace);

        // Botón de cierre en la parte superior derecha
        var close = new RoundedButton
        {
            Variant = ButtonVariant.Muted,
            Text = "✕",
            Icon = null,
            CornerRadius = 20,
            Size = new Size(38, 38),
            Location = new Point(0, 0),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Font = AppTheme.Medium(9f)
        };
        close.Click += (_, _) => Close();
        _workspace.Controls.Add(close);
        close.BringToFront();
        _closeButton = close;

        UpdateRegion();
    }

    private readonly RoundedButton _closeButton;

    /// <summary>Pane de contenido (con scroll interno).</summary>
    public Panel Content => _contentHost;

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRegion();
        if (_closeButton is not null)
        {
            _closeButton.Location = new Point(ClientSize.Width - _closeButton.Width - 22, 20);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = DrawingHelpers.RoundedRect(rect, 24))
        using (var b = new SolidBrush(AppTheme.Card))
        {
            g.FillPath(b, path);
        }
        DrawingHelpers.StrokeRounded(g, AppTheme.Border, 1f, rect, 24);
    }

    public void UpdateRegion()
    {
        if (Width <= 0 || Height <= 0) return;
        // Región ligeramente mayor para conservar el antialias de las esquinas
        // (una región exacta recorta los píxeles de suavizado y deja esquinas dentadas).
        using var path = DrawingHelpers.RoundedRect(new Rectangle(-1, -1, Width + 2, Height + 2), 24);
        Region = new Region(path);
    }

    protected override void Dispose(bool disposing)
    {
        AppTheme.ThemeChanged -= (_, _) => { };
        base.Dispose(disposing);
    }

    /// <summary>Agrega un encabezado estándar al modal.</summary>
    public void SetHeader(string iconEmoji, string title, string? subtitle = null)
    {
        var header = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = subtitle is null ? 58 : 74,
            Padding = new Padding(0, 8, 40, 0),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        var titleLabel = new ThemeLabel { TextKind = TextKind.Title, Text = $"{iconEmoji}  {title}" };
        header.Controls.Add(titleLabel);

        if (subtitle is not null)
        {
            var sub = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = subtitle };
            header.Controls.Add(sub);
            sub.Margin = new Padding(0, -6, 0, 0);
        }

        _workspace.Controls.Add(header);
        header.BringToFront();
    }
}