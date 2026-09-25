using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.Dashboard;

public partial class DashboardControl : UserControl
{
    private Panel _heroPanel = null!;
    private System.Windows.Forms.Timer? _heroTimer;
    private float _floatY = 0f;
    private float _floatDirection = 0.5f;

    public DashboardControl()
    {
        InitializeComponent();
        BuildUI();
        StartHeroAnimation();
    }

    private void InitializeComponent() { }

    private void BuildUI()
    {
        BackColor = Color.Transparent;
        AutoSize = true;
        DoubleBuffered = true;
        Padding = new Padding(40, 30, 40, 30);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            AutoSize = true
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var leftPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            Padding = new Padding(0, 10, 20, 10)
        };

        int y = 0;

        leftPanel.Controls.Add(new Label { Text = "\u2728  Bienvenido", Font = AppTheme.TitleHero, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(0, y) });
        y += 54;

        leftPanel.Controls.Add(new Label { Text = "Sistema de Orientacion de Salud para Mascotas", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 50;

        var symptomCard = CreateFeatureCard("\U0001FA7A", "Analisis de Sintomas", "Evalua el estado de salud de tu mascota basado en sus sintomas",
            Color.FromArgb(25, AppTheme.Primary), Color.FromArgb(25, AppTheme.Secondary));
        symptomCard.Location = new Point(0, y);
        symptomCard.Size = new Size(450, 90);
        symptomCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        leftPanel.Controls.Add(symptomCard);
        y += 104;

        var gridPanel = new Panel { Location = new Point(0, y), Size = new Size(450, 90), BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        gridPanel.Resize += (s, e) =>
        {
            int halfW = (gridPanel.Width - 10) / 2;
            if (gridPanel.Controls.Count >= 2)
            {
                gridPanel.Controls[0].Size = new Size(halfW, 90);
                gridPanel.Controls[1].Location = new Point(halfW + 10, 0);
                gridPanel.Controls[1].Size = new Size(halfW, 90);
            }
        };

        var petsCard = CreateFeatureCard("\U0001F43E", "Mis Mascotas", "Gestiona el perfil clinico de tus mascotas",
            Color.FromArgb(25, AppTheme.Secondary), Color.FromArgb(25, AppTheme.Blue500));
        petsCard.Size = new Size(220, 90);
        gridPanel.Controls.Add(petsCard);

        var historyCard = CreateFeatureCard("\U0001F553", "Historial", "Revisa diagnosticos anteriores y seguimiento",
            Color.FromArgb(25, AppTheme.Purple500), Color.FromArgb(25, AppTheme.Indigo500));
        historyCard.Location = new Point(230, 0);
        historyCard.Size = new Size(220, 90);
        gridPanel.Controls.Add(historyCard);

        leftPanel.Controls.Add(gridPanel);
        y += 104;

        var disclaimer = new RoundedPanel
        {
            Location = new Point(0, y),
            Size = new Size(450, 70),
            BackColor = AppTheme.Card,
            BorderColor = Color.FromArgb(50, AppTheme.Destructive),
            BorderWidth = 1,
            CornerRadius = 16,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        disclaimer.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new(0, 0, disclaimer.Width, disclaimer.Height);
            AppTheme.DrawRoundedRectangle(e.Graphics, bounds, 16, Color.FromArgb(15, AppTheme.Destructive), Color.FromArgb(50, AppTheme.Destructive), 1);
            using Font iconFont = new("Segoe UI Symbol", 18f);
            using SolidBrush iconBrush = new(AppTheme.Destructive);
            e.Graphics.DrawString("\u26A0", iconFont, iconBrush, 14, 12);
            using Font titleFont = AppTheme.FontBold(13f);
            using SolidBrush titleBrush = new(AppTheme.Destructive);
            e.Graphics.DrawString("Aviso Medico Importante", titleFont, titleBrush, 48, 8);
            using Font textFont = new("Segoe UI", 10f);
            using SolidBrush textBrush = new(AppTheme.MutedForeground);
            e.Graphics.DrawString("Esta herramienta proporciona orientacion general y NO reemplaza el diagnostico veterinario profesional.", textFont, textBrush, new RectangleF(48, 30, disclaimer.Width - 68, 36));
        };
        leftPanel.Controls.Add(disclaimer);

        _heroPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            MinimumSize = new Size(200, 300)
        };
        _heroPanel.Paint += HeroPaint;

        mainLayout.Controls.Add(leftPanel, 0, 0);
        mainLayout.Controls.Add(_heroPanel, 1, 0);

        Controls.Add(mainLayout);
    }

    private GradientPanel CreateFeatureCard(string icon, string title, string description, Color color1, Color color2)
    {
        var card = new GradientPanel
        {
            CornerRadius = 20,
            GradientColor1 = color1,
            GradientColor2 = color2,
            GradientMode = LinearGradientMode.ForwardDiagonal,
            Cursor = Cursors.Hand
        };

        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle iconBounds = new(14, 14, 38, 38);
            AppTheme.DrawRoundedRectangle(e.Graphics, iconBounds, 10, AppTheme.Card);
            using Font iconFont = new("Segoe UI Symbol", 16f);
            using SolidBrush iconBrush = new(AppTheme.Foreground);
            SizeF iconSize = e.Graphics.MeasureString(icon, iconFont);
            e.Graphics.DrawString(icon, iconFont, iconBrush, iconBounds.X + (38 - iconSize.Width) / 2, iconBounds.Y + (38 - iconSize.Height) / 2);

            using Font titleFont = AppTheme.FontBold(13f);
            using SolidBrush titleBrush = new(AppTheme.Foreground);
            e.Graphics.DrawString(title, titleFont, titleBrush, 60, 14);

            using Font descFont = new("Segoe UI", 9.5f);
            using SolidBrush descBrush = new(AppTheme.MutedForeground);
            e.Graphics.DrawString(description, descFont, descBrush, new RectangleF(60, 36, card.Width - 76, 40));
        };

        return card;
    }

