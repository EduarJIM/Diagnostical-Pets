using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

public sealed partial class LoginPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private CenteredHost host = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel card = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel layout = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PulsingIconBox logo = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel subtitle = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel emailLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel passLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel forgot = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton loginBtn = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel footer = null!;
    private INavigator _nav = null!;
    private RoundedTextBox _email = null!;
    private RoundedTextBox _password = null!;

    public LoginPage(INavigator nav)
    {
        _nav = nav;
        InitializeComponent();
    }

    /// <summary>
    /// Constructor solo para el Diseñador de Visual Studio.
    /// Usa un navegador y un almacenamiento en memoria, así la página
    /// se ve en el diseño exactamente igual que en ejecución.
    /// </summary>
    public LoginPage() : this(DesignTimeNavigator.Instance)
    {
        Size = DesignTime.PageCanvas;
        DesignTime.PrimeLayout(this);
    }

    private void InitializeComponent()
    {
        host = new CenteredHost { Dock = DockStyle.Fill };
        Controls.Add(host);

        card = new RoundedPanel { FillColor = AppTheme.Card, CornerRadius = 24, Size = new Size(440, 620), Anchor = AnchorStyles.None };
        host.Controls.Add(card);

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(36, 30, 36, 30),
            BackColor = Color.Transparent
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 0 logo
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 1 título
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 2 subtítulo
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); // 3 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 4 etiqueta correo
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 5 campo correo  (era Absolute 6 → recortaba el campo)
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 6 etiqueta contraseña
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 7 campo contraseña
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 8 "¿Olvidaste tu contraseña?"
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24)); // 9 separador
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 10 botón iniciar sesión
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 6));  // 11 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // 12 pie

        logo = new PulsingIconBox { IconText = "🩺", Size = new Size(64, 64), CornerRadius = 14, IconFontSize = 20f, Anchor = AnchorStyles.None };
        layout.Controls.Add(logo, 0, 0);

        title = new ThemeLabel { TextKind = TextKind.Title, Text = "Bienvenido de nuevo", Anchor = AnchorStyles.None };
        layout.Controls.Add(title, 0, 1);

        subtitle = new ThemeLabel { TextKind = TextKind.Muted, Text = "Ingresa a tu cuenta de Diagnostica tu Mascota", Anchor = AnchorStyles.None, AutoEllipsis = true, MaximumSize = new Size(340, 40) };
        layout.Controls.Add(subtitle, 0, 2);

        // Correo
        emailLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Correo electrónico", Dock = DockStyle.Fill };
        layout.Controls.Add(emailLabel, 0, 4);

        _email = MakeField("✉️", "usuario@ejemplo.com");
        layout.Controls.Add(_email, 0, 5);

        // Contraseña
        passLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Contraseña", Dock = DockStyle.Fill };
        layout.Controls.Add(passLabel, 0, 6);

        _password = MakeField("🔒", "••••••••");
        _password.PasswordChar = '•';
        layout.Controls.Add(_password, 0, 7);

        forgot = new ThemeLabel { TextKind = TextKind.PrimarySmall, Text = "¿Olvidaste tu contraseña?", Anchor = AnchorStyles.Right, Cursor = Cursors.Hand };
        forgot.Click += (_, _) => _nav.Navigate(AppPage.ForgotPassword);
        layout.Controls.Add(forgot, 0, 8);

        loginBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Iniciar Sesión",
            Icon = "🚪",
            IconSize = 10f,
            Dock = DockStyle.Fill,
            Font = AppTheme.Medium(10.5f),
            Height = 46
        };
        loginBtn.Click += (_, _) => DoLogin();
        layout.Controls.Add(loginBtn, 0, 10);

        footer = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = "¿No tienes una cuenta?  Regístrate aquí",
            Anchor = AnchorStyles.None,
            Cursor = Cursors.Hand
        };
        footer.Click += (_, _) => _nav.Navigate(AppPage.Register);
        layout.Controls.Add(footer, 0, 12);

        card.Controls.Add(layout);
    }

    private RoundedTextBox MakeField(string icon, string placeholder)
    {
        var field = new RoundedTextBox
        {
            FieldHeight = 44, // asignado antes que Height para no aplastar el alto
            IconText = icon,
            PlaceholderText = placeholder,
            Width = 368,
            Height = 44,
            Anchor = AnchorStyles.None
        };
        field.Inner.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; DoLogin(); }
        };
        return field;
    }

    private void DoLogin()
    {
        var users = _nav.Storage.GetUsers();
        var failure = AuthService.LoginError(users, _email.Text, _password.Text);
        switch (failure)
        {
            case AuthService.LoginFailure.NoAccounts:
                _nav.ShowToast("No hay cuentas registradas. Regístrate primero", ToastKind.Error);
                return;
            case AuthService.LoginFailure.EmptyEmail:
                _nav.ShowToast("Escribe tu correo electrónico", ToastKind.Error);
                return;
            case AuthService.LoginFailure.EmptyPassword:
                _nav.ShowToast("Escribe tu contraseña", ToastKind.Error);
                return;
            case AuthService.LoginFailure.InvalidCredentials:
                _nav.ShowToast("Correo o contraseña incorrectos", ToastKind.Error);
                return;
        }

        var user = AuthService.FindByEmail(users, _email.Text);
        _nav.Storage.SetCurrentUser(user);
        _nav.ShowToast($"¡Bienvenido de nuevo, {user!.Name}!", ToastKind.Success);
        _nav.Navigate(AppPage.Dashboard);
    }
}

