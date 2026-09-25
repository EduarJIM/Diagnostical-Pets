using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Pages;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota;

/// <summary>Configuración global persistida (tema claro/oscuro).</summary>
public sealed class UiSettings
{
    public bool IsDark { get; set; } = true;
}

/// <summary>
/// Ventana principal: barra lateral, navegación entre páginas, tema persistido,
/// toasts y el sistema de alarmas (revisión cada 5s como useAlarms.ts).
/// </summary>
public sealed class MainForm : Form, INavigator
{
    private readonly AppStorage _storage;
    private readonly SidebarControl _sidebar;
    private readonly Panel _content;
    private readonly Dictionary<AppPage, PageBase> _pages = new();
    private readonly System.Windows.Forms.Timer _alarmTimer;
    private readonly AlarmSoundPlayer _sound;
    private bool _alarmModalOpen;

    public MainForm(AppStorage storage)
    {
        _storage = storage;
        Text = "Diagnostica tu Mascota";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1120, 720);
        BackColor = AppTheme.Background;

        // Tema persistido
        var settings = _storage.GetData("uiSettings", new UiSettings());
        AppTheme.SetDark(settings.IsDark);
        AppTheme.ThemeChanged += (_, _) =>
        {
            BackColor = AppTheme.Background;
            _content.BackColor = AppTheme.Background;
        };

        _sidebar = new SidebarControl();
        _sidebar.NavigateRequested += p => Navigate(FromSidebar(p));
        _sidebar.LogoutRequested += Logout;
        _sidebar.DarkModeToggled += () =>
        {
            AppTheme.Toggle();
            SaveTheme();
        };

        _content = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background
        };

        Controls.Add(_content);
        Controls.Add(_sidebar);

        // Sistema de alarmas: revisión cada 5 segundos
        _alarmTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        _alarmTimer.Tick += (_, _) => CheckAlarms();

        _sound = new AlarmSoundPlayer();
    }

    public AppStorage Storage => _storage;

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _alarmTimer.Start();
        Navigate(_storage.GetCurrentUser() is null ? AppPage.Login : AppPage.Dashboard);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _alarmTimer.Stop();
        _sound.Stop();
        base.OnFormClosed(e);
    }

    // ---- Navegación ----

    private static AppPage FromSidebar(SidebarPage p) => p switch
    {
        SidebarPage.Pets => AppPage.Pets,
        SidebarPage.Symptoms => AppPage.SymptomAnalysis,
        SidebarPage.History => AppPage.History,
        SidebarPage.Settings => AppPage.Settings,
        _ => AppPage.Dashboard
    };

    private static SidebarPage SidebarPageFor(AppPage p) => p switch
    {
        AppPage.Pets => SidebarPage.Pets,
        AppPage.SymptomAnalysis or AppPage.ConsultationStep2 or AppPage.DiagnosticResult => SidebarPage.Symptoms,
        AppPage.History => SidebarPage.History,
        AppPage.Settings => SidebarPage.Settings,
        _ => SidebarPage.Dashboard
    };

    public void Navigate(AppPage page, NavState? state = null)
    {
        if (!_pages.TryGetValue(page, out var control))
        {
            control = CreatePage(page);
            _pages[page] = control;
        }

        _content.SuspendLayout();
        _content.Controls.Clear();
        _content.Controls.Add(control);
        _content.ResumeLayout();
        control.OnNavigatedTo(state);

        bool authed = page is not (AppPage.Login or AppPage.Register or AppPage.ForgotPassword);
        _sidebar.Visible = authed;
        if (authed) _sidebar.SetActive(SidebarPageFor(page));
        _content.BackColor = AppTheme.Background;
    }

    private PageBase CreatePage(AppPage page) => page switch
    {
        AppPage.Login => new LoginPage(this),
        AppPage.Register => new RegisterPage(this),
        AppPage.ForgotPassword => new ForgotPasswordPage(this),
        AppPage.Pets => new PetsPage(this),
        AppPage.SymptomAnalysis => new SymptomAnalysisPage(this),
        AppPage.ConsultationStep2 => new ConsultationStep2Page(this),
        AppPage.DiagnosticResult => new DiagnosticResultPage(this),
        AppPage.History => new HistoryPage(this),
        AppPage.Settings => new SettingsPage(this),
        _ => new DashboardPage(this)
    };

    public void ShowToast(string message, ToastKind kind = ToastKind.Info)
    {
        ToastService.Show(this, message, kind);
    }

    public void Logout()
    {
        _storage.SetCurrentUser(null);
        Navigate(AppPage.Login);
        ShowToast("Sesión cerrada", ToastKind.Info);
    }

    private void SaveTheme() => _storage.SetData("uiSettings", new UiSettings { IsDark = AppTheme.IsDark });

    // ---- Alarmas ----

    private void CheckAlarms()
    {
        if (_alarmModalOpen) return;
        var alerts = _storage.GetData("alertsData", DemoData.CreateInitialAlerts());
        if (!AlarmService.IsTimeToRing(alerts, DateTime.Now, out var due) || due is null) return;

        var modal = new ActiveAlarmModal(due);
        _alarmModalOpen = true;
        _sound.Start();
        try
        {
            ModalHost.ShowDialog(this, modal);
        }
        finally
        {
            _sound.Stop();
            _alarmModalOpen = false;
        }

        // Cualquier cierre elimina la alerta (comportamiento de la web).
        alerts.RemoveAll(a => a.Id == due.Id);
        _storage.SetData("alertsData", alerts);
    }
}

