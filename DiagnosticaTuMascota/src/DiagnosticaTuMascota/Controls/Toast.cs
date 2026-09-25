using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using Timer = System.Windows.Forms.Timer;

namespace DiagnosticaTuMascota.Controls;

public enum ToastKind { Success, Error, Info }

/// <summary>
/// Notificaciones tipo "toast" (sonner.tsx): tarjeta redondeada que aparece arriba
/// a la derecha, con icono de estado y auto-cierre.
/// </summary>
public static class ToastService
{
    private static int _openCount;

    public static void Show(Form? owner, string message, ToastKind kind = ToastKind.Info)
    {
        var toast = new ToastForm(owner, message, kind);
        toast.Show();
        if (owner is not null) toast.Owner = owner;
    }

    public static void Success(Form? owner, string message) => Show(owner, message, ToastKind.Success);
    public static void Error(Form? owner, string message) => Show(owner, message, ToastKind.Error);
    public static void Info(Form? owner, string message) => Show(owner, message, ToastKind.Info);

    private sealed class ToastForm : Form
    {
        private readonly string _message;
        private readonly ToastKind _kind;
        private readonly Timer _lifeTimer;
        private readonly Timer _fadeTimer;
        private int _slot;

        public ToastForm(Form? owner, string message, ToastKind kind)
        {
            _message = message;
            _kind = kind;
            _slot = _openCount++;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = AppTheme.Card;
            Size = new Size(360, 78);
            DoubleBuffered = true;

            var wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1024, 768);
            int top = wa.Top + 24 + _slot * (Height + 10);
            Location = new Point(wa.Right - Width - 24, top);

            _lifeTimer = new Timer { Interval = 3400 };
            _lifeTimer.Tick += (_, _) =>
            {
                _lifeTimer.Stop();
                _fadeTimer.Start();
            };
            _lifeTimer.Start();

            _fadeTimer = new Timer { Interval = 24 };
            _fadeTimer.Tick += (_, _) =>
            {
                Opacity -= 0.08;
                if (Opacity <= 0.05) { _fadeTimer.Stop(); Close(); }
            };

            FormClosed += (_, _) => { _openCount = Math.Max(0, _openCount - 1); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using var path = DrawingHelpers.RoundedRect(rect, 14);
            using (var b = new SolidBrush(AppTheme.Card)) g.FillPath(b, path);
            DrawingHelpers.StrokeRounded(g, AppTheme.Border, 1f, rect, 14);
            using (var shadow = new SolidBrush(Color.FromArgb(40, Color.Black)))
            {
                var shadowRect = new Rectangle(2, 4, Width, Height);
                using var spath = DrawingHelpers.RoundedRect(shadowRect, 14);
                g.FillPath(shadow, spath);
            }

            var accent = _kind switch
            {
                ToastKind.Success => AppTheme.SeverityLeve,
                ToastKind.Error => AppTheme.Destructive,
                _ => Color.FromArgb(59, 130, 246)
            };
            var icon = _kind switch
            {
                ToastKind.Success => "✅",
                ToastKind.Error => "⚠️",
                _ => "ℹ️"
            };

            var iconRect = new Rectangle(18, (Height - 36) / 2, 36, 36);
            using (var ipath = DrawingHelpers.RoundedRect(iconRect, 10))
            using (var ib = new SolidBrush(Color.FromArgb(24, accent)))
            {
                g.FillPath(ib, ipath);
            }
            using (var emoji = AppTheme.Emoji(11f))
            {
                DrawingHelpers.DrawEmoji(g, icon, emoji, iconRect, accent);
            }

            using var font = AppTheme.Medium(9.5f);
            var textRect = new Rectangle(64, 8, Width - 78, Height - 16);
            TextRenderer.DrawText(g, _message, font, textRect, AppTheme.Foreground,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }
    }
}