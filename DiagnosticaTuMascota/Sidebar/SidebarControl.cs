using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Sidebar;

public class SidebarControl : UserControl
{
    private readonly List<NavButton> _navButtons = new();
    private NavButton? _activeButton;
    private ToggleSwitch _themeToggle = null!;

    public event EventHandler<string>? NavigationChanged;
    public event EventHandler? ThemeToggled;
    public event EventHandler? LogoutClicked;

    public SidebarControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Width = 256;
        Dock = DockStyle.Left;
        BackColor = AppTheme.SidebarBg;
        Padding = new Padding(0);
        DoubleBuffered = true;

        InitializeContent();
    }

    private void InitializeContent()
    {
        // Main layout panel
        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };
        mainPanel.Paint += MainPanel_Paint;

        // Logo section
        var logoPanel = new Panel
        {
            Height = 80,
            Dock = DockStyle.Top,
            BackColor = Color.Transparent,
            Padding = new Padding(24, 20, 24, 10)
        };

        var logoLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            WrapContents = false
        };

        var logoIcon = new Label
        {
            Text = "\U0001FA7A",
            Font = new Font("Segoe UI Symbol", 20f),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(36, 36),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Primary,
            Margin = new Padding(0, 0, 12, 0)
        };
        logoIcon.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new(0, 0, 36, 36);
            using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, 10);
            logoIcon.Region = new Region(path);
            using LinearGradientBrush brush = new(bounds, AppTheme.Primary, AppTheme.Secondary, LinearGradientMode.ForwardDiagonal);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawString("\U0001FA7A", new Font("Segoe UI Symbol", 16f), Brushes.White, 0, 5);
        };

        var logoText = new Panel
        {
            BackColor = Color.Transparent,
            AutoSize = true,
            MinimumSize = new Size(100, 36),
            Padding = new Padding(0, 2, 0, 0)
        };
        logoText.Paint += (s, e) =>
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString("Diagnostica", new Font("Segoe UI", 13f, FontStyle.Bold), new SolidBrush(AppTheme.Foreground), 0, 0);
            e.Graphics.DrawString("tu Mascota", new Font("Segoe UI", 8f), new SolidBrush(AppTheme.MutedForeground), 0, 18);
        };

        _themeToggle = new ToggleSwitch
        {
            Dock = DockStyle.Right,
            Margin = new Padding(8, 8, 0, 0),
            BackColor = Color.Transparent
        };
        _themeToggle.Toggled += (s, e) => ThemeToggled?.Invoke(this, EventArgs.Empty);

        logoLayout.Controls.Add(logoIcon);
        logoLayout.Controls.Add(logoText);
        logoPanel.Controls.Add(logoLayout);
        logoPanel.Controls.Add(_themeToggle);

        // Navigation section
        var navPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(16, 10, 16, 0)
        };

        // Add nav buttons
        string[] navItems = new[]
        {
            "Dashboard|Mis Mascota|Analisis de Sintomas|Historial"
        };
        string[] navIcons = new[]
        {
            "\U0001F3E0|\U0001F43E|\U0001FA7A|\U0001F553"
        };
        string[] navKeys = new[]
        {
            "dashboard|pets|symptoms|history"
        };

        var navItemsArr = navItems[0].Split('|');
        var navIconsArr = navIcons[0].Split('|');
        var navKeysArr = navKeys[0].Split('|');

        var navFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            BackColor = Color.Transparent,
            WrapContents = false,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        for (int i = 0; i < navItemsArr.Length; i++)
        {
            var btn = new NavButton(navKeysArr[i], navIconsArr[i], navItemsArr[i]);
            btn.Width = 224;
            btn.Height = 44;
            btn.Margin = new Padding(0, 0, 0, 4);
            btn.Click += NavButton_Click;
            navFlow.Controls.Add(btn);
            _navButtons.Add(btn);
        }

        navPanel.Controls.Add(navFlow);

        // Bottom section
        var bottomPanel = new Panel
        {
            Height = 120,
            Dock = DockStyle.Bottom,
            BackColor = Color.Transparent,
            Padding = new Padding(16, 12, 16, 16)
        };
        bottomPanel.Paint += (s, e) =>
        {
            using Pen sepPen = new(AppTheme.Border, 1);
            e.Graphics.DrawLine(sepPen, 0, 0, bottomPanel.Width, 0);
        };

        var bottomFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = false,
            BackColor = Color.Transparent,
            WrapContents = false,
            Padding = new Padding(8, 0, 8, 0)
        };

        var settingsBtn = new NavButton("settings", "\u2699", "Configuracion");
        settingsBtn.Width = 224;
        settingsBtn.Height = 40;
        settingsBtn.Margin = new Padding(0, 4, 0, 4);
        settingsBtn.Click += NavButton_Click;
        _navButtons.Add(settingsBtn);

        var logoutBtn = new NavButton("logout", "\u2192", "Cerrar Sesion");
        logoutBtn.Width = 224;
        logoutBtn.Height = 40;
        logoutBtn.Margin = new Padding(0, 0, 0, 0);
        logoutBtn.IsLogout = true;
        logoutBtn.Click += (s, e) => LogoutClicked?.Invoke(this, EventArgs.Empty);

        bottomFlow.Controls.Add(settingsBtn);
        bottomFlow.Controls.Add(logoutBtn);

        bottomPanel.Controls.Add(bottomFlow);

        mainPanel.Controls.Add(navPanel);
        mainPanel.Controls.Add(logoPanel);
        mainPanel.Controls.Add(bottomPanel);

        Controls.Add(mainPanel);
    }

    private void MainPanel_Paint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Right border
        using Pen pen = new(AppTheme.Border, 1);
        g.DrawLine(pen, Width - 1, 0, Width - 1, Height);
    }

    private void NavButton_Click(object? sender, EventArgs e)
    {
        if (sender is NavButton btn)
        {
            SetActive(btn);
            NavigationChanged?.Invoke(this, btn.NavKey);
        }
    }

    public void SetActive(string navKey)
    {
        var btn = _navButtons.FirstOrDefault(b => b.NavKey == navKey);
        if (btn != null) SetActive(btn);
    }

    private void SetActive(NavButton btn)
    {
        _activeButton?.SetActive(false);
        btn.SetActive(true);
        _activeButton = btn;
    }

    public void RefreshTheme()
    {
        BackColor = AppTheme.SidebarBg;
        Invalidate(true);
    }
}