    private void HeroPaint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Panel panel = (Panel)sender!;
        int centerX = panel.Width / 2;
        int centerY = panel.Height / 2;
        int floatOffset = (int)_floatY;
        int circleSize = Math.Min(220, Math.Min(panel.Width - 60, panel.Height - 60));
        int circleX = centerX - circleSize / 2;
        int circleY = centerY - circleSize / 2 + floatOffset;

        for (int i = 30; i > 0; i -= 3)
        {
            int alpha = (int)(20 * ((float)i / 30));
            Rectangle glowBounds = new(circleX - i * 3, circleY - i * 3, circleSize + i * 6, circleSize + i * 6);
            using GraphicsPath glowPath = AppTheme.GetRoundedRectPath(glowBounds, glowBounds.Height / 2);
            using SolidBrush glowBrush = new(Color.FromArgb(alpha, AppTheme.Primary));
            g.FillPath(glowBrush, glowPath);
        }

        Rectangle circleBounds = new(circleX, circleY, circleSize, circleSize);
        using GraphicsPath circlePath = AppTheme.GetRoundedRectPath(circleBounds, circleSize / 2);
        using LinearGradientBrush mainBrush = new(circleBounds, AppTheme.Primary, AppTheme.Blue500, LinearGradientMode.ForwardDiagonal);
        g.FillPath(mainBrush, circlePath);

        Rectangle shadowBounds = new(circleX + 20, circleY + circleSize - 10, circleSize - 40, 24);
        using GraphicsPath shadowPath = AppTheme.GetRoundedRectPath(shadowBounds, 12);
        using SolidBrush shadowBrush = new(Color.FromArgb(25, AppTheme.Primary));
        g.FillPath(shadowBrush, shadowPath);

        using Font iconFont = new("Segoe UI Symbol", Math.Max(20, circleSize * 0.32f));
        using SolidBrush iconBrush = new(Color.FromArgb(220, Color.White));
        SizeF iconSize = g.MeasureString("\U0001FA7A", iconFont);
        g.DrawString("\U0001FA7A", iconFont, iconBrush, circleX + (circleSize - iconSize.Width) / 2, circleY + (circleSize - iconSize.Height) / 2);

        double angle = DateTime.Now.TimeOfDay.TotalSeconds * (2 * Math.PI / 20);
        int orbitRadius = circleSize / 2 + 40;
        int sparkX = centerX + (int)(Math.Cos(angle) * orbitRadius) - 10;
        int sparkY = centerY + (int)(Math.Sin(angle) * orbitRadius) - 10 + floatOffset;
        using Font sparkFont = new("Segoe UI Symbol", 18f);
        using SolidBrush sparkBrush = new(Color.FromArgb(180, AppTheme.Secondary));
        g.DrawString("\u2728", sparkFont, sparkBrush, sparkX, sparkY);
    }

    private void StartHeroAnimation()
    {
        _heroTimer = new System.Windows.Forms.Timer { Interval = 30 };
        _heroTimer.Tick += (s, e) =>
        {
            _floatY += _floatDirection;
            if (_floatY >= 10f) _floatDirection = -0.4f;
            if (_floatY <= -10f) _floatDirection = 0.4f;
            _heroPanel?.Invalidate();
        };
        _heroTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(AppTheme.Background);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _heroTimer?.Stop(); _heroTimer?.Dispose(); }
        base.Dispose(disposing);
    }
}