/// <summary>Reproductor de sonido de alarmas (pitidos con Console.Beep).</summary>
public sealed class AlarmSoundPlayer
{
    private System.Windows.Forms.Timer? _timer;
    private bool _high;

    public void Start()
    {
        if (_timer is null)
        {
            _timer = new System.Windows.Forms.Timer { Interval = 2400 };
            _timer.Tick += (_, _) => Beep();
        }
        Beep();
        _timer.Start();
    }

    public void Stop() => _timer?.Stop();

    private void Beep()
    {
        try
        {
            int freq = _high ? 980 : 760;
            _high = !_high;
            Console.Beep(freq, 150);
        }
        catch (InvalidOperationException)
        {
            // Sin dispositivo de sonido: continuar en silencio.
        }
        catch (Exception)
        {
            // Evita que un fallo de audio rompa la app.
        }
    }
}

/// <summary>
/// Modal de alarma activa (ActiveAlarmModal.tsx): icono pulsante, título de la
/// alerta y botón "Marcar como Hecho".
/// </summary>
public sealed class ActiveAlarmModal : ModalForm
{
    public ActiveAlarmModal(AlertItem alert)
    {
        Size = new Size(620, 520);
        StartPosition = FormStartPosition.CenterParent;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            BackColor = Color.Transparent,
            Padding = new Padding(40, 30, 40, 30)
        };
        for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var icon = new PulsingIconBox
        {
            IconText = "⏰",
            Size = new Size(96, 96),
            CornerRadius = 48,
            IconFontSize = 26f,
            GlowColor = AppTheme.SeverityModerate,
            Anchor = AnchorStyles.None
        };
        layout.Controls.Add(icon, 0, 0);

        var badge = new PillButton
        {
            Text = "¡Es la hora!",
            IconText = "🔔",
            Active = true,
            Width = 130,
            Height = 34,
            ActiveColor = AppTheme.SeverityModerate,
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 18, 0, 0)
        };
        layout.Controls.Add(badge, 0, 1);

        var title = new ThemeLabel { TextKind = TextKind.Title, Text = alert.Title, Anchor = AnchorStyles.None, Margin = new Padding(0, 10, 0, 0) };
        layout.Controls.Add(title, 0, 2);

        var pet = new ThemeLabel { TextKind = TextKind.Primary, Text = "🐾  Para: " + alert.Pet, Anchor = AnchorStyles.None, Margin = new Padding(0, 6, 0, 0) };
        layout.Controls.Add(pet, 0, 3);

        var msgBox = new RoundedPanel
        {
            FillColor = AppTheme.WithAlpha(AppTheme.SeverityModerate, 12),
            CornerRadius = 14,
            BorderColor = AppTheme.WithAlpha(AppTheme.SeverityModerate, 70),
            Width = 520,
            Height = 80,
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 14, 0, 0)
        };
        var msg = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = alert.Message,
            MaximumSize = new Size(480, 60),
            AutoEllipsis = true,
            Location = new Point(18, 14)
        };
        msgBox.Controls.Add(msg);
        layout.Controls.Add(msgBox, 0, 4);

        var done = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Marcar como Hecho",
            Icon = "✅",
            IconSize = 9f,
            Size = new Size(220, 46),
            Font = AppTheme.Medium(10f),
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 18, 0, 0)
        };
        done.Click += (_, _) => DialogResult = DialogResult.OK;
        layout.Controls.Add(done, 0, 5);

        Content.Controls.Add(layout);
    }
}