using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public class GlowCircle : Control
{
    public Color CircleColor1 { get; set; } = AppTheme.Primary;
    public Color CircleColor2 { get; set; } = AppTheme.Secondary;
    public Color CircleColor3 { get; set; } = AppTheme.Blue500;
    public int GlowIntensity { get; set; } = 30;
    public bool ShowPulse { get; set; } = true;

    private float _pulseScale = 1f;
    private float _pulseDirection = 0.01f;
    private System.Windows.Forms.Timer? _pulseTimer;

    public GlowCircle()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Size = new Size(256, 256);

        if (DesignMode)
            return;

        _pulseTimer = new System.Windows.Forms.Timer { Interval = 30 };
        _pulseTimer.Tick += (s, e) =>
        {
            _pulseScale += _pulseDirection;
            if (_pulseScale >= 1.05f) _pulseDirection = -0.005f;
            if (_pulseScale <= 0.95f) _pulseDirection = 0.005f;
            Invalidate();
        };
        _pulseTimer.Start();
    }

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int w = (int)(Width * _pulseScale);
        int h = (int)(Height * _pulseScale);
        int x = (Width - w) / 2;
        int y = (Height - h) / 2;
        Rectangle bounds = new(x, y, w, h);

        // Glow layers
        for (int i = GlowIntensity; i > 0; i -= 3)
        {
            int alpha = (int)(30 * ((float)i / GlowIntensity));
            Rectangle glowBounds = bounds;
            glowBounds.Inflate(i * 2, i * 2);
            using GraphicsPath glowPath = AppTheme.GetRoundedRectPath(glowBounds, glowBounds.Height / 2);
            using SolidBrush glowBrush = new(Color.FromArgb(alpha, CircleColor1));
            g.FillPath(glowBrush, glowPath);
        }

        // Main circle with gradient
        using GraphicsPath circlePath = AppTheme.GetRoundedRectPath(bounds, bounds.Height / 2);
        using LinearGradientBrush mainBrush = new(bounds, CircleColor1, CircleColor3, LinearGradientMode.ForwardDiagonal);
        g.FillPath(mainBrush, circlePath);

        // Stethoscope icon
        Rectangle iconBounds = new(x + w / 4, y + h / 4, w / 2, h / 2);
        using Font iconFont = new("Segoe UI Symbol", Math.Max(24, w * 0.35f));
        using SolidBrush iconBrush = new(Color.FromArgb(200, Color.White));
        string icon = "\U0001FA7A";
        SizeF iconSize = g.MeasureString(icon, iconFont);
        g.DrawString(icon, iconFont, iconBrush,
            x + (w - iconSize.Width) / 2,
            y + (h - iconSize.Height) / 2);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pulseTimer?.Stop();
            _pulseTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