public class NavButton : Control
{
    public string NavKey { get; }
    public string Icon { get; }
    public string Label { get; }
    public bool IsActive { get; private set; }
    public bool IsLogout { get; set; }

    private bool _isHovered;

    public NavButton(string navKey, string icon, string label)
    {
        NavKey = navKey;
        Icon = icon;
        Label = label;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        Rectangle bounds = new(0, 0, Width, Height);

        // Background
        if (IsActive)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, 12);
            using SolidBrush brush = new(AppTheme.Primary);
            g.FillPath(brush, path);
        }
        else if (_isHovered)
        {
            using GraphicsPath path = AppTheme.GetRoundedRectPath(bounds, 12);
            using SolidBrush brush = new(AppTheme.SidebarAccent);
            g.FillPath(brush, path);
        }

        // Icon
        Color iconColor = IsLogout
            ? AppTheme.DestructiveCurrent
            : IsActive ? Color.White : AppTheme.MutedForeground;

        using Font iconFont = new("Segoe UI Symbol", 14f);
        using SolidBrush iconBrush = new(iconColor);
        g.DrawString(Icon, iconFont, iconBrush, 16, (Height - 18) / 2);

        // Label
        Color textColor = IsLogout
            ? AppTheme.DestructiveCurrent
            : IsActive ? Color.White : AppTheme.MutedForeground;

        using Font labelFont = new("Segoe UI", 13f, FontStyle.Regular);
        using SolidBrush labelBrush = new(textColor);
        g.DrawString(Label, labelFont, labelBrush, 48, (Height - 18) / 2);
    }
}
