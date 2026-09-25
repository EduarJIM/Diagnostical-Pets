using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using Timer = System.Windows.Forms.Timer;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Interruptor de tema (sol/luna) idéntico al ThemeToggle.tsx de la web.
/// Anima el perilla y muestra ambos iconos tenues sobre la pista.
/// </summary>
public class ToggleSwitch : UserControl
{
    private bool _checked; // true = tema oscuro
    private float _knobX;  // posición animada del perilla
    private readonly Timer _timer;

    public ToggleSwitch()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        Size = new Size(56, 28);
        BackColor = Color.Transparent; // sin caja gris detrás del interruptor
        Cursor = Cursors.Hand;
        AppTheme.ThemeChanged += (_, _) => Invalidate();

        _timer = new Timer { Interval = 16 };
        _timer.Tick += (_, _) =>
        {
            float target = _checked ? 30 : 2;
            _knobX += (target - _knobX) * 0.25f;
            if (Math.Abs(target - _knobX) < 0.3f) { _knobX = target; _timer.Stop(); }
            Invalidate();
        };
    }

    public bool Checked
    {
        get => _checked;
        set { _checked = value; _knobX = value ? 30 : 2; Invalidate(); }
    }

    public event EventHandler? CheckedChanged;

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        _checked = !_checked;
        CheckedChanged?.Invoke(this, EventArgs.Empty);
        _timer.Start();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var track = new Rectangle(0, 0, Width - 1, Height - 1);
        var trackColor = _checked ? Color.FromArgb(30, 41, 59) : Color.FromArgb(254, 243, 199); // slate-800 / amber-100
        using (var path = DrawingHelpers.RoundedRect(track, 14))
        using (var b = new SolidBrush(trackColor))
        {
            g.FillPath(b, path);
        }
        DrawingHelpers.StrokeRounded(g, AppTheme.Border, 1f, track, 14);

        // Iconos tenues sobre la pista
        var sunRect = new Rectangle(4, 0, 14, Height);
        var moonRect = new Rectangle(Width - 18, 0, 14, Height);
        using (var emoji = AppTheme.Emoji(8f))
        {
            TextRenderer.DrawText(g, "☀", emoji, sunRect, Color.FromArgb(120, 217, 119, 6), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, "☾", emoji, moonRect, Color.FromArgb(120, 148, 163, 184), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // Perilla
        var knobSize = 20;
        var knobRect = new Rectangle((int)_knobX, (Height - knobSize) / 2, knobSize, knobSize);
        var knobColor = _checked ? Color.FromArgb(226, 232, 240) : Color.FromArgb(251, 191, 36); // slate-200 / amber-400
        using (var path = DrawingHelpers.RoundedRect(knobRect, knobSize / 2))
        using (var b = new SolidBrush(knobColor))
        {
            g.FillPath(b, path);
        }
        var knobIcon = _checked ? "☾" : "☀";
        using (var emoji = AppTheme.Emoji(9f))
        {
            TextRenderer.DrawText(g, knobIcon, emoji, knobRect,
                _checked ? Color.FromArgb(15, 23, 42) : Color.FromArgb(120, 53, 15),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}