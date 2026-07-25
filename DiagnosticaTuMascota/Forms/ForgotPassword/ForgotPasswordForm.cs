using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.ForgotPassword;

public partial class ForgotPasswordForm : Form
{
    private int _currentStep = 1;
    private RoundedTextBox _emailBox = null!;
    private RoundedTextBox _phoneBox = null!;
    private RoundedTextBox _newPassBox = null!;
    private RoundedTextBox _confirmPassBox = null!;
    private RoundedButton _actionBtn = null!;
    private Panel _step1Panel = null!;
    private Panel _step2Panel = null!;
    private Panel _step3Panel = null!;
    private Panel _dotsPanel = null!;
    private RoundedPanel _card = null!;

    public ForgotPasswordForm()
    {
        InitializeComponent();
        BuildUI();
    }

    private void BuildUI()
    {
        AppTheme.ApplyFormStyle(this, "Diagnostica tu Mascota - Restablecer Contrasena");
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(500, 600);
        BackColor = AppTheme.Background;

        var bgGlow1 = new Panel { Size = new Size(400, 400), BackColor = Color.Transparent, Location = new Point(-100, -100) };
        bgGlow1.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = new();
            path.AddEllipse(0, 0, 400, 400);
            using PathGradientBrush brush = new(path);
            brush.CenterColor = Color.FromArgb(20, AppTheme.Primary);
            brush.SurroundColors = new[] { Color.Transparent };
            e.Graphics.FillPath(brush, path);
        };
        Controls.Add(bgGlow1);

        _card = new RoundedPanel
        {
            Size = new Size(420, 520),
            BackColor = AppTheme.Card,
            BorderColor = AppTheme.Border,
            BorderWidth = 1,
            CornerRadius = 24
        };
        Controls.Add(_card);
        CenterCard();
        Resize += (s, e) => CenterCard();

        int cardW = 420;
        int y = 24;

        // Back link
        var backLink = new Label { Text = "\u2190 Volver al inicio de sesion", Font = new Font("Segoe UI", 11f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(40, y) };
        backLink.Click += (s, e) => { var lf = new Login.LoginForm(); lf.Show(); Close(); };
        _card.Controls.Add(backLink);
        y += 36;

        // Logo
        var logoPanel = new Panel { Size = new Size(64, 64), Location = new Point((cardW - 64) / 2, y), BackColor = Color.Transparent };
        logoPanel.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new(0, 0, 64, 64);
            using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, 16);
            using LinearGradientBrush brush = new(bounds, AppTheme.Primary, AppTheme.Secondary, LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillPath(brush, path);
            using Font iconFont = new("Segoe UI Symbol", 28f);
            using SolidBrush iconBrush = new(Color.White);
            SizeF iconSize = e.Graphics.MeasureString("\U0001F511", iconFont);
            e.Graphics.DrawString("\U0001F511", iconFont, iconBrush, (64 - iconSize.Width) / 2, (64 - iconSize.Height) / 2);
        };
        _card.Controls.Add(logoPanel);
        y += 80;

        _card.Controls.Add(new Label { Text = "Restablecer Contrasena", Font = AppTheme.TitleH3, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point((cardW - 260) / 2, y) });
        y += 40;

        // Dots
        _dotsPanel = new Panel { Size = new Size(cardW - 80, 12), Location = new Point(40, y), BackColor = Color.Transparent };
        _dotsPanel.Paint += DotsPanel_Paint;
        _card.Controls.Add(_dotsPanel);
        y += 28;

        // Step 1
        _step1Panel = new Panel { Size = new Size(cardW - 80, 100), Location = new Point(40, y), BackColor = Color.Transparent };
        _step1Panel.Controls.Add(new Label { Text = "Correo electronico", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 0) });
        _emailBox = new RoundedTextBox { PlaceholderText = "usuario@ejemplo.com", Location = new Point(0, 26), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _step1Panel.Controls.Add(_emailBox);
        _card.Controls.Add(_step1Panel);

        // Step 2
        _step2Panel = new Panel { Size = new Size(cardW - 80, 100), Location = new Point(40, y), BackColor = Color.Transparent, Visible = false };
        _step2Panel.Controls.Add(new Label { Text = "Por seguridad, requerimos el telefono registrado.", Font = new Font("Segoe UI", 9f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 0) });
        _step2Panel.Controls.Add(new Label { Text = "Telefono registrado", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 26) });
        _phoneBox = new RoundedTextBox { PlaceholderText = "+1 234 567 890", Location = new Point(0, 52), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _step2Panel.Controls.Add(_phoneBox);
        _card.Controls.Add(_step2Panel);

        // Step 3
        _step3Panel = new Panel { Size = new Size(cardW - 80, 140), Location = new Point(40, y), BackColor = Color.Transparent, Visible = false };
        _step3Panel.Controls.Add(new Label { Text = "Nueva contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 0) });
        _newPassBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(0, 26), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        _step3Panel.Controls.Add(_newPassBox);
        _step3Panel.Controls.Add(new Label { Text = "Confirmar contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 80) });
        _confirmPassBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(0, 106), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        _step3Panel.Controls.Add(_confirmPassBox);
        _card.Controls.Add(_step3Panel);

        y += 140;

        // Action button
        _actionBtn = new RoundedButton { Text = "Continuar", Location = new Point(40, y), Size = new Size(cardW - 80, 50), CornerRadius = 14, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(16f) };
        _actionBtn.Click += ActionBtn_Click;
        _card.Controls.Add(_actionBtn);

        var closeBtn = new Label { Text = "\u2715", Font = new Font("Segoe UI", 14f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardW - 40, 12), Cursor = Cursors.Hand };
        closeBtn.Click += (s, e) => Application.Exit();
        _card.Controls.Add(closeBtn);
    }

    private void CenterCard()
    {
        if (_card == null) return;
        _card.Location = new Point((ClientSize.Width - _card.Width) / 2, (ClientSize.Height - _card.Height) / 2);
    }

    private void DotsPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int totalWidth = 3 * 8 + 2 * 8;
        int startX = (_dotsPanel.Width - totalWidth) / 2;
        for (int i = 1; i <= 3; i++)
        {
            int w = (i == _currentStep) ? 32 : 8;
            int x = startX + (i - 1) * 16;
            Color color = (i <= _currentStep) ? AppTheme.Primary : AppTheme.Muted;
            using GraphicsPath path = AppTheme.GetRoundedRectPath(new Rectangle(x, 0, w, 6), 3);
            using SolidBrush brush = new(color);
            g.FillPath(brush, path);
        }
    }

    private void ActionBtn_Click(object? sender, EventArgs e)
    {
        if (_currentStep == 1)
        {
            _step1Panel.Visible = false;
            _step2Panel.Visible = true;
            _currentStep = 2;
            _actionBtn.Text = "Verificar Identidad";
        }
        else if (_currentStep == 2)
        {
            _step2Panel.Visible = false;
            _step3Panel.Visible = true;
            _currentStep = 3;
            _actionBtn.Text = "Actualizar Contrasena";
        }
        else
        {
            var lf = new Login.LoginForm();
            lf.Show();
            Close();
        }
        _dotsPanel.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(AppTheme.Background);
    }
}
