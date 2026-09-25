using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Theme;
using Timer = System.Windows.Forms.Timer;

namespace DiagnosticaTuMascota.Pages;

public sealed class DashboardPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PageBody body = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel grid = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel left = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel welcomeRow = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel sparkles = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel welcome = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel sub = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FeatureCard fc1 = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel row2 = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FeatureCard fc2 = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FeatureCard fc3 = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private MedicalDisclaimer disclaimer = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private HeroArt hero = null!;
    private readonly INavigator _nav;

    public DashboardPage(INavigator nav)
    {
        _nav = nav;
        body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 1200 };
        Controls.Add(body);

        grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));

        // ---- Columna izquierda ----
        left = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(8, 0, 36, 0)
        };

        welcomeRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent };
        sparkles = new ThemeLabel { TextKind = TextKind.Body, Text = "✨", Font = AppTheme.Emoji(14f) };
        welcome = new ThemeLabel { TextKind = TextKind.Display, Text = "  Bienvenido" };
        welcomeRow.Controls.Add(sparkles);
        welcomeRow.Controls.Add(welcome);
        left.Controls.Add(welcomeRow);

        sub = new ThemeLabel { TextKind = TextKind.Body, Text = "Sistema de Orientación de Salud para Mascotas" };
        sub.Margin = new Padding(2, 0, 0, 22);
        left.Controls.Add(sub);

        fc1 = new FeatureCard
        {
            IconText = "🩺",
            CardTitle = "Análisis de Síntomas",
            CardDescription = "Evalúa los síntomas de tu mascota y recibe orientación inmediata",
            TintStart = AppTheme.WithAlpha(AppTheme.Primary, 26),
            TintEnd = AppTheme.WithAlpha(AppTheme.Secondary, 26),
            Width = 640,
            Height = 150
        };
        fc1.Click += (_, _) => _nav.Navigate(AppPage.SymptomAnalysis);
        left.Controls.Add(fc1);

        row2 = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent, Width = 640 };
        fc2 = new FeatureCard
        {
            IconText = "🐾",
            CardTitle = "Mis Mascotas",
            CardDescription = "Gestiona los perfiles de tus mascotas",
            TintStart = AppTheme.WithAlpha(AppTheme.Secondary, 26),
            TintEnd = AppTheme.WithAlpha(Color.FromArgb(59, 130, 246), 26),
            Width = 314,
            Height = 150,
            Margin = new Padding(0, 0, 12, 0)
        };
        fc2.Click += (_, _) => _nav.Navigate(AppPage.Pets);
        row2.Controls.Add(fc2);

        fc3 = new FeatureCard
        {
            IconText = "📜",
            CardTitle = "Historial",
            CardDescription = "Revisa consultas anteriores",
            TintStart = AppTheme.WithAlpha(Color.FromArgb(139, 92, 246), 26),
            TintEnd = AppTheme.WithAlpha(Color.FromArgb(99, 102, 241), 26),
            Width = 314,
            Height = 150
        };
        fc3.Click += (_, _) => _nav.Navigate(AppPage.History);
        row2.Controls.Add(fc3);
        left.Controls.Add(row2);

        disclaimer = new MedicalDisclaimer { Width = 640 };
        disclaimer.Margin = new Padding(0, 12, 0, 0);
        left.Controls.Add(disclaimer);

        grid.Controls.Add(left, 0, 0);

        // ---- Columna derecha ----
        hero = new HeroArt { Dock = DockStyle.Fill };
        grid.Controls.Add(hero, 1, 0);

        body.Controls.Add(grid);
    }
}

/// <summary>Banner de aviso médico (DisclaimerBanner.tsx).</summary>
public sealed class MedicalDisclaimer : RoundedPanel
{
    public MedicalDisclaimer()
    {
        CornerRadius = 14;
        Height = 118;
        FillColor = AppTheme.WithAlpha(AppTheme.Destructive, 12);
        BorderColor = AppTheme.WithAlpha(AppTheme.Destructive, 60);
        Padding = new Padding(20, 16, 20, 16);
        AppTheme.ThemeChanged += (_, _) =>
        {
            FillColor = AppTheme.WithAlpha(AppTheme.Destructive, 12);
            BorderColor = AppTheme.WithAlpha(AppTheme.Destructive, 60);
            Invalidate();
        };
        Build("Aviso Médico Importante",
            "Esta herramienta proporciona orientación general y NO reemplaza el diagnóstico veterinario profesional. " +
            "Siempre consulte con un veterinario certificado para obtener un diagnóstico preciso y tratamiento adecuado para su mascota.");
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        LayoutChildren();
    }

    protected override void OnControlAdded(ControlEventArgs e) { base.OnControlAdded(e); LayoutChildren(); }

    private void LayoutChildren()
    {
        if (Controls.Count == 0) return;
        var box = Controls[0];
        var text = Controls[1];
        box.Location = new Point(20, (Height - box.Height) / 2);
        text.Location = new Point(box.Right + 14, 18);
        text.Width = Width - box.Right - 34;
    }

