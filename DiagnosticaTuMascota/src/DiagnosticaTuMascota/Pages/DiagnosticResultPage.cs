using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;
using Timer = System.Windows.Forms.Timer;

namespace DiagnosticaTuMascota.Pages;

/// <summary>
/// Resultado del triaje (DiagnosticResult.tsx): hero de severidad, implicaciones
/// clínicas, acciones recomendadas y guardado automático en el historial.
/// </summary>
public sealed partial class DiagnosticResultPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PageBody body = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel root = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private Panel topBar = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel back = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PillButton resultPill = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel headerRow = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel headerIcon = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel sectionRow = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel implicationsCard = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel implHeader = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel actionsCard = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel actionsHeader = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel extraHeader = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private MedicalDisclaimer legal = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel actions = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton finishBtn = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton historyBtn = null!;
    private INavigator _nav = null!;
    private SeverityHero _hero = null!;
    private FlowLayoutPanel _implicationsList = null!;
    private FlowLayoutPanel _actionsList = null!;
    private RoundedPanel _extraCard = null!;
    private ThemeLabel _extraText = null!;
    private ThemeLabel _generatedLabel = null!;
    private bool _saved;
    private NavState? _pending;

    public DiagnosticResultPage(INavigator nav)
    {
        _nav = nav;
        InitializeComponent();
    }

    /// <summary>
    /// Constructor solo para el Diseñador de Visual Studio.
    /// Usa un navegador y un almacenamiento en memoria, así la página
    /// se ve en el diseño exactamente igual que en ejecución.
    /// </summary>
    public DiagnosticResultPage() : this(DesignTimeNavigator.Instance)
    {
        Size = DesignTime.PageCanvas;
        DesignTime.PrimeLayout(this);
    }

    private void InitializeComponent()
    {
        body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 980 };
        Controls.Add(body);

        root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            AutoSize = true,
            Width = 940
        };

        topBar = new Panel { Height = 46, Width = 940, BackColor = Color.Transparent };
        back = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "←  Volver al Inicio", Cursor = Cursors.Hand, Anchor = AnchorStyles.Left };
        back.Click += (_, _) => _nav.Navigate(AppPage.Dashboard);
        resultPill = new PillButton { Text = "Resultado del Análisis", IconText = "🔬", Active = true, Width = 170, Height = 34, ActiveColor = AppTheme.SeverityLeve };
        topBar.Controls.Add(back);
        topBar.Controls.Add(resultPill);
        topBar.Resize += (_, _) =>
        {
            back.Location = new Point(4, 12);
            resultPill.Location = new Point(topBar.Width - resultPill.Width - 4, 6);
        };
        root.Controls.Add(topBar);

        headerRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent };
        headerIcon = new ThemeLabel { TextKind = TextKind.Body, Text = "🩺", Font = AppTheme.Emoji(15f) };
        title = new ThemeLabel { TextKind = TextKind.Display, Text = "  Análisis Clínico" };
        headerRow.Controls.Add(headerIcon);
        headerRow.Controls.Add(title);
        root.Controls.Add(headerRow);

        _generatedLabel = new ThemeLabel { TextKind = TextKind.Body, Text = "", MaximumSize = new Size(900, 40), AutoEllipsis = true, Margin = new Padding(2, 0, 0, 16) };
        root.Controls.Add(_generatedLabel);

        _hero = new SeverityHero { Width = 940, Height = 190 };
        root.Controls.Add(_hero);

        sectionRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = true, AutoSize = true, BackColor = Color.Transparent, Width = 940 };

        implicationsCard = new RoundedPanel
        {
            FillColor = AppTheme.Card, CornerRadius = 16, BorderColor = AppTheme.Border,
            Width = 455, Height = 250, Padding = new Padding(22, 16, 22, 16), Margin = new Padding(0, 20, 18, 8)
        };
        implHeader = new ThemeLabel { TextKind = TextKind.Heading, Text = "⚠️  Implicaciones Clínicas", AutoSize = true };
        _implicationsList = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, BackColor = Color.Transparent,
            Location = new Point(22, 44), Width = 410
        };
        implicationsCard.Controls.Add(implHeader);
        implicationsCard.Controls.Add(_implicationsList);
        sectionRow.Controls.Add(implicationsCard);

        actionsCard = new RoundedPanel
        {
            FillColor = AppTheme.Card, CornerRadius = 16, BorderColor = AppTheme.Border,
            Width = 455, Height = 250, Padding = new Padding(22, 16, 22, 16), Margin = new Padding(0, 20, 0, 8)
        };
        actionsHeader = new ThemeLabel { TextKind = TextKind.Heading, Text = "🩹  Primeros Auxilios / Acciones", AutoSize = true };
        _actionsList = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, BackColor = Color.Transparent,
            Location = new Point(22, 44), Width = 410
        };
        actionsCard.Controls.Add(actionsHeader);
        actionsCard.Controls.Add(_actionsList);
        sectionRow.Controls.Add(actionsCard);

        root.Controls.Add(sectionRow);

        _extraCard = new RoundedPanel
        {
            FillColor = AppTheme.WithAlpha(AppTheme.Secondary, 10),
            CornerRadius = 16,
            BorderColor = AppTheme.WithAlpha(AppTheme.Secondary, 60),
            Width = 940,
            Height = 120,
            Padding = new Padding(22, 16, 22, 16),
            Margin = new Padding(0, 8, 0, 8),
            Visible = false
        };
        extraHeader = new ThemeLabel { TextKind = TextKind.SubHeading, Text = "📝  Información Adicional", AutoSize = true };
        _extraText = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            MaximumSize = new Size(880, 80),
            AutoEllipsis = true,
            Location = new Point(22, 46)
        };
        _extraCard.Controls.Add(extraHeader);
        _extraCard.Controls.Add(_extraText);
        root.Controls.Add(_extraCard);

        legal = new MedicalDisclaimer { Width = 940, Height = 118 };
        root.Controls.Add(legal);

        actions = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            BackColor = Color.Transparent,
            Width = 940,
            Padding = new Padding(0, 4, 0, 20)
        };
        finishBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Finalizar y Volver al Inicio", Icon = "🏠", IconSize = 9f,
            Size = new Size(240, 44), Font = AppTheme.Medium(9.5f), Margin = new Padding(10, 0, 0, 0)
        };
        finishBtn.Click += (_, _) => _nav.Navigate(AppPage.Dashboard);
        historyBtn = new RoundedButton
        {
            Variant = ButtonVariant.Outline, Text = "Ver Historial", Icon = "📜", IconSize = 9f,
            Size = new Size(150, 44), Font = AppTheme.Medium(9.5f), Margin = new Padding(0, 0, 10, 0)
        };
        historyBtn.Click += (_, _) => _nav.Navigate(AppPage.History);
        actions.Controls.Add(finishBtn);
        actions.Controls.Add(historyBtn);
        root.Controls.Add(actions);

        body.Controls.Add(root);
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        _pending = state;
        _saved = false;

        if (state?.SelectedSymptoms is not { Count: > 0 })
        {
            _nav.Navigate(AppPage.Dashboard);
            return;
        }

        var petName = string.IsNullOrWhiteSpace(state.PetName) ? "Tu Mascota" : state.PetName;
        var triage = TriageEngine.Evaluate(state.SelectedSymptoms);

        _generatedLabel.Text = $"Resultado para {petName} — generado el {DateTime.Now.ToString("dd 'de' MMMM, yyyy", CultureInfo.GetCultureInfo("es-ES"))}.";

        _hero.SetLevel(triage.OverallSeverity, petName);

        // Implicaciones
        _implicationsList.Controls.Clear();
        if (triage.Implications.Count == 0)
        {
            _implicationsList.Controls.Add(new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "No se registraron implicaciones para los síntomas seleccionados." });
        }
        else
        {
            foreach (var imp in triage.Implications)
            {
                var label = new ThemeLabel
                {
                    TextKind = TextKind.MutedSmall,
                    Text = imp,
                    MaximumSize = new Size(400, 60),
                    AutoEllipsis = true,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 2, 0, 4)
                };
                _implicationsList.Controls.Add(label);
            }
        }

        // Acciones recomendadas por severidad
        _actionsList.Controls.Clear();
        foreach (var action in RecommendedActions(triage.OverallSeverity))
        {
            _actionsList.Controls.Add(new ThemeLabel
            {
                TextKind = TextKind.MutedSmall,
                Text = "•  " + action,
                MaximumSize = new Size(400, 60),
                AutoEllipsis = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 2, 0, 4)
            });
        }

        // Información adicional
        bool hasExtra = !string.IsNullOrWhiteSpace(state.AdditionalInfo);
        _extraCard.Visible = hasExtra;
        if (hasExtra) _extraText.Text = state.AdditionalInfo;

        SaveToHistoryOnce(petName, state.SelectedSymptoms, state.AdditionalInfo);
    }

    private static string[] RecommendedActions(SeverityLevel level) => level switch
    {
        SeverityLevel.CRITICO => new[]
        {
            "Acude de inmediato a la clínica veterinaria de emergencia.",
            "No administres medicamentos por tu cuenta.",
            "Mantén a la mascota quieta, abrigada y a temperatura estable.",
            "Evita alimentos y agua hasta recibir indicaciones."
        },
        SeverityLevel.MODERADO => new[]
        {
            "Consulta a tu veterinario dentro de las próximas 24 horas.",
            "Lleva un registro de los síntomas y su evolución.",
            "Mantén la hidratación y evita ejercicio intenso."
        },
        _ => new[]
        {
            "Observa a tu mascota durante 24-48 horas.",
            "Monitorea apetito, hidratación y nivel de energía.",
            "Si los síntomas empeoran, agenda una consulta veterinaria."
        }
    };

    private void SaveToHistoryOnce(string petName, List<string> symptoms, string additionalInfo)
    {
        if (_saved) return;
        _saved = true;

        var triage = TriageEngine.Evaluate(symptoms);
        var historyData = _nav.Storage.GetData("historyData", DemoData.CreateInitialHistory());

        var item = new HistoryItem
        {
            Id = DateTime.UtcNow.Ticks,
            Date = DateTime.Now.ToString("dd 'de' MMMM, yyyy", CultureInfo.GetCultureInfo("es-ES")),
            Pet = petName,
            Status = "Completado",
            Result = triage.OverallSeverity.ToLabel(),
            Summary = "Síntomas: " + string.Join(", ", symptoms)
                     + (string.IsNullOrWhiteSpace(additionalInfo) ? "" : $". Contexto: {additionalInfo}"),
            Recommendation = TriageEngine.GetBaseRecommendation(triage.OverallSeverity)
        };
        historyData.Insert(0, item);
        _nav.Storage.SetData("historyData", historyData);
    }
}

