using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.DiagnosticResult;

public partial class DiagnosticResultControl : UserControl
{
    private readonly Forms.MainForm? _mainForm;

    public DiagnosticResultControl(Forms.MainForm? mainForm = null)
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
        Padding = new Padding(48, 32, 48, 32);

        var content = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = Color.Transparent, MinimumSize = new Size(400, 0) };
        int y = 0;

        content.Controls.Add(new Label { Text = "Analisis Clinico de Max", Font = AppTheme.TitleH1, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(0, y) });
        y += 40;

        content.Controls.Add(new Label { Text = "Evaluacion preliminar basada en protocolos de triaje veterinario.", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 40;

        var banner = new Panel { Location = new Point(0, y), Size = new Size(700, 110), BackColor = AppTheme.CriticalBg, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        banner.Paint += (s, e) =>
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle iconBounds = new(20, 20, 66, 66);
            using GraphicsPath iconPath = AppTheme.GetRoundedRectPath(iconBounds, 33);
            using SolidBrush iconBrush = new(AppTheme.Critical);
            g.FillPath(iconBrush, iconPath);
            using Font iconFont = new("Segoe UI Symbol", 30f);
            using SolidBrush iconTextBrush = new(Color.White);
            SizeF iconSize = g.MeasureString("\u26A0", iconFont);
            g.DrawString("\u26A0", iconFont, iconTextBrush, iconBounds.X + (66 - iconSize.Width) / 2, iconBounds.Y + (66 - iconSize.Height) / 2);

            using Font badgeFont = AppTheme.FontBold(10f);
            using SolidBrush badgeBrush = new(AppTheme.Critical);
            g.DrawString("NIVEL DE URGENCIA: CRITICO", badgeFont, badgeBrush, 100, 26);

            using Font titleFont = AppTheme.FontBold(16f);
            g.DrawString("Atencion Veterinaria Inmediata Requerida", titleFont, badgeBrush, 100, 48);

            using Font descFont = new("Segoe UI", 10.5f);
            using SolidBrush descBrush = new(AppTheme.MutedForeground);
            g.DrawString("Los sintomas indican una condicion que requiere atencion veterinaria urgente.", descFont, descBrush, 100, 74);
        };
        content.Controls.Add(banner);
        y += 126;

        var cardsLayout = new TableLayoutPanel { Location = new Point(0, y), ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var implCard = new RoundedPanel { Dock = DockStyle.Fill, BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 18, Padding = new Padding(20) };
        implCard.Paint += (s, e) =>
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using Font tf = AppTheme.FontBold(14f);
            using SolidBrush tb = new(AppTheme.Foreground);
            e.Graphics.DrawString("Implicaciones Clinicas", tf, tb, 20, 20);
            string[] items = { "Fiebre elevada indicando infeccion activa", "Letargo puede indicar malestar general", "Vomitos persistentes requieren atencion" };
            int iy = 52;
            foreach (var item in items)
            {
                using SolidBrush db = new(AppTheme.Critical);
                e.Graphics.FillEllipse(db, 24, iy + 4, 7, 7);
                using Font if2 = new("Segoe UI", 10.5f);
                using SolidBrush ib = new(AppTheme.MutedForeground);
                e.Graphics.DrawString(item, if2, ib, 38, iy);
                iy += 26;
            }
        };
        cardsLayout.Controls.Add(implCard, 0, 0);

        var actCard = new RoundedPanel { Dock = DockStyle.Fill, BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 18, Padding = new Padding(20) };
        actCard.Paint += (s, e) =>
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using Font tf = AppTheme.FontBold(14f);
            using SolidBrush tb = new(AppTheme.Foreground);
            e.Graphics.DrawString("Primeros Auxilios / Acciones", tf, tb, 20, 20);
            string[] items = { "Mantener a la mascota hidratada", "Evitar alimentos solidos por 12 horas", "Acudir al veterinario de inmediato" };
            int iy = 52;
            foreach (var item in items)
            {
                using SolidBrush db = new(AppTheme.Critical);
                e.Graphics.FillEllipse(db, 24, iy + 4, 7, 7);
                using Font if2 = new("Segoe UI", 10.5f);
                using SolidBrush ib = new(AppTheme.MutedForeground);
                e.Graphics.DrawString(item, if2, ib, 38, iy);
                iy += 26;
            }
        };
        cardsLayout.Controls.Add(actCard, 1, 0);
        content.Controls.Add(cardsLayout);
        y += 180;

        var legal = new RoundedPanel { Location = new Point(0, y), Size = new Size(700, 50), BackColor = AppTheme.Muted, CornerRadius = 14, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        legal.Paint += (s, e) =>
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using Font tf = new("Segoe UI", 9.5f);
            using SolidBrush tb = new(AppTheme.MutedForeground);
            e.Graphics.DrawString("Aviso Legal: Este analisis es una herramienta de orientacion basada en sintomas generales. No sustituye el diagnostico profesional de un veterinario.", tf, tb, new RectangleF(20, 12, legal.Width - 40, 30));
        };
        content.Controls.Add(legal);
        y += 66;

        var returnBtn = new RoundedButton { Text = "\U0001F3E0  Finalizar y Volver al Inicio", Location = new Point(0, y), Size = new Size(280, 48), CornerRadius = 14, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(13f) };
        returnBtn.Click += (s, e) => _mainForm?.NavigateTo("dashboard");
        content.Controls.Add(returnBtn);

        Controls.Add(content);
    }

    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); e.Graphics.Clear(AppTheme.Background); }
}
