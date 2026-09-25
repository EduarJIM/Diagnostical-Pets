using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

public enum SidebarPage
{
    Dashboard, Pets, Symptoms, History, Settings
}

/// <summary>
/// Barra lateral de navegación (Sidebar.tsx): logo animado, toggle de tema,
/// ítems de navegación e ítems de pie (Configuración / Cerrar Sesión).
/// </summary>
public class SidebarControl : UserControl
{
    private readonly NavItem _dashboard;
    private readonly NavItem _pets;
    private readonly NavItem _symptoms;
    private readonly NavItem _history;
    private readonly NavItem _settings;
    private readonly NavItem _logout;
    private readonly ToggleSwitch _toggle;

    public event Action<SidebarPage>? NavigateRequested;
    public event Action? LogoutRequested;
    public event Action? DarkModeToggled;

    public SidebarControl()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        Width = 256;
        Dock = DockStyle.Left;
        AppTheme.ThemeChanged += (_, _) => { _toggle.Checked = AppTheme.IsDark; Invalidate(); };

        // ---- Encabezado ----
        var header = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Color.Transparent };

        var logo = new PulsingIconBox { IconText = "🩺", Size = new Size(38, 38), CornerRadius = 10, IconFontSize = 15f };
        var name = new ThemeLabel { TextKind = TextKind.SubHeading, Text = "Diagnostica" };
        var sub = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "tu Mascota" };
        _toggle = new ToggleSwitch { Checked = AppTheme.IsDark };
        _toggle.CheckedChanged += (_, _) => DarkModeToggled?.Invoke();

        header.Controls.Add(logo);
        header.Controls.Add(name);
        header.Controls.Add(sub);
        header.Controls.Add(_toggle);
        header.Resize += (_, _) =>
        {
            logo.Location = new Point(20, 22);
            name.Location = new Point(logo.Right + 10, 20);
            sub.Location = new Point(logo.Right + 10, name.Bottom + 2);
            _toggle.Location = new Point(header.Width - _toggle.Width - 14, 26);
        };

        // ---- Navegación ----
        var nav = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16, 10, 16, 8),
            BackColor = Color.Transparent
        };

        _dashboard = BuildNavItem("🏠", "Dashboard", SidebarPage.Dashboard);
        _pets = BuildNavItem("🐾", "Mis Mascotas", SidebarPage.Pets);
        _symptoms = BuildNavItem("🩺", "Análisis de Síntomas", SidebarPage.Symptoms);
        _history = BuildNavItem("📜", "Historial", SidebarPage.History);

        nav.Controls.Add(_dashboard);
        nav.Controls.Add(_pets);
        nav.Controls.Add(_symptoms);
        nav.Controls.Add(_history);
        nav.Controls.Add(new Panel { Height = 6, BackColor = Color.Transparent });

        // ---- Pie ----
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 112, BackColor = Color.Transparent, Padding = new Padding(0, 10, 0, 10) };

        _settings = new NavItem { IconText = "⚙️", Text = "Configuración", Width = 224, Height = 44 };
        _settings.Click += (_, _) => NavigateRequested?.Invoke(SidebarPage.Settings);

        _logout = new NavItem
        {
            IconText = "🚪",
            Text = "Cerrar Sesión",
            Width = 224,
            Height = 44,
            AccentColor = AppTheme.Destructive
        };
        _logout.Click += (_, _) => LogoutRequested?.Invoke();

        footer.Controls.Add(_settings);
        footer.Controls.Add(_logout);
        footer.Resize += (_, _) =>
        {
            _settings.Location = new Point(16, 6);
            _logout.Location = new Point(16, _settings.Bottom + 4);
        };
        footer.Paint += (_, e) =>
        {
            using var pen = new Pen(AppTheme.Border, 1f);
            e.Graphics.DrawLine(pen, 0, 0, Width, 0);
        };

        // ---- Ensamble ----
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, BackColor = Color.Transparent };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        root.Controls.Add(nav, 0, 0);
        root.Controls.Add(footer, 0, 1);

        var shell = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        shell.Controls.Add(root);
        shell.Controls.Add(header);
        Controls.Add(shell);
    }

    private NavItem BuildNavItem(string icon, string label, SidebarPage page)
    {
        var item = new NavItem { IconText = icon, Text = label, Width = 224, Height = 44 };
        item.Click += (_, _) => NavigateRequested?.Invoke(page);
        return item;
    }

    public void SetActive(SidebarPage page)
    {
        _dashboard.Active = page == SidebarPage.Dashboard;
        _pets.Active = page == SidebarPage.Pets;
        _symptoms.Active = page == SidebarPage.Symptoms;
        _history.Active = page == SidebarPage.History;
        _settings.Active = page == SidebarPage.Settings;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        using var b = new SolidBrush(AppTheme.SidebarBg);
        e.Graphics.FillRectangle(b, ClientRectangle);
        using var pen = new Pen(AppTheme.Border, 1f);
        e.Graphics.DrawLine(pen, Width - 1, 0, Width - 1, Height);
    }
}