    public void Build(string title, string body)
    {
        Controls.Clear();
        var iconBox = new RoundedPanel
        {
            FillColor = AppTheme.WithAlpha(AppTheme.Destructive, 22),
            CornerRadius = 10,
            Size = new Size(40, 40)
        };
        var icon = new ThemeLabel { TextKind = TextKind.Body, Text = "⚠️", Font = AppTheme.Emoji(11f), Anchor = AnchorStyles.None, AutoSize = true };
        iconBox.Controls.Add(icon);
        icon.Location = new Point((40 - icon.Width) / 2, (40 - icon.Height) / 2);

        var text = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = false,
            Height = 84
        };
        var titleLabel = new ThemeLabel { TextKind = TextKind.SubHeading, Text = title };
        var bodyLabel = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            MaximumSize = new Size(1200, 60),
            AutoEllipsis = true,
            Text = body
        };
        text.Controls.Add(titleLabel);
        text.Controls.Add(bodyLabel);

        Controls.Add(iconBox);
        Controls.Add(text);
        LayoutChildren();
    }
}

/// <summary>Panel visual animado del dashboard (anillos, globo flotante, destellos).</summary>
public sealed class HeroArt : Control
{
    private readonly Timer _timer;
    private double _phase;

    public HeroArt()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        _timer = new Timer { Interval = 50 };
        _timer.Tick += (_, _) => { _phase += 0.05; Invalidate(); };
        _timer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Fondo con degradado sutil hacia primary
        using (var bg = new LinearGradientBrush(ClientRectangle, Color.Transparent, AppTheme.WithAlpha(AppTheme.Primary, 20), 90f))
        {
            g.FillRectangle(bg, ClientRectangle);
        }
        // Línea divisoria izquierda
        using (var pen = new Pen(AppTheme.Border, 1f))
        {
            g.DrawLine(pen, 0, 0, 0, Height);
        }

        int cx = Width / 2, cy = Height / 2;

        // Anillos pulsantes
        double pulse = (Math.Sin(_phase) + 1) / 2;
        int ringOuter = (int)(Math.Min(Width, Height) * 0.72);
        int r1 = (int)(ringOuter * (0.4 + pulse * 0.12));
        int r2 = (int)(ringOuter * 0.52);
        DrawRing(g, cx, cy, r2, AppTheme.Secondary, 46);
        DrawRing(g, cx, cy, r1, AppTheme.Primary, 26);

        // Globo flotante
        float bob = (float)Math.Sin(_phase * 0.6) * 12;
        int blobSize = (int)(Math.Min(Width, Height) * 0.46);
        var blobRect = new Rectangle(cx - blobSize / 2, cy - (int)bob - blobSize / 2, blobSize, blobSize);
        using (var blobPath = new System.Drawing.Drawing2D.GraphicsPath())
        {
            blobPath.AddEllipse(blobRect);
            using var brush = new LinearGradientBrush(blobRect, AppTheme.Primary, Color.FromArgb(59, 130, 246), 45f);
            g.FillPath(brush, blobPath);
        }
        // Borde interior
        var innerSize = blobSize - (int)(blobSize * 0.18);
        var innerRect = new Rectangle(cx - innerSize / 2, cy - (int)bob - innerSize / 2, innerSize, innerSize);
        using (var innerPath = new System.Drawing.Drawing2D.GraphicsPath())
        {
            innerPath.AddEllipse(innerRect);
            using var brush = new SolidBrush(Color.FromArgb(60, AppTheme.Background));
            g.FillPath(brush, innerPath);
        }
        using (var emoji = AppTheme.Emoji(30f))
        {
            DrawingHelpers.DrawEmoji(g, "🩺", emoji, innerRect);
        }

        // Destellos orbitales
        double orbit = _phase * 1.4;
        int orbitR = blobSize / 2 + 26;
        int ox = cx + (int)(Math.Cos(orbit) * orbitR);
        int oy = cy - (int)bob + (int)(Math.Sin(orbit) * orbitR);
        using (var emoji = AppTheme.Emoji(9f))
        {
            DrawingHelpers.DrawEmoji(g, "✨", emoji, new Rectangle(ox - 14, oy - 14, 28, 28), AppTheme.Primary);
        }

        // Manchas de color difuminadas
        using (var b1 = new SolidBrush(AppTheme.WithAlpha(AppTheme.Primary, 20)))
        {
            g.FillEllipse(b1, 30, 40, 90, 90);
        }
        using (var b2 = new SolidBrush(AppTheme.WithAlpha(AppTheme.Secondary, 18)))
        {
            g.FillEllipse(b2, Width - 130, Height - 170, 110, 110);
        }
    }

    private void DrawRing(Graphics g, int cx, int cy, int radius, Color color, int alpha)
    {
        var rect = new Rectangle(cx - radius, cy - radius, radius * 2, radius * 2);
        using var pen = new Pen(Color.FromArgb(alpha, color), 2f);
        g.DrawEllipse(pen, rect);
    }
}