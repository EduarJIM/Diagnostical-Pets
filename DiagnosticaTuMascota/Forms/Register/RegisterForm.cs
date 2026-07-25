using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.Register;

public partial class RegisterForm : Form
{
    private RoundedTextBox _nameBox = null!;
    private RoundedTextBox _phoneBox = null!;
    private RoundedTextBox _emailBox = null!;
    private RoundedTextBox _passwordBox = null!;
    private RoundedTextBox _confirmBox = null!;
    private RoundedButton _registerBtn = null!;
    private Label _loginLink = null!;
    private RoundedPanel _card = null!;

    public RegisterForm()
    {
        InitializeComponent();
        BuildUI();
    }

    private void BuildUI()
    {
        AppTheme.ApplyFormStyle(this, "Diagnostica tu Mascota - Crear Cuenta");
        ClientSize = new Size(1000, 750);
        MinimumSize = new Size(500, 650);
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
            Size = new Size(480, 680),
            BackColor = AppTheme.Card,
            BorderColor = AppTheme.Border,
            BorderWidth = 1,
            CornerRadius = 24
        };
        Controls.Add(_card);
        CenterCard();
        Resize += (s, e) => CenterCard();

        int cardW = 480;
        int y = 30;

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

        _card.Controls.Add(new Label { Text = "Crea tu cuenta", Font = AppTheme.TitleH2, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point((cardW - 200) / 2, y) });
        y += 40;

        _card.Controls.Add(new Label { Text = "Unete a la mejor comunidad de cuidado para mascotas", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point((cardW - 330) / 2, y) });
        y += 40;

        // Name
        _card.Controls.Add(new Label { Text = "Nombre completo", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _nameBox = new RoundedTextBox { PlaceholderText = "Juan Perez", Location = new Point(40, y), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _card.Controls.Add(_nameBox);
        y += 56;

        // Phone
        _card.Controls.Add(new Label { Text = "Telefono", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _phoneBox = new RoundedTextBox { PlaceholderText = "+1 234 567 890", Location = new Point(40, y), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _card.Controls.Add(_phoneBox);
        y += 56;

        // Email
        _card.Controls.Add(new Label { Text = "Correo electronico", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _emailBox = new RoundedTextBox { PlaceholderText = "usuario@ejemplo.com", Location = new Point(40, y), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground };
        _card.Controls.Add(_emailBox);
        y += 56;

        // Password
        _card.Controls.Add(new Label { Text = "Contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _passwordBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(40, y), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        _card.Controls.Add(_passwordBox);
        y += 56;

        // Confirm password
        _card.Controls.Add(new Label { Text = "Confirmar contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(40, y) });
        y += 24;
        _confirmBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(40, y), Size = new Size(cardW - 80, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        _card.Controls.Add(_confirmBox);
        y += 56;

        // Register button
        _registerBtn = new RoundedButton { Text = "Crear Cuenta", Location = new Point(40, y), Size = new Size(cardW - 80, 50), CornerRadius = 14, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(16f) };
        _registerBtn.Click += (s, e) =>
        {
            var loginForm = new Login.LoginForm();
            loginForm.Show();
            Close();
        };
        _card.Controls.Add(_registerBtn);
        y += 64;

        // Login link
        var loginPanel = new Panel { AutoSize = true, Location = new Point((cardW - 250) / 2, y), BackColor = Color.Transparent };
        var loginText = new Label { Text = "Ya tienes una cuenta? ", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true };
        _loginLink = new Label { Text = "Inicia sesion", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = AppTheme.Primary, AutoSize = true, Cursor = Cursors.Hand, Location = new Point(loginText.PreferredSize.Width, 0) };
        _loginLink.Click += (s, e) =>
        {
            var loginForm = new Login.LoginForm();
            loginForm.Show();
            Close();
        };
        loginPanel.Controls.Add(loginText);
        loginPanel.Controls.Add(_loginLink);
        _card.Controls.Add(loginPanel);

        var closeBtn = new Label { Text = "\u2715", Font = new Font("Segoe UI", 14f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(cardW - 40, 12), Cursor = Cursors.Hand };
        closeBtn.Click += (s, e) => Application.Exit();
        _card.Controls.Add(closeBtn);
    }

    private void CenterCard()
    {
        if (_card == null) return;
        _card.Location = new Point((ClientSize.Width - _card.Width) / 2, (ClientSize.Height - _card.Height) / 2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(AppTheme.Background);
    }
}
