using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Sidebar;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms;

public partial class MainForm : Form
{
    private SidebarControl _sidebar = null!;
    private Panel _contentContainer = null!;
    private Panel _backgroundGlows = null!;
    private Control? _currentContent;

    public MainForm()
    {
        InitializeComponent();
        BuildUI();
    }

    private void BuildUI()
    {
        AppTheme.ApplyFormStyle(this, "Diagnostica tu Mascota");
        ClientSize = new Size(1280, 800);
        MinimumSize = new Size(1024, 700);
        StartPosition = FormStartPosition.CenterScreen;

        _backgroundGlows = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Enabled = false
        };
        _backgroundGlows.Paint += BackgroundGlows_Paint;
        Controls.Add(_backgroundGlows);

        _contentContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            AutoScroll = true
        };
        _contentContainer.Resize += ContentContainer_Resize;
        Controls.Add(_contentContainer);

        _sidebar = new SidebarControl
        {
            Dock = DockStyle.Left,
            Width = 256
        };
        _sidebar.NavigationChanged += Sidebar_NavigationChanged;
        _sidebar.ThemeToggled += Sidebar_ThemeToggled;
        _sidebar.LogoutClicked += Sidebar_LogoutClicked;
        Controls.Add(_sidebar);

        _contentContainer.BringToFront();
        _backgroundGlows.BringToFront();

        NavigateTo("dashboard");
    }

    private void ContentContainer_Resize(object? sender, EventArgs e)
    {
        if (_currentContent != null && _currentContent.IsHandleCreated)
        {
            _currentContent.Width = _contentContainer.ClientSize.Width;
        }
    }

    private void BackgroundGlows_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (GraphicsPath path = new())
        {
            path.AddEllipse(-100, -100, 500, 500);
            using PathGradientBrush brush = new(path);
            brush.CenterColor = Color.FromArgb(15, AppTheme.Primary);
            brush.SurroundColors = new[] { Color.Transparent };
            g.FillPath(brush, path);
        }

        using (GraphicsPath path = new())
        {
            path.AddEllipse(ClientSize.Width - 400, ClientSize.Height - 400, 500, 500);
            using PathGradientBrush brush = new(path);
            brush.CenterColor = Color.FromArgb(15, AppTheme.Secondary);
            brush.SurroundColors = new[] { Color.Transparent };
            g.FillPath(brush, path);
        }
    }

    private void Sidebar_NavigationChanged(object? sender, string navKey)
    {
        NavigateTo(navKey);
    }

    private void Sidebar_ThemeToggled(object? sender, EventArgs e)
    {
        AppTheme.IsDarkMode = !AppTheme.IsDarkMode;
        BackColor = AppTheme.Background;
        _sidebar.RefreshTheme();
        _contentContainer.BackColor = Color.Transparent;
        _backgroundGlows.Invalidate();
        NavigateTo(_currentContent?.Tag?.ToString() ?? "dashboard");
    }

    private void Sidebar_LogoutClicked(object? sender, EventArgs e)
    {
        var loginForm = new Login.LoginForm();
        loginForm.Show();
        Close();
    }

    public void NavigateTo(string navKey)
    {
        _currentContent?.Dispose();
        _contentContainer.Controls.Clear();

        Control content = navKey switch
        {
            "dashboard" => new Dashboard.DashboardControl(),
            "pets" => new Pets.PetsControl(),
            "symptoms" => new SymptomAnalysis.SymptomAnalysisControl(this),
            "consultation" => new SymptomAnalysis.ConsultationStep2Control(this),
            "result" => new DiagnosticResult.DiagnosticResultControl(this),
            "history" => new History.HistoryControl(),
            "settings" => new Settings.SettingsControl(),
            _ => new Dashboard.DashboardControl()
        };

        content.Tag = navKey;
        content.Dock = DockStyle.Top;
        content.Width = _contentContainer.ClientSize.Width;
        _currentContent = content;

        _contentContainer.Controls.Add(content);

        _sidebar.SetActive(navKey);
        _contentContainer.AutoScrollPosition = Point.Empty;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(AppTheme.Background);
    }
}
