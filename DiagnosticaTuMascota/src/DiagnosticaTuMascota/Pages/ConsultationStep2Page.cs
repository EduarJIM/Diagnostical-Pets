using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

/// <summary>
/// Paso 2 del análisis (ConsultationStep2.tsx): asignación de mascota e
/// información adicional antes de generar el diagnóstico.
/// </summary>
public sealed class ConsultationStep2Page : PageBase
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
    private PillButton stepPill = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ProgressBarSkin progress = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel headerRow = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel headerIcon = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel subtitle = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel card = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel layout = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel petLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel infoLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel tip = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel tipLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton submit = null!;
    private const string ManualOption = "Otra Mascota (No Registrada)";

    private readonly INavigator _nav;
    private readonly RoundedComboBox _petCombo;
    private readonly RoundedTextBox _manualName;
    private readonly ThemeLabel _manualLabel;
    private readonly RoundedTextBox _additional;
    private NavState? _pending;

    public ConsultationStep2Page(INavigator nav)
    {
        _nav = nav;
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

        // Barra superior
        topBar = new Panel { Height = 46, Width = 940, BackColor = Color.Transparent };
        back = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "←  Volver a Síntomas", Cursor = Cursors.Hand, Anchor = AnchorStyles.Left };
        back.Click += (_, _) => _nav.Navigate(AppPage.SymptomAnalysis);
        stepPill = new PillButton { Text = "Paso 2 de 2", IconText = "🗂️", Active = true, Width = 130, Height = 34, ActiveColor = AppTheme.Secondary };
        progress = new ProgressBarSkin { Width = 220, Height = 8, Percent = 100, Anchor = AnchorStyles.Left };
        topBar.Controls.Add(back);
        topBar.Controls.Add(stepPill);
        topBar.Controls.Add(progress);
        topBar.Resize += (_, _) =>
        {
            back.Location = new Point(4, 12);
            progress.Location = new Point(back.Right + 130, 19);
            stepPill.Location = new Point(progress.Right + 14, 6);
        };
        root.Controls.Add(topBar);

        headerRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent };
        headerIcon = new ThemeLabel { TextKind = TextKind.Body, Text = "🩺", Font = AppTheme.Emoji(15f) };
        title = new ThemeLabel { TextKind = TextKind.Display, Text = "  Asignación de Consulta" };
        headerRow.Controls.Add(headerIcon);
        headerRow.Controls.Add(title);
        root.Controls.Add(headerRow);

        subtitle = new ThemeLabel
        {
            TextKind = TextKind.Body,
            Text = "Completa los datos de la consulta para generar la orientación clínica.",
            MaximumSize = new Size(900, 40),
            AutoEllipsis = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(38, 0, 0, 18)
        };
        root.Controls.Add(subtitle);

        // Tarjeta principal
        card = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 20,
            BorderColor = AppTheme.Border,
            Width = 940,
            Height = 500,
            Padding = new Padding(30, 24, 30, 24)
        };

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            BackColor = Color.Transparent
        };
        for (int i = 0; i < 9; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        petLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Mascota para la consulta", Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 3) };
        layout.Controls.Add(petLabel, 0, 0);

        _petCombo = new RoundedComboBox
        {
            Width = 800,
            Height = 44,
            FieldHeight = 44,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };
        layout.Controls.Add(_petCombo, 0, 1);

        _manualLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Nombre de la mascota (no registrada)", Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 3) };
        layout.Controls.Add(_manualLabel, 0, 2);

        _manualName = new RoundedTextBox
        {
            IconText = "🐾",
            PlaceholderText = "Ej. Rocky",
            Width = 800,
            Height = 42,
            FieldHeight = 42,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };
        layout.Controls.Add(_manualName, 0, 3);

        infoLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Información adicional o contexto importante", Dock = DockStyle.Fill, Margin = new Padding(0, 14, 0, 3) };
        layout.Controls.Add(infoLabel, 0, 4);

        _additional = new RoundedTextBox
        {
            IconText = "📝",
            PlaceholderText = "Describe cualquier detalle relevante: tiempo con síntomas, cambios de apetito, medicación actual, etc.",
            Width = 800,
            Height = 110,
            FieldHeight = 110,
            Multiline = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Right
        };
        layout.Controls.Add(_additional, 0, 5);

        tip = new RoundedPanel
        {
            FillColor = AppTheme.WithAlpha(AppTheme.Secondary, 14),
            CornerRadius = 12,
            BorderColor = AppTheme.WithAlpha(AppTheme.Secondary, 60),
            Height = 46,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 16, 0, 4)
        };
        tipLabel = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = "💡  Será agregada automáticamente a tu historial de consultas.",
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Location = new Point(16, 12)
        };
        tip.Controls.Add(tipLabel);
        tip.Resize += (_, _) => tipLabel.Location = new Point(16, (tip.Height - tipLabel.Height) / 2);
        layout.Controls.Add(tip, 0, 6);

        submit = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Generar Diagnóstico y Guardar",
            Icon = "🧬",
            IconSize = 10f,
            Height = 48,
            Width = 330,
            Anchor = AnchorStyles.None,
            Font = AppTheme.Medium(10.5f),
            Margin = new Padding(0, 18, 0, 0)
        };
        submit.Click += (_, _) => Generate();
        layout.Controls.Add(submit, 0, 7);

        card.Controls.Add(layout);
        root.Controls.Add(card);

        body.Controls.Add(root);
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        _pending = state;
        // Si llega sin síntomas, volver al paso 1
        if (state?.SelectedSymptoms is not { Count: > 0 })
        {
            _nav.Navigate(AppPage.SymptomAnalysis);
            return;
        }

        var pets = _nav.Storage.GetData("petsData", DemoData.CreateInitialPets());
        _petCombo.SetItems(pets.Select(p => p.Name).Concat(new[] { ManualOption }));
        _petCombo.SelectedIndex = pets.Count > 0 ? 0 : pets.Count; // última opción = manual
        _petCombo.SelectedIndexChanged -= OnPetChanged;
        _petCombo.SelectedIndexChanged += OnPetChanged;
        OnPetChanged(_petCombo, EventArgs.Empty);
        _manualName.Text = "";
        _additional.Text = "";
    }

    private void OnPetChanged(object? sender, EventArgs e)
    {
        bool manual = _petCombo.SelectedText == ManualOption;
        _manualLabel.Visible = manual;
        _manualName.Visible = manual;
    }

    private void Generate()
    {
        var state = _pending;
        if (state?.SelectedSymptoms is not { Count: > 0 }) return;

        string finalPetName;
        if (_petCombo.SelectedText == ManualOption)
        {
            finalPetName = string.IsNullOrWhiteSpace(_manualName.Text) ? "Mascota No Registrada" : _manualName.Text.Trim();
        }
        else
        {
            finalPetName = _petCombo.SelectedText;
        }

        _nav.Navigate(AppPage.DiagnosticResult, new NavState
        {
            SelectedSymptoms = state.SelectedSymptoms,
            PetName = finalPetName,
            AdditionalInfo = _additional.Text.Trim()
        });
    }
}