/// <summary>
/// Hero del resultado: gran círculo del color de severidad con icono pulsante,
/// etiqueta de nivel de urgencia, título y caja de recomendación.
/// </summary>
public sealed class SeverityHero : UserControl
{
    private readonly Timer _timer;
    private double _phase;
    private SeverityLevel _level = SeverityLevel.LEVE;
    private string _petName = "Tu Mascota";

    public SeverityHero()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // el fondo es traslúcido: requiere el fondo del padre
        AppTheme.ThemeChanged += (_, _) => Invalidate();
        _timer = new Timer { Interval = 45 };
        _timer.Tick += (_, _) => { _phase += 0.06; Invalidate(); };
        _timer.Start();
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }

    public void SetLevel(SeverityLevel level, string petName)
    {
        _level = level;
        _petName = petName;
        Invalidate();
    }

    private Color SeverityColor() => AppTheme.SeverityFor(_level);
    private string Icon() => _level switch
    {
        SeverityLevel.CRITICO => "🚨",
        SeverityLevel.MODERADO => "🌡️",
        _ => "✅"
    };
    private string Title() => _level switch
    {
        SeverityLevel.CRITICO => "Caso Crítico / Emergencia",
        SeverityLevel.MODERADO => "Caso Moderado",
        _ => "Caso Leve"
    };
    private string Description() => _level switch
    {
        SeverityLevel.CRITICO => "Los síntomas de " + _petName + " indican una posible emergencia médica. Se necesita atención veterinaria inmediata.",
        SeverityLevel.MODERADO => "Los síntomas de " + _petName + " requieren atención veterinaria. Programa una cita lo antes posible.",
        _ => "Los síntomas de " + _petName + " son leves. Monitoreo en casa con observación cuidadosa."
    };
    private string Recommendation() => TriageEngine.GetBaseRecommendation(_level);

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        var color = SeverityColor();
        using (var path = DrawingHelpers.RoundedRect(rect, 22))
        using (var brush = new SolidBrush(AppTheme.WithAlpha(color, 14)))
        {
            g.FillPath(brush, path);
        }
        DrawingHelpers.StrokeRounded(g, AppTheme.WithAlpha(color, 110), 1.2f, rect, 22);

        // Círculo pulsante
        float pulse = (float)((Math.Sin(_phase) + 1) / 2);
        int circleSize = 110;
        var circle = new Rectangle(46, (Height - circleSize) / 2, circleSize, circleSize);

        int halo = circleSize + 14 + (int)(pulse * 16);
        var haloRect = new Rectangle(46 - (halo - circleSize) / 2, (Height - halo) / 2, halo, halo);
        using (var haloBrush = new SolidBrush(Color.FromArgb(12 + (int)(pulse * 30), color)))
        {
            g.FillEllipse(haloBrush, haloRect);
        }
        using (var circleBrush = new SolidBrush(AppTheme.WithAlpha(color, 26)))
        {
            g.FillEllipse(circleBrush, circle);
        }
        using (var circlePen = new Pen(AppTheme.WithAlpha(color, 150), 2f))
        {
            g.DrawEllipse(circlePen, circle);
        }
        using (var glow = new SolidBrush(Color.FromArgb(30, color)))
        {
            g.FillEllipse(glow, new Rectangle(circle.X - 8, circle.Y - 8, circleSize + 16, circleSize + 16));
        }
        using (var emoji = AppTheme.Emoji(22f))
        {
            DrawingHelpers.DrawEmoji(g, Icon(), emoji, circle, color);
        }

        int textX = circle.Right + 36;
        int textW = Width - textX - 30;

        // Etiqueta
        var badgeRect = new Rectangle(textX, 26, 150, 26);
        using (var bpath = DrawingHelpers.RoundedRect(badgeRect, 13))
        using (var bbrush = new SolidBrush(AppTheme.WithAlpha(color, 26)))
        {
            g.FillPath(bbrush, bpath);
        }
        using (var bfont = AppTheme.Medium(7.5f))
        {
            TextRenderer.DrawText(g, "NIVEL DE URGENCIA · " + _level.ToLabel(), bfont, badgeRect, color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        using (var titleFont = AppTheme.Heading(16f))
        {
            TextRenderer.DrawText(g, Title(), titleFont, new Rectangle(textX, 58, textW, 36),
                AppTheme.Foreground, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        using (var descFont = AppTheme.Body(9.5f))
        {
            TextRenderer.DrawText(g, Description(), descFont, new Rectangle(textX, 96, textW, 40),
                AppTheme.MutedForeground, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }
        using (var recFont = AppTheme.Medium(8.5f))
        {
            int recY = 150;
            using (var recEmoji = AppTheme.Emoji(10.5f))
            {
                DrawingHelpers.DrawEmoji(g, "💡", recEmoji, new Rectangle(textX, recY - 2, 24, 28), color);
            }
            TextRenderer.DrawText(g, "  " + Recommendation(), recFont, new Rectangle(textX + 26, recY, textW - 26, 26),
                color, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);
        }
    }
}