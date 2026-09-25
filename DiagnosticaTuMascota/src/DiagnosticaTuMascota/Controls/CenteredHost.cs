using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>
/// Anfitrión que centra su hijo (horizontal y verticalmente) y permite scroll.
/// Usado por las pantallas de autenticación para centrar la tarjeta.
/// </summary>
public class CenteredHost : Panel
{
    public CenteredHost()
    {
        AutoScroll = true;
        BackColor = AppTheme.Background;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        AppTheme.ThemeChanged += (_, _) => BackColor = AppTheme.Background;
    }

    public void CenterChild()
    {
        foreach (Control c in Controls)
        {
            c.Location = new Point(Math.Max(0, (ClientSize.Width - c.Width) / 2), Math.Max(0, (ClientSize.Height - c.Height) / 2));
        }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        e.Control.Anchor = AnchorStyles.None;
        CenterChild();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        CenterChild();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Brillos de fondo (degradados radiales suaves)
        var g = e.Graphics;
        using var b1 = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(0, 0, Width / 2, Height / 2),
            AppTheme.WithAlpha(AppTheme.Primary, 14), Color.Transparent, 45f);
        g.FillRectangle(b1, 0, 0, Width / 2, Height / 2);

        using var b2 = new System.Drawing.Drawing2D.LinearGradientBrush(
            new Rectangle(Width / 2, Height / 2, Width / 2, Height / 2),
            AppTheme.WithAlpha(AppTheme.Secondary, 12), Color.Transparent, 45f);
        g.FillRectangle(b2, Width / 2, Height / 2, Width / 2, Height / 2);
    }
}