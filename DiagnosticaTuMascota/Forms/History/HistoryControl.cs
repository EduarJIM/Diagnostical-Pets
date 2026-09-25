using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.History;

public partial class HistoryControl : UserControl
{
    private string _activeTab = "consultas";
    private Panel _contentPanel = null!;
    private RoundedButton _tabConsultas = null!;
    private RoundedButton _tabProximos = null!;
    private RoundedButton _tabAlertas = null!;

    public HistoryControl()
    {
        InitializeComponent();
        BuildUI();
    }

    private void InitializeComponent() { }

    private void BuildUI()
    {
        BackColor = Color.Transparent;
        AutoSize = true;
        DoubleBuffered = true;
        Padding = new Padding(48, 32, 48, 32);

        var content = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = Color.Transparent, MinimumSize = new Size(400, 0) };
        int y = 0;

        content.Controls.Add(new Label { Text = "\U0001F553  Historial y Seguimiento", Font = AppTheme.TitleH1, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(0, y) });
        y += 48;

        content.Controls.Add(new Label { Text = "Revisa y guarda diagnosticos, proximos procesos y alertas de salud.", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 38;

        _tabConsultas = CreateTabButton("consultas", "Consultas Anteriores", AppTheme.Primary);
        _tabConsultas.Location = new Point(0, y);
        _tabConsultas.Click += Tab_Click;
        content.Controls.Add(_tabConsultas);

        _tabProximos = CreateTabButton("proximos", "Proximos Procesos", AppTheme.Blue500);
        _tabProximos.Location = new Point(180, y);
        _tabProximos.Click += Tab_Click;
        content.Controls.Add(_tabProximos);

        _tabAlertas = CreateTabButton("alertas", "Alertas Activas (2)", AppTheme.Amber500);
        _tabAlertas.Location = new Point(360, y);
        _tabAlertas.Click += Tab_Click;
        content.Controls.Add(_tabAlertas);
        y += 52;

        _contentPanel = new Panel { Location = new Point(0, y), Size = new Size(600, 350), BackColor = Color.Transparent, AutoScroll = true, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        content.Controls.Add(_contentPanel);

        Controls.Add(content);
        LoadTab("consultas");
    }

    private RoundedButton CreateTabButton(string key, string text, Color activeColor)
    {
        return new RoundedButton { Text = text, Tag = (key, activeColor), CornerRadius = 18, ButtonColor1 = AppTheme.Muted, ButtonColor2 = AppTheme.Muted, UseGradient = false, TextColor = AppTheme.MutedForeground, Font = AppTheme.FontBold(11f), Height = 34, Width = TextRenderer.MeasureText(text, AppTheme.FontBold(11f)).Width + 28, Cursor = Cursors.Hand };
    }

    private void Tab_Click(object? sender, EventArgs e)
    {
        if (sender is RoundedButton btn && btn.Tag is (string key, Color _)) LoadTab(key);
    }

    private void LoadTab(string tab)
    {
        _activeTab = tab;
        _contentPanel.Controls.Clear();
        UpdateTabStyle(_tabConsultas, "consultas", AppTheme.Primary);
        UpdateTabStyle(_tabProximos, "proximos", AppTheme.Blue500);
        UpdateTabStyle(_tabAlertas, "alertas", AppTheme.Amber500);

        int y = 0;
        if (tab == "consultas") y = LoadConsultas(y);
        else if (tab == "proximos") y = LoadProximos(y);
        else if (tab == "alertas") y = LoadAlertas(y);
    }

    private void UpdateTabStyle(RoundedButton btn, string key, Color color)
    {
        bool isActive = _activeTab == key;
        btn.ButtonColor1 = isActive ? color : AppTheme.Muted;
        btn.ButtonColor2 = isActive ? color : AppTheme.Muted;
        btn.TextColor = isActive ? Color.White : AppTheme.MutedForeground;
        btn.Invalidate();
    }

    private int LoadConsultas(int y)
    {
        string[][] items = { new[] { "Max", "CRITICO", "15 Jul 2026", "Vomitos, Fiebre, Letargo", "Completada" }, new[] { "Luna", "MODERADO", "10 Jul 2026", "Tos persistente, Estornudos", "Completada" } };
        foreach (var item in items) { var c = CreateHistoryCard(item[0], item[1], item[2], item[3], item[4]); c.Location = new Point(0, y); _contentPanel.Controls.Add(c); y += 100; }
        return y;
    }

    private int LoadProximos(int y)
    {
        string[][] items = { new[] { "Max", "Vacunacion", "20 Jul 2026", "Segunda dosis de Parvovirus" }, new[] { "Luna", "Revisacion", "25 Jul 2026", "Control post-tratamiento" } };
        foreach (var item in items) { var c = CreateUpcomingCard(item[0], item[1], item[2], item[3]); c.Location = new Point(0, y); _contentPanel.Controls.Add(c); y += 90; }
        return y;
    }

    private int LoadAlertas(int y)
    {
        string[][] items = { new[] { "Alta", "Max", "15 Jul 2026 - 14:30", "Veterinario: Dr. Ramirez" }, new[] { "Media", "Luna", "10 Jul 2026 - 09:15", "Proxima vacuna pendiente" } };
        foreach (var item in items) { var c = CreateAlertCard(item[0], item[1], item[2], item[3]); c.Location = new Point(0, y); _contentPanel.Controls.Add(c); y += 88; }
        return y;
    }

    private RoundedPanel CreateHistoryCard(string pet, string severity, string date, string symptoms, string status)
    {
        var card = new RoundedPanel { Size = new Size(580, 88), BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 14 };
        card.Paint += (s, e) =>
        {
            Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Rectangle iconBounds = new(14, 14, 36, 36);
            AppTheme.DrawRoundedRectangle(g, iconBounds, 8, Color.FromArgb(25, AppTheme.Primary));
            using Font iconFont = new("Segoe UI Symbol", 16f);
            using SolidBrush iconBrush = new(AppTheme.Primary);
            g.DrawString("\U0001F4C4", iconFont, iconBrush, 19, 19);
            using Font nameFont = AppTheme.FontBold(14f);
            using SolidBrush nameBrush = new(AppTheme.Foreground);
            g.DrawString(pet, nameFont, nameBrush, 58, 14);
            Color sevColor = severity == "CRITICO" ? AppTheme.Critical : severity == "MODERADO" ? AppTheme.Moderate : AppTheme.Mild;
            SizeF bs = g.MeasureString(severity, AppTheme.BadgeFont);
            Rectangle bb = new(58 + (int)g.MeasureString(pet, nameFont).Width + 10, 16, (int)bs.Width + 14, 18);
            AppTheme.DrawRoundedRectangle(g, bb, 9, sevColor);
            using SolidBrush bt = new(Color.White);
            g.DrawString(severity, AppTheme.BadgeFont, bt, bb.X + 7, bb.Y + 2);
            using Font df = new("Segoe UI", 9.5f);
            using SolidBrush db = new(AppTheme.MutedForeground);
            g.DrawString($"\U0001F552 {date}", df, db, 58, 38);
            using Font sf = new("Segoe UI", 10f);
            g.DrawString(symptoms, sf, db, 14, 60);
            using SolidBrush sb = new(AppTheme.Primary);
            g.DrawString($"\u2714 {status}", new Font("Segoe UI", 9.5f), sb, 14, 74);
        };
        return card;
    }

    private RoundedPanel CreateUpcomingCard(string pet, string type, string date, string summary)
    {
        var card = new RoundedPanel { Size = new Size(580, 78), BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 14 };
        card.Paint += (s, e) =>
        {
            Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Rectangle iconBounds = new(14, 14, 36, 36);
            AppTheme.DrawRoundedRectangle(g, iconBounds, 8, Color.FromArgb(25, AppTheme.Blue500));
            using Font iconFont = new("Segoe UI Symbol", 16f);
            using SolidBrush iconBrush = new(AppTheme.Blue500);
            g.DrawString("\U0001F4C5", iconFont, iconBrush, 19, 19);
            using Font nf = AppTheme.FontBold(14f);
            using SolidBrush nb = new(AppTheme.Foreground);
            g.DrawString($"{pet} - {type}", nf, nb, 58, 14);
            using Font df = new("Segoe UI", 9.5f);
            using SolidBrush db = new(AppTheme.MutedForeground);
            g.DrawString($"\U0001F552 {date}", df, db, 58, 36);
            g.DrawString(summary, df, db, 14, 56);
        };
        return card;
    }

    private RoundedPanel CreateAlertCard(string severity, string pet, string date, string message)
    {
        Color sevColor = severity == "Alta" ? AppTheme.Critical : AppTheme.Moderate;
        Color bgColor = severity == "Alta" ? AppTheme.CriticalBg : AppTheme.ModerateBg;
        var card = new RoundedPanel { Size = new Size(580, 76), BackColor = bgColor, BorderColor = Color.FromArgb(60, sevColor), BorderWidth = 1, CornerRadius = 14 };
        card.Paint += (s, e) =>
        {
            Graphics g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Rectangle iconBounds = new(14, 14, 36, 36);
            AppTheme.DrawRoundedRectangle(g, iconBounds, 8, Color.FromArgb(40, sevColor));
            using Font iconFont = new("Segoe UI Symbol", 16f);
            using SolidBrush iconBrush = new(sevColor);
            g.DrawString("\u26A0", iconFont, iconBrush, 19, 19);
            using Font sf = AppTheme.FontBold(14f);
            using SolidBrush sb = new(sevColor);
            g.DrawString($"Alerta {severity}", sf, sb, 58, 14);
            using Font pf = new("Segoe UI", 10f);
            using SolidBrush pb = new(AppTheme.Foreground);
            g.DrawString($"Mascota: {pet}", pf, pb, 58, 36);
            using Font df = new("Segoe UI", 9.5f);
            using SolidBrush db = new(AppTheme.MutedForeground);
            g.DrawString($"\U0001F552 {date}", df, db, 58, 54);
            g.DrawString(message, df, db, 14, 54);
        };
        return card;
    }

    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); e.Graphics.Clear(AppTheme.Background); }
}