public sealed partial class RegisterPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private CenteredHost host = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel card = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel layout = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PulsingIconBox logo = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel subtitle = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton createBtn = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel footer = null!;
    private INavigator _nav = null!;
    private RoundedTextBox _name = null!;
    private RoundedTextBox _phone = null!;
    private RoundedTextBox _email = null!;
    private RoundedTextBox _password = null!;
    private RoundedTextBox _confirm = null!;

    public RegisterPage(INavigator nav)
    {
        _nav = nav;
        InitializeComponent();
    }

    /// <summary>
    /// Constructor solo para el Diseñador de Visual Studio.
    /// Usa un navegador y un almacenamiento en memoria, así la página
    /// se ve en el diseño exactamente igual que en ejecución.
    /// </summary>
    public RegisterPage() : this(DesignTimeNavigator.Instance)
    {
        Size = DesignTime.PageCanvas;
        DesignTime.PrimeLayout(this);
    }

    private void InitializeComponent()
    {
        host = new CenteredHost { Dock = DockStyle.Fill };
        Controls.Add(host);

        card = new RoundedPanel { FillColor = AppTheme.Card, CornerRadius = 24, Size = new Size(540, 600), Anchor = AnchorStyles.None };
        host.Controls.Add(card);

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(36, 26, 36, 26),
            BackColor = Color.Transparent
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        // Esquema de filas: todo el contenido en filas AutoSize (se mide con el tamaño
        // real de cada control) y separadores pequeños entre bloques.
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 0 logo (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 1 título (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 2 subtítulo (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18)); // 3 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 4 etiquetas nombre | teléfono
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 5 campos nombre | teléfono
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12)); // 6 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 7 etiqueta correo (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 8 campo correo (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12)); // 9 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 10 etiquetas contraseña | confirmar
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 11 campos contraseña | confirmar
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26)); // 12 separador
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // 13 botón crear cuenta (span 2)
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10)); // 14 separador
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // 15 pie (span 2)

        logo = new PulsingIconBox { IconText = "🩺", Size = new Size(64, 64), CornerRadius = 14, IconFontSize = 20f, Anchor = AnchorStyles.None };
        layout.Controls.Add(logo, 0, 0);
        layout.SetColumnSpan(logo, 2);

        title = new ThemeLabel { TextKind = TextKind.Title, Text = "Crea tu cuenta", Anchor = AnchorStyles.None };
        layout.Controls.Add(title, 0, 1);
        layout.SetColumnSpan(title, 2);

        subtitle = new ThemeLabel { TextKind = TextKind.Muted, Text = "Únete a la mejor comunidad de cuidado para mascotas", Anchor = AnchorStyles.None, AutoEllipsis = true, MaximumSize = new Size(440, 40) };
        layout.Controls.Add(subtitle, 0, 2);
        layout.SetColumnSpan(subtitle, 2);

        _name = AddField(layout, 4, 5, 0, "Nombre completo", "👤", "Juan Pérez");
        _phone = AddField(layout, 4, 5, 1, "Teléfono", "📞", "+1 234 567 890");

        _email = AddField(layout, 7, 8, 0, "Correo electrónico", "✉️", "usuario@ejemplo.com", span2: true);

        _password = AddField(layout, 10, 11, 0, "Contraseña", "🔒", "••••••••", password: true);
        _confirm = AddField(layout, 10, 11, 1, "Confirmar contraseña", "🔒", "••••••••", password: true);

        createBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Crear Cuenta",
            Icon = "👥",
            IconSize = 10f,
            Dock = DockStyle.Fill,
            Font = AppTheme.Medium(10.5f),
            Height = 46
        };
        createBtn.Click += (_, _) => DoRegister();
        layout.Controls.Add(createBtn, 0, 13);
        layout.SetColumnSpan(createBtn, 2);

        footer = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "¿Ya tienes una cuenta?  Inicia sesión", Anchor = AnchorStyles.None, Cursor = Cursors.Hand };
        footer.Click += (_, _) => _nav.Navigate(AppPage.Login);
        layout.Controls.Add(footer, 0, 15);
        layout.SetColumnSpan(footer, 2);

        card.Controls.Add(layout);
    }

    /// <summary>Agrega etiqueta + campo en una fila (o columna) del layout.</summary>
    private RoundedTextBox AddField(TableLayoutPanel layout, int labelRow, int fieldRow, int column, string label,
        string icon, string placeholder, bool span2 = false, bool password = false)
    {
        var lbl = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = label, Dock = DockStyle.Fill };
        layout.Controls.Add(lbl, column, labelRow);
        if (span2) layout.SetColumnSpan(lbl, 2);

        var field = new RoundedTextBox
        {
            FieldHeight = 42, // asignado antes que Height para no aplastar el alto
            IconText = icon,
            PlaceholderText = placeholder,
            Width = span2 ? 440 : 220,
            Height = 42,
            Anchor = AnchorStyles.None
        };
        if (password) field.PasswordChar = '•';
        layout.Controls.Add(field, column, fieldRow);
        if (span2) layout.SetColumnSpan(field, 2);
        return field;
    }

    private void DoRegister()
    {
        if (string.IsNullOrWhiteSpace(_name.Text) || string.IsNullOrWhiteSpace(_email.Text) ||
            string.IsNullOrWhiteSpace(_phone.Text) || string.IsNullOrWhiteSpace(_password.Text))
        {
            _nav.ShowToast("Completa todos los campos", ToastKind.Error);
            return;
        }
        if (!AuthService.PasswordsMatch(_password.Text, _confirm.Text))
        {
            _nav.ShowToast("Las contraseñas no coinciden", ToastKind.Error);
            return;
        }
        var users = _nav.Storage.GetUsers();
        if (AuthService.IsEmailRegistered(users, _email.Text))
        {
            _nav.ShowToast("Este correo ya está registrado", ToastKind.Error);
            return;
        }

        var newUser = new UserAccount
        {
            Name = _name.Text.Trim(),
            Email = _email.Text.Trim(),
            Phone = _phone.Text.Trim(),
            Password = _password.Text
        };
        users.Add(newUser);
        _nav.Storage.SaveUsers(users);
        _nav.Storage.SetCurrentUser(newUser);
        _nav.ShowToast("¡Cuenta creada exitosamente!", ToastKind.Success);
        _nav.Navigate(AppPage.Dashboard);
    }
}

