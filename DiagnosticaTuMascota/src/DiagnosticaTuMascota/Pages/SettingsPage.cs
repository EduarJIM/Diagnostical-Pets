using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

/// <summary>
/// Configuración de cuenta (Settings.tsx): perfil (nombre + correo fijo),
/// seguridad de contraseña y cierre de sesión.
/// </summary>
public sealed class SettingsPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PageBody body = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel root = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private Panel header = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel headerIcon = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel subtitle = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton logoutBtn = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel grid = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel profileCard = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel pl = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel emailNote = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton saveProfile = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel securityCard = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel sl = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel warn = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel warnLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton saveSecurity = null!;
    private readonly INavigator _nav;
    private readonly RoundedTextBox _name;
    private readonly RoundedTextBox _email;
    private readonly RoundedTextBox _currentPass;
    private readonly RoundedTextBox _newPass;
    private readonly RoundedTextBox _confirmPass;
    private UserAccount? _user;

    public SettingsPage(INavigator nav)
    {
        _nav = nav;
        body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 1100 };
        Controls.Add(body);

        root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            AutoSize = true,
            Width = 1060
        };

        // Encabezado
        header = new Panel { Height = 96, Width = 1060, BackColor = Color.Transparent };
        headerIcon = new ThemeLabel { TextKind = TextKind.Body, Text = "⚙️", Font = AppTheme.Emoji(16f) };
        title = new ThemeLabel { TextKind = TextKind.Display, Text = "  Configuración de Cuenta" };
        subtitle = new ThemeLabel { TextKind = TextKind.Body, Text = "Administra tu perfil, seguridad y preferencias." };
        logoutBtn = new RoundedButton
        {
            Variant = ButtonVariant.Destructive,
            Text = "Cerrar Sesión",
            Icon = "🚪",
            IconSize = 9f,
            Size = new Size(170, 42),
            Font = AppTheme.Medium(9.5f)
        };
        logoutBtn.Click += (_, _) => _nav.Logout();
        header.Controls.Add(headerIcon);
        header.Controls.Add(title);
        header.Controls.Add(subtitle);
        header.Controls.Add(logoutBtn);
        header.Resize += (_, _) =>
        {
            headerIcon.Location = new Point(0, 2);
            title.Location = new Point(34, 0);
            subtitle.Location = new Point(34, 40);
            logoutBtn.Location = new Point(header.Width - logoutBtn.Width, 26);
        };
        root.Controls.Add(header);

        // Dos columnas
        grid = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, Width = 1060, Height = 540, BackColor = Color.Transparent };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        // ---- Tarjeta Perfil ----
        profileCard = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 18,
            BorderColor = AppTheme.Border,
            Dock = DockStyle.Fill,
            Padding = new Padding(26, 20, 26, 20),
            Margin = new Padding(0, 0, 12, 0)
        };
        pl = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        pl.Controls.Add(new ThemeLabel { TextKind = TextKind.Heading, Text = "👤  Información del Perfil" });
        pl.Controls.Add(BuildFieldLabel("Nombre completo"));
        _name = new RoundedTextBox { IconText = "👤", PlaceholderText = "Tu nombre", Width = 460, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        pl.Controls.Add(_name);
        pl.Controls.Add(BuildFieldLabel("Correo electrónico"));
        _email = new RoundedTextBox { IconText = "✉️", Width = 460, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        _email.Enabled = false;
        pl.Controls.Add(_email);
        emailNote = new ThemeLabel
        {
            TextKind = TextKind.TinyMuted,
            Text = "El correo es el identificador de tu cuenta y no puede cambiarse.",
            MaximumSize = new Size(460, 30),
            AutoEllipsis = true,
            Margin = new Padding(0, 2, 0, 4)
        };
        pl.Controls.Add(emailNote);
        saveProfile = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Guardar Cambios", Icon = "💾", IconSize = 9f,
            Size = new Size(170, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(0, 10, 0, 0)
        };
        saveProfile.Click += (_, _) => SaveProfile();
        pl.Controls.Add(saveProfile);
        profileCard.Controls.Add(pl);
        grid.Controls.Add(profileCard, 0, 0);

        // ---- Tarjeta Seguridad ----
        securityCard = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 18,
            BorderColor = AppTheme.Border,
            Dock = DockStyle.Fill,
            Padding = new Padding(26, 20, 26, 20),
            Margin = new Padding(12, 0, 0, 0)
        };
        sl = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        sl.Controls.Add(new ThemeLabel { TextKind = TextKind.Heading, Text = "🔒  Seguridad de la Contraseña" });

        warn = new RoundedPanel
        {
            FillColor = AppTheme.WithAlpha(AppTheme.SeverityModerate, 12),
            CornerRadius = 12,
            BorderColor = AppTheme.WithAlpha(AppTheme.SeverityModerate, 60),
            Width = 460,
            Height = 74,
            Margin = new Padding(0, 6, 0, 10)
        };
        warnLabel = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = "⚠️  Para cambiar tu contraseña debes escribir primero la contraseña actual, y las nuevas deben coincidir.",
            MaximumSize = new Size(430, 50),
            AutoEllipsis = true,
            Location = new Point(14, 12)
        };
        warn.Controls.Add(warnLabel);
        sl.Controls.Add(warn);

        sl.Controls.Add(BuildFieldLabel("Contraseña actual"));
        _currentPass = new RoundedTextBox { IconText = "🔒", PlaceholderText = "••••••••", Width = 460, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        _currentPass.PasswordChar = '•';
        sl.Controls.Add(_currentPass);
        sl.Controls.Add(BuildFieldLabel("Nueva contraseña"));
        _newPass = new RoundedTextBox { IconText = "🔑", PlaceholderText = "••••••••", Width = 460, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        _newPass.PasswordChar = '•';
        sl.Controls.Add(_newPass);
        sl.Controls.Add(BuildFieldLabel("Confirmar nueva contraseña"));
        _confirmPass = new RoundedTextBox { IconText = "🔄", PlaceholderText = "••••••••", Width = 460, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        _confirmPass.PasswordChar = '•';
        sl.Controls.Add(_confirmPass);
        saveSecurity = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Actualizar Contraseña", Icon = "🔑", IconSize = 9f,
            Size = new Size(190, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(0, 10, 0, 0)
        };
        saveSecurity.Click += (_, _) => SaveSecurity();
        sl.Controls.Add(saveSecurity);
        securityCard.Controls.Add(sl);
        grid.Controls.Add(securityCard, 1, 0);

        root.Controls.Add(grid);
        body.Controls.Add(root);
    }

    private static ThemeLabel BuildFieldLabel(string text) => new()
    {
        TextKind = TextKind.MutedSmall,
        Text = text,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 10, 0, 3)
    };

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        _user = _nav.Storage.GetCurrentUser();
        if (_user is not null)
        {
            _name.Text = _user.Name;
            _email.Text = _user.Email;
        }
        _currentPass.Text = "";
        _newPass.Text = "";
        _confirmPass.Text = "";
    }

    private void SaveProfile()
    {
        if (_user is null) return;
        if (string.IsNullOrWhiteSpace(_name.Text))
        {
            _nav.ShowToast("El nombre no puede estar vacío", ToastKind.Error);
            return;
        }
        var users = _nav.Storage.GetUsers();
        var user = AuthService.FindByEmail(users, _user.Email);
        if (user is null) return;
        user.Name = _name.Text.Trim();
        _nav.Storage.SaveUsers(users);
        _user.Name = user.Name;
        _nav.Storage.SetCurrentUser(user);
        _nav.ShowToast("Perfil actualizado con éxito", ToastKind.Success);
    }

    private void SaveSecurity()
    {
        if (_user is null) return;
        var users = _nav.Storage.GetUsers();
        var user = AuthService.FindByEmail(users, _user.Email);
        if (user is null) return;

        bool wantsChange = !string.IsNullOrWhiteSpace(_newPass.Text);
        if (wantsChange)
        {
            if (!string.Equals(_currentPass.Text, user.Password, StringComparison.Ordinal))
            {
                _nav.ShowToast("La contraseña actual es incorrecta", ToastKind.Error);
                return;
            }
            if (!AuthService.PasswordsMatch(_newPass.Text, _confirmPass.Text))
            {
                _nav.ShowToast("Las nuevas contraseñas no coinciden", ToastKind.Error);
                return;
            }
            user.Password = _newPass.Text;
        }
        else if (!string.IsNullOrWhiteSpace(_currentPass.Text) || !string.IsNullOrWhiteSpace(_confirmPass.Text))
        {
            _nav.ShowToast("Ingresa la nueva contraseña y su confirmación", ToastKind.Error);
            return;
        }
        else
        {
            _nav.ShowToast("No hay cambios pendientes", ToastKind.Info);
            return;
        }

        _nav.Storage.SaveUsers(users);
        _nav.Storage.SetCurrentUser(user);
        _currentPass.Text = "";
        _newPass.Text = "";
        _confirmPass.Text = "";
        _nav.ShowToast("¡Contraseña actualizada con éxito!", ToastKind.Success);
    }
}