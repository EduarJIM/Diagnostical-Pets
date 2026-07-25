using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.Settings;

public partial class SettingsControl : UserControl
{
    private RoundedTextBox _nameBox = null!;
    private RoundedTextBox _emailBox = null!;
    private RoundedTextBox _currentPassBox = null!;
    private RoundedTextBox _newPassBox = null!;
    private RoundedTextBox _confirmPassBox = null!;
    private RoundedButton _saveBtn = null!;

    public SettingsControl()
    {
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

        var headerFlow = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, AutoSize = true, BackColor = Color.Transparent, WrapContents = false, Margin = new Padding(0, 0, 0, 6) };
        headerFlow.Controls.Add(new Label { Text = "\u2699", Font = new Font("Segoe UI Symbol", 26f), AutoSize = true, Margin = new Padding(0, 2, 10, 0) });
        headerFlow.Controls.Add(new Label { Text = "Configuracion de Cuenta", Font = AppTheme.TitleH1, ForeColor = AppTheme.Foreground, AutoSize = true, Margin = new Padding(0, 4, 0, 0) });
        content.Controls.Add(headerFlow);
        y += 46;

        content.Controls.Add(new Label { Text = "Administra tu informacion personal y credenciales de acceso.", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 36;

        var card = new RoundedPanel { Location = new Point(0, y), Size = new Size(600, 680), BackColor = AppTheme.Card, BorderColor = AppTheme.Border, BorderWidth = 1, CornerRadius = 22, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        int cardY = 24;
        int pad = 28;
        int fw = 600 - pad * 2;

        card.Controls.Add(new Label { Text = "\U0001F464  Informacion del Perfil", Font = AppTheme.FontBold(14f), ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 36;

        card.Controls.Add(new Label { Text = "Nombre completo", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 22;
        _nameBox = new RoundedTextBox { PlaceholderText = "Tu nombre", Location = new Point(pad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, Text = "Usuario Demo" };
        card.Controls.Add(_nameBox);
        cardY += 58;

        card.Controls.Add(new Panel { Location = new Point(pad, cardY), Size = new Size(fw, 1), BackColor = AppTheme.Border });
        cardY += 18;

        card.Controls.Add(new Label { Text = "\u2709  Correo Electronico", Font = AppTheme.FontBold(14f), ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 36;

        _emailBox = new RoundedTextBox { PlaceholderText = "correo@ejemplo.com", Location = new Point(pad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.MutedForeground, Text = "usuario@ejemplo.com", Enabled = false };
        card.Controls.Add(_emailBox);
        cardY += 24;

        card.Controls.Add(new Label { Text = "El correo electronico no se puede cambiar.", Font = new Font("Segoe UI", 9f), ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 26;

        card.Controls.Add(new Panel { Location = new Point(pad, cardY), Size = new Size(fw, 1), BackColor = AppTheme.Border });
        cardY += 18;

        card.Controls.Add(new Label { Text = "\U0001F512  Seguridad de Contrasena", Font = AppTheme.FontBold(14f), ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 36;

        var warning = new RoundedPanel { Location = new Point(pad, cardY), Size = new Size(fw, 32), BackColor = AppTheme.ModerateBg, CornerRadius = 8 };
        warning.Paint += (s, e) =>
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            using Font f = new("Segoe UI", 9.5f);
            using SolidBrush b = new(AppTheme.Moderate);
            e.Graphics.DrawString("\u26A0  Para cambiar tu contrasena, verifica la actual.", f, b, 10, 7);
        };
        card.Controls.Add(warning);
        cardY += 42;

        card.Controls.Add(new Label { Text = "Contrasena actual", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 22;
        _currentPassBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(pad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        card.Controls.Add(_currentPassBox);
        cardY += 56;

        card.Controls.Add(new Label { Text = "Nueva contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 22;
        _newPassBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(pad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        card.Controls.Add(_newPassBox);
        cardY += 56;

        card.Controls.Add(new Label { Text = "Confirmar contrasena", Font = AppTheme.Small, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(pad, cardY) });
        cardY += 22;
        _confirmPassBox = new RoundedTextBox { PlaceholderText = "--------", Location = new Point(pad, cardY), Size = new Size(fw, 44), BackColor = AppTheme.InputBg, ForeColor = AppTheme.Foreground, UseSystemPasswordChar = true };
        card.Controls.Add(_confirmPassBox);
        cardY += 60;

        _saveBtn = new RoundedButton { Text = "\U0001F4BE  Guardar Cambios", Location = new Point(pad, cardY), Size = new Size(fw, 48), CornerRadius = 14, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(14f) };
        card.Controls.Add(_saveBtn);

        content.Controls.Add(card);
        Controls.Add(content);
    }

    protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); e.Graphics.Clear(AppTheme.Background); }
}
