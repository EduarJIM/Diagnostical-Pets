using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.SymptomAnalysis;

public partial class ConsultationStep2Control : UserControl
{
    private readonly Forms.MainForm? _mainForm;
    private RoundedComboBox _petCombo = null!;
    private RoundedTextBox _petNameBox = null!;
    private TextBox _infoBox = null!;
    private RoundedButton _generateBtn = null!;

    public ConsultationStep2Control(Forms.MainForm? mainForm = null)
    {
        _mainForm = mainForm;
        InitializeComponent();
        BuildUI();
    }

    private void InitializeComponent() { }

    private void BuildUI()
    {
        BackColor = Color.Transparent;
        AutoSize = true;
        DoubleBuffered = true;
        Padding = new Padding(48, 24, 48, 24);

        var content = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = Color.Transparent, MinimumSize = new Size(400, 0) };
        int y = 0;

        content.Controls.Add(new Label { Text = "\u2190 Volver al Dashboard", Font = new Font("Segoe UI", 12f), ForeColor = AppTheme.Primary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(0, y) });
        ((Label)content.Controls[^1]).Click += (s, e) => _mainForm?.NavigateTo("symptoms");
        y += 34;

        var progressBg = new Panel { Location = new Point(0, y), Size = new Size(500, 6), BackColor = AppTheme.Primary };
        progressBg.Paint += (s, e) => AppTheme.DrawRoundedRectangle(e.Graphics, new Rectangle(0, 0, progressBg.Width, 6), 3, AppTheme.Primary);
        content.Controls.Add(progressBg);
        y += 18;

        content.Controls.Add(new Label { Text = "Paso 2 de 2", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 26;

        var card = new RoundedPanel { Location = new Point(0, y), Size = new Size(560, 380), BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 22, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

        int cardY = 28;
        int cardPad = 28;
        int fw = 560 - cardPad * 2;

        card.Controls.Add(new Label { Text = "Asignar Consulta", Font = AppTheme.TitleH2, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(cardPad, cardY) });
        cardY += 34;

        card.Controls.Add(new Label { Text = "Selecciona a cual de tus mascotas le pertenece este analisis.", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardPad, cardY) });
        cardY += 36;

        card.Controls.Add(new Label { Text = "Mascota Registrada", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardPad, cardY) });
        cardY += 22;

        _petCombo = new RoundedComboBox { Location = new Point(cardPad, cardY), Size = new Size(fw, 44), PlaceholderText = "Seleccionar mascota..." };
        _petCombo.Items.AddRange(new object[] { "Max (Golden Retriever)", "Luna (Gato Sames)", "Otra Mascota (No Registrada)" });
        card.Controls.Add(_petCombo);
        cardY += 56;

        card.Controls.Add(new Label { Text = "Nombre de la mascota", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardPad, cardY) });
        cardY += 22;

        _petNameBox = new RoundedTextBox { PlaceholderText = "Ej. Max, Luna...", Location = new Point(cardPad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        card.Controls.Add(_petNameBox);
        cardY += 56;

        card.Controls.Add(new Label { Text = "Informacion adicional o contexto importante", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardPad, cardY) });
        cardY += 22;

        _infoBox = new TextBox
        {
            PlaceholderText = "Ej. Empezo hace 2 dias, comio algo raro...",
            Location = new Point(cardPad, cardY),
            Size = new Size(fw, 90),
            BackColor = AppTheme.InputBg,
            ForeColor = AppTheme.Foreground,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.Body
        };
        card.Controls.Add(_infoBox);

        content.Controls.Add(card);
        y += 400;

        _generateBtn = new RoundedButton
        {
            Text = "Generar Diagnostico y Guardar",
            Location = new Point(0, y),
            Size = new Size(560, 50),
            CornerRadius = 14,
            ButtonColor1 = AppTheme.Primary,
            ButtonColor2 = AppTheme.Secondary,
            UseGradient = true,
            TextColor = Color.White,
            Font = AppTheme.FontBold(15f),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        _generateBtn.Click += (s, e) => _mainForm?.NavigateTo("result");
        content.Controls.Add(_generateBtn);

        Controls.Add(content);
    }

    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); e.Graphics.Clear(AppTheme.Background); }
}
