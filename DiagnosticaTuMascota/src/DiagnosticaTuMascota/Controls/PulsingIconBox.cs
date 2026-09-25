using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using Timer = System.Windows.Forms.Timer;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Caja de icono con degradado primary→secondary y un anillo que pulsa,
/// replicando los bloques animados (logo, hero) de la web.
/// </summary>
public class PulsingIconBox : Control
{
    private readonly Timer _timer;
    private float _phase;

    public PulsingIconBox()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // sin fondo gris opaco detrás del halo
        _timer = new Timer { Interval = 40 };
        _timer.Tick += (_, _) => { _phase += 0.06f; Invalidate(); };
        _timer.Start();
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize de TableLayoutPanel).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    /// <summary>Texto emoji dibujado en el centro.</summary>
    public string? IconText { get; set; } = "🩺";
    public float IconFontSize { get; set; } = 16f;
    /// <summary>Radio de esquina: 0 = círculo perfecto.</summary>
    public int CornerRadius { get; set; } = 14;
    public bool Pulsing { get; set; } = true;
    public Color StartColor { get; set; } = AppTheme.Primary;
    public Color EndColor { get; set; } = AppTheme.Secondary;
    /// <summary>Color del halo pulsante (usado para glow).</summary>
    public Color GlowColor { get; set; } = AppTheme.Primary;

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (_phase > 2 * Math.PI) _phase = 0;
        float pulse = 0.5f + 0.5f * (float)Math.Sin(_phase);

        if (Pulsing)
        {
            // Halo pulsante (alpha elevado para mejor contraste sobre el fondo)
            int grow = 3 + (int)(pulse * 5);
            var halo = new Rectangle(-grow, -grow, Width + grow * 2, Height + grow * 2);
            int alpha = 26 + (int)(pulse * 50);
            using (var path = DrawingHelpers.RoundedRect(halo, CornerRadius))
            using (var b = new SolidBrush(Color.FromArgb(alpha, GlowColor)))
            {
                g.FillPath(b, path);
            }
        }

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = DrawingHelpers.RoundedRect(rect, CornerRadius))
        using (var brush = new LinearGradientBrush(rect, StartColor, EndColor, 45f))
        {
            g.FillPath(brush, path);
        }

        if (!string.IsNullOrEmpty(IconText))
        {
            using var emoji = AppTheme.Emoji(IconFontSize);
            DrawingHelpers.DrawEmoji(g, IconText, emoji, rect, Color.White);
        }
    }
}