public sealed partial class ForgotPasswordPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private CenteredHost host = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedPanel card = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private TableLayoutPanel layout = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel back = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PulsingIconBox logo = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel phoneNote = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel step3 = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel newLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel confirmLabel = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton submit = null!;
    private INavigator _nav = null!;
    private Panel _stepHost = null!;
    private RoundedTextBox _emailField = null!;
    private RoundedTextBox _phoneField = null!;
    private RoundedTextBox _newPass = null!;
    private RoundedTextBox _confirmPass = null!;
    private ProgressIndicator _progress = null!;
    private ThemeLabel _subtitle = null!;
    private int _step = 1;

    public ForgotPasswordPage(INavigator nav)
    {
        _nav = nav;
        InitializeComponent();
    }

    /// <summary>
    /// Constructor solo para el Diseñador de Visual Studio.
    /// Usa un navegador y un almacenamiento en memoria, así la página
    /// se ve en el diseño exactamente igual que en ejecución.
    /// </summary>
    public ForgotPasswordPage() : this(DesignTimeNavigator.Instance)
    {
        Size = DesignTime.PageCanvas;
        DesignTime.PrimeLayout(this);
    }

    private void InitializeComponent()
    {
        host = new CenteredHost { Dock = DockStyle.Fill };
        Controls.Add(host);

        card = new RoundedPanel { FillColor = AppTheme.Card, CornerRadius = 24, Size = new Size(440, 600), Anchor = AnchorStyles.None };
        host.Controls.Add(card);

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(36, 26, 36, 26),
            BackColor = Color.Transparent
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Volver al inicio
        back = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "←  Volver al inicio de sesión", Anchor = AnchorStyles.Left, Cursor = Cursors.Hand };
        back.Click += (_, _) => _nav.Navigate(AppPage.Login);
        layout.Controls.Add(back, 0, 0);

        logo = new PulsingIconBox { IconText = "🔑", Size = new Size(64, 64), CornerRadius = 14, IconFontSize = 20f, Anchor = AnchorStyles.None };
        layout.Controls.Add(logo, 0, 2);

        title = new ThemeLabel { TextKind = TextKind.Title, Text = "Restablecer Contraseña", Anchor = AnchorStyles.None };
        layout.Controls.Add(title, 0, 3);

        _subtitle = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Ingresa tu correo para comenzar el proceso", Anchor = AnchorStyles.None, AutoEllipsis = true, MaximumSize = new Size(350, 30) };
        layout.Controls.Add(_subtitle, 0, 4);

        _progress = new ProgressIndicator { Height = 8, Width = 120, Step = 1, Anchor = AnchorStyles.None };
        layout.Controls.Add(_progress, 0, 5);

        _stepHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        layout.Controls.Add(_stepHost, 0, 6);

        // ---- Paso 1 ----
        _emailField = BuildStepField("✉️", "usuario@ejemplo.com");
        var step1 = BuildStepPanel(_emailField, "Continuar", "➡️", DoStep1);
        _stepHost.Controls.Add(step1);

        // ---- Paso 2 ----
        _phoneField = BuildStepField("📞", "+1 234 567 890");
        phoneNote = new ThemeLabel { TextKind = TextKind.TinyMuted, Text = "Por seguridad, requerimos el teléfono registrado en tu cuenta.", MaximumSize = new Size(360, 26), Anchor = AnchorStyles.Left };
        var step2 = BuildStepPanelWithNote(_phoneField, "Verificar Identidad", DoStep2, phoneNote);
        _stepHost.Controls.Add(step2);

        // ---- Paso 3 ----
        _newPass = BuildStepField("🔒", "••••••••");
        _newPass.PasswordChar = '•';
        _confirmPass = BuildStepField("🔄", "••••••••");
        _confirmPass.PasswordChar = '•';

        step3 = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 0, 0, 4)
        };
        newLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Nueva Contraseña", Dock = DockStyle.Top, Anchor = AnchorStyles.Left };
        confirmLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Confirmar Nueva Contraseña", Dock = DockStyle.Top, Anchor = AnchorStyles.Left };
        submit = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Actualizar Contraseña", Icon = "🔑", IconSize = 9f,
            Height = 46, Width = 368, Anchor = AnchorStyles.None, Font = AppTheme.Medium(10.5f)
        };
        submit.Click += (_, _) => DoStep3();
        step3.Controls.Add(newLabel);
        step3.Controls.Add(_newPass);
        step3.Controls.Add(confirmLabel);
        step3.Controls.Add(_confirmPass);
        _confirmPass.Margin = new Padding(0, 0, 0, 18);
        step3.Controls.Add(submit);
        _stepHost.Controls.Add(step3);

        ShowStep(1);
        card.Controls.Add(layout);
    }

    private RoundedTextBox BuildStepField(string icon, string placeholder)
    {
        return new RoundedTextBox
        {
            FieldHeight = 44, // asignado antes que Height para no aplastar el alto
            IconText = icon,
            PlaceholderText = placeholder,
            Width = 368,
            Height = 44,
            Anchor = AnchorStyles.None
        };
    }

    private FlowLayoutPanel BuildStepPanel(RoundedTextBox field, string buttonText, string buttonIcon, Action action)
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        var label = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "Correo electrónico", Dock = DockStyle.Top, Anchor = AnchorStyles.Left };
        var btn = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = buttonText, Icon = buttonIcon, IconSize = 9f,
            Height = 46, Width = 368, Anchor = AnchorStyles.None, Font = AppTheme.Medium(10.5f)
        };
        btn.Click += (_, _) => action();
        field.Inner.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; action(); } };
        panel.Controls.Add(label);
        panel.Controls.Add(field);
        panel.Controls.Add(btn);
        return panel;
    }

    private FlowLayoutPanel BuildStepPanelWithNote(RoundedTextBox field, string buttonText, Action action, ThemeLabel note)
    {
        var panel = BuildStepPanel(field, buttonText, "🛡️", action);
        panel.Controls.RemoveAt(panel.Controls.Count - 1); // quitar botón
        panel.Controls.Add(note);
        panel.Controls.Add(new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = buttonText, Icon = "🛡️", IconSize = 9f,
            Height = 46, Width = 368, Anchor = AnchorStyles.None, Font = AppTheme.Medium(10.5f)
        } as Control);
        return panel;
    }

    private void DoStep1()
    {
        var users = _nav.Storage.GetUsers();
        if (AuthService.FindByEmail(users, _emailField.Text) is not null) ShowStep(2);
        else _nav.ShowToast("No se encontró ninguna cuenta con ese correo", ToastKind.Error);
    }

    private void DoStep2()
    {
        var users = _nav.Storage.GetUsers();
        if (AuthService.VerifyIdentity(users, _emailField.Text, _phoneField.Text)) ShowStep(3);
        else _nav.ShowToast("El número de teléfono no coincide con nuestros registros", ToastKind.Error);
    }

    private void DoStep3()
    {
        if (!AuthService.PasswordsMatch(_newPass.Text, _confirmPass.Text))
        {
            _nav.ShowToast("Las contraseñas no coinciden", ToastKind.Error);
            return;
        }
        var users = _nav.Storage.GetUsers();
        var user = AuthService.FindByEmail(users, _emailField.Text);
        if (user is null) { _nav.ShowToast("Cuenta no encontrada", ToastKind.Error); return; }
        user.Password = _newPass.Text;
        _nav.Storage.SaveUsers(users);
        _nav.ShowToast("¡Contraseña restablecida con éxito!", ToastKind.Success);
        _nav.Navigate(AppPage.Login);
    }

    private void ShowStep(int step)
    {
        _step = step;
        _progress.Step = step;
        _subtitle.Text = step switch
        {
            1 => "Ingresa tu correo para comenzar el proceso",
            2 => "Verifica tu identidad con tu número de teléfono",
            _ => "Ingresa tu nueva contraseña"
        };
        int i = 0;
        foreach (Control c in _stepHost.Controls)
        {
            c.Visible = i == step - 1;
            i++;
        }
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        ShowStep(1);
    }
}

/// <summary>Indicador de progreso de 3 puntos (activo = barra ancha primary).</summary>
public sealed class ProgressIndicator : Control
{
    private int _step = 1;

    public ProgressIndicator()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // sin caja gris detrás de las barras
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public int Step
    {
        get => _step;
        set { _step = Math.Clamp(value, 1, 3); Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        int totalSegments = 3;
        int gap = 8;
        for (int i = 0; i < totalSegments; i++)
        {
            bool active = i + 1 == _step;
            int w = active ? 36 : 8;
            int h = 6;
            // Reparte el ancho disponible
            int usable = Width - gap * (totalSegments - 1);
            int segW = usable / totalSegments;
            // Ajuste proporcional al modelo web: activo más ancho
            int actualW = active ? segW + 14 : segW - 6;
            actualW = Math.Max(8, actualW);
            int x = i * (segW + gap) + (segW - actualW) / 2 + 3;
            var rect = new Rectangle(x, (Height - h) / 2, actualW, h);
            using var path = DrawingHelpers.RoundedRect(rect, h / 2);
            using var brush = new SolidBrush(active ? AppTheme.Primary : AppTheme.Muted);
            g.FillPath(brush, path);
        }
    }
}