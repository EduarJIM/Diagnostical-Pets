using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.SymptomAnalysis;

public partial class SymptomAnalysisControl : UserControl
{
    private readonly Forms.MainForm? _mainForm;
    private readonly HashSet<string> _selectedSymptoms = new();
    private Label _countLabel = null!;
    private RoundedButton _continueBtn = null!;
    private Panel _symptomsPanel = null!;
    private string _activeCategory = "general";

    private readonly Dictionary<string, (string Icon, Color Color1, Color Color2, string[] Symptoms)> _categories = new()
    {
        ["general"] = ("\u26A1", AppTheme.Primary, AppTheme.Secondary, new[] { "Fiebre", "Letargo/Debilidad", "Perdida de peso", "Aumento de sed", "Temblores" }),
        ["respiratory"] = ("\U0001F4A8", AppTheme.Blue500, AppTheme.Cyan500, new[] { "Tos persistente", "Estornudos", "Dificultad para respirar", "Secrecion nasal", "Respiracion ruidosa" }),
        ["digestive"] = ("\U0001F37D", AppTheme.Orange500, AppTheme.Amber500, new[] { "Vomitos", "Diarrea", "Perdida de apetito", "Dificultad al tragar", "Abdomen hinchado" }),
        ["neurological"] = new("\U0001F9E0", AppTheme.Purple500, AppTheme.Indigo500, new[] { "Convulsiones", "Desorientacion", "Incoordinacion al caminar", "Paralisis", "Cambio de conducta" })
    };

    public SymptomAnalysisControl(Forms.MainForm? mainForm = null)
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
        ((Label)content.Controls[^1]).Click += (s, e) => _mainForm?.NavigateTo("dashboard");
        y += 34;

        var progressBg = new Panel { Location = new Point(0, y), Size = new Size(500, 6), BackColor = AppTheme.Muted };
        progressBg.Paint += (s, e) =>
        {
            AppTheme.DrawRoundedRectangle(e.Graphics, new Rectangle(0, 0, progressBg.Width, 6), 3, AppTheme.Muted);
            AppTheme.DrawRoundedRectangle(e.Graphics, new Rectangle(0, 0, progressBg.Width / 2, 6), 3, AppTheme.Primary);
        };
        content.Controls.Add(progressBg);
        y += 18;

        content.Controls.Add(new Label { Text = "Paso 1 de 2", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 26;

        content.Controls.Add(new Label { Text = "Analisis de Sintomas", Font = AppTheme.TitleH1, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(0, y) });
        y += 42;

        content.Controls.Add(new Label { Text = "Selecciona todos los sintomas que has observado en tu mascota.", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 36;

        int catY = y;
        foreach (var cat in _categories)
        {
            bool isActive = cat.Key == _activeCategory;
            var catBtn = new RoundedButton
            {
                Text = $"  {cat.Value.Icon}  {cat.Key.ToUpper()}",
                Location = new Point(0, catY),
                Size = new Size(210, 48),
                CornerRadius = 14,
                ButtonColor1 = isActive ? cat.Value.Color1 : AppTheme.Card,
                ButtonColor2 = isActive ? cat.Value.Color1 : AppTheme.Card,
                UseGradient = false,
                TextColor = isActive ? Color.White : AppTheme.Foreground,
                Font = AppTheme.FontBold(11f),
                Cursor = Cursors.Hand
            };
            string key = cat.Key;
            catBtn.Click += (s, e) => { _activeCategory = key; LoadSymptoms(key); };
            content.Controls.Add(catBtn);
            catY += 56;
        }

        y = catY + 10;

        _symptomsPanel = new Panel { Location = new Point(230), Size = new Size(500, 280), BackColor = Color.Transparent, AutoScroll = true };
        _symptomsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        content.Controls.Add(_symptomsPanel);

        y = Math.Max(y, 280);

        _countLabel = new Label { Text = "Seleccionados: 0 sintomas", Font = AppTheme.BodySmall, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y + 8) };
        content.Controls.Add(_countLabel);

        _continueBtn = new RoundedButton
        {
            Text = "Continuar Analisis \u2192",
            Location = new Point(0, y + 40),
            Size = new Size(220, 44),
            CornerRadius = 12,
            ButtonColor1 = AppTheme.Primary,
            ButtonColor2 = AppTheme.Secondary,
            UseGradient = true,
            TextColor = Color.White,
            Font = AppTheme.FontBold(13f),
            Enabled = false
        };
        _continueBtn.Click += (s, e) => _mainForm?.NavigateTo("consultation");
        content.Controls.Add(_continueBtn);

        Controls.Add(content);
        LoadSymptoms("general");
    }

    private void LoadSymptoms(string category)
    {
        _symptomsPanel.Controls.Clear();
        if (!_categories.ContainsKey(category)) return;
        var (_, _, _, symptoms) = _categories[category];

        int y = 0;
        foreach (var symptom in symptoms)
        {
            var symptomBtn = new Panel { Size = new Size(200, 44), Location = new Point(0, y), BackColor = Color.Transparent, Cursor = Cursors.Hand, Tag = symptom };
            symptomBtn.Paint += SymptomBtn_Paint;
            symptomBtn.Click += SymptomBtn_Click;
            _symptomsPanel.Controls.Add(symptomBtn);
            y += 52;
        }
    }

    private void SymptomBtn_Paint(object? sender, PaintEventArgs e)
    {
        if (sender is not Panel panel || panel.Tag is not string symptom) return;
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        bool isSel = _selectedSymptoms.Contains(symptom);
        Rectangle bounds = new(0, 0, panel.Width, panel.Height);
        AppTheme.DrawRoundedRectangle(g, bounds, 14, isSel ? Color.FromArgb(25, AppTheme.Primary) : AppTheme.Card, isSel ? Color.FromArgb(128, AppTheme.Primary) : AppTheme.Border, 1);

        Rectangle checkBounds = new(panel.Width - 30, 12, 20, 20);
        using GraphicsPath checkPath = AppTheme.GetRoundedRectPath(checkBounds, 10);
        using Pen checkPen = new(isSel ? AppTheme.Primary : AppTheme.MutedForeground, 2);
        g.DrawPath(checkPen, checkPath);

        if (isSel)
        {
            Rectangle dotBounds = new(panel.Width - 24, 18, 8, 8);
            using GraphicsPath dotPath = AppTheme.GetRoundedRectPath(dotBounds, 4);
            using SolidBrush dotBrush = new(AppTheme.Primary);
            g.FillPath(dotBrush, dotPath);
        }

        using Font textFont = AppTheme.FontRegular(12f);
        using SolidBrush textBrush = new(isSel ? AppTheme.Primary : AppTheme.Foreground);
        g.DrawString(symptom, textFont, textBrush, 14, (panel.Height - 16) / 2);
    }

    private void SymptomBtn_Click(object? sender, EventArgs e)
    {
        if (sender is not Panel panel || panel.Tag is not string symptom) return;
        if (_selectedSymptoms.Contains(symptom)) _selectedSymptoms.Remove(symptom);
        else _selectedSymptoms.Add(symptom);
        _countLabel.Text = $"Seleccionados: {_selectedSymptoms.Count} sintomas";
        _continueBtn.Enabled = _selectedSymptoms.Count > 0;
        panel.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); e.Graphics.Clear(AppTheme.Background); }
}
