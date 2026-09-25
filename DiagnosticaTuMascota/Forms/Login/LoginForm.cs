using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.Login;

public partial class LoginForm : Form
{
    private RoundedTextBox _emailBox = null!;
    private RoundedTextBox _passwordBox = null!;
    private RoundedButton _loginBtn = null!;
    private Label _registerLink = null!;
    private Label _forgotLink = null!;
    private RoundedPanel _card = null!;

    public LoginForm()
    {
        InitializeComponent();
        BuildUI();
    }

    private void BuildUI()
    {
        AppTheme.ApplyFormStyle(this, "Diagnostica tu Mascota - Iniciar Sesion");
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(500, 600);
        BackColor = AppTheme.Background;

        // Background decorative glows
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

        var bgGlow2 = new Panel { Size = new Size(400, 400), BackColor = Color.Transparent };
        bgGlow2.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = new();
            path.AddEllipse(0, 0, 400, 400);
            using PathGradientBrush brush = new(path);
            brush.CenterColor = Color.FromArgb(20, AppTheme.Secondary);
            brush.SurroundColors = new[] { Color.Transparent };
            e.Graphics.FillPath(brush, path);
        };
        Controls.Add(bgGlow2);

        // Card
        _card = new RoundedPanel
        {
            Size = new Size(420, 560),
            BackColor = AppTheme.Card,
            BorderColor = AppTheme.Border,
            BorderWidth = 1,
            CornerRadius = 24
        };
        Controls.Add(_card);

        CenterCard();
        Resize += (s, e) => { CenterCard(); bgGlow2.Location = new Point(ClientSize.Width - 300, ClientSize.Height - 300); };

        int cardW = 420;
        int y = 40;

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
            SizeF iconSize = e.Graphics.MeasureString("\U0001FA7A", iconFont);
            e.Graphics.DrawString("\U0001FA7A", iconFont, iconBrush, (64 - iconSize.Width) / 2, (64 - iconSize.Height) / 2);
        };
        _card.Controls.Add(logoPanel);
        y += 84;

        // Title
        _card.Controls.Add(new Label { Text = "Bienvenido de nuevo", Font = AppTheme.TitleH2, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point((cardW - 250) / 2, y) });
        y += 40;

        // Subtitle
        _card.Controls.Add(new Label { Text = "Ingresa a tu cuenta de Diagnostica tu Mascota", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point((cardW - 300) / 2, y) });
        y += 44;

        // Email
        _card.Controls.Add(new Label { Text = "Correo electronico", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _emailBox = new RoundedTextBox { PlaceholderText = "usuario@ejemplo.com", Location = new Point(40, y), Size = new Size(cardW - 80, 48), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _card.Controls.Add(_emailBox);
        y += 64;

        // Password
        _card.Controls.Add(new Label { Text = "Contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _passwordBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(40, y), Size = new Size(cardW - 80, 48), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        _card.Controls.Add(_passwordBox);
        y += 60;

        // Forgot password
        _forgotLink = new Label { Text = "Olvidaste tu contrasena?", Font = new Font("Segoe UI", 10f), ForeColor = AppTheme.Primary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(40, y) };
        _forgotLink.Click += (s, e) =>
        {
            var fp = new ForgotPassword.ForgotPasswordForm();
            fp.Show();
            Hide();
        };
        _card.Controls.Add(_forgotLink);
        y += 36;

        // Login button
        _loginBtn = new RoundedButton { Text = "Iniciar Sesion", Location = new Point(40, y), Size = new Size(cardW - 80, 50), CornerRadius = 14, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(16f) };
        _loginBtn.Click += LoginBtn_Click;
        _card.Controls.Add(_loginBtn);
        y += 70;

        // Register link
        var registerPanel = new Panel { AutoSize = true, Location = new Point((cardW - 280) / 2, y), BackColor = Color.Transparent };
        var regText = new Label { Text = "No tienes una cuenta? ", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, 0) };
        _registerLink = new Label { Text = "Registrate aqui", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = AppTheme.Primary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(regText.PreferredSize.Width, 0) };
        _registerLink.Click += (s, e) =>
        {
            var registerForm = new Register.RegisterForm();
            registerForm.Show();
            Hide();
        };
        registerPanel.Controls.Add(regText);
        registerPanel.Controls.Add(_registerLink);
        _card.Controls.Add(registerPanel);

        // Close button
        var closeBtn = new Label { Text = "\u2715", Font = new Font("Segoe UI", 14f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardW - 40, 12), Cursor = Cursors.Hand };
        closeBtn.Click += (s, e) => Application.Exit();
        _card.Controls.Add(closeBtn);
    }

    private void CenterCard()
    {
        if (_card == null) return;
        _card.Location = new Point((ClientSize.Width - _card.Width) / 2, (ClientSize.Height - _card.Height) / 2);
    }

    private void LoginBtn_Click(object? sender, EventArgs e)
    {
        var mainForm = new MainForm();
        mainForm.Show();
        Close();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(AppTheme.Background);
    }
}
