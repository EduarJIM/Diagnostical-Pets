using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

/// <summary>
/// Historial y seguimiento (History.tsx): tres pestañas — Consultas Anteriores,
/// Próximos Procesos y Alertas Activas — con tarjetas y modales.
/// </summary>
public sealed class HistoryPage : PageBase
{
    private readonly INavigator _nav;
    private readonly PillButton _tabHistory;
    private readonly PillButton _tabUpcoming;
    private readonly PillButton _tabAlerts;
    private readonly FlowLayoutPanel _historyList;
    private readonly FlowLayoutPanel _upcomingList;
    private readonly FlowLayoutPanel _alertsList;
    private readonly RoundedButton _actionBtn;
    private int _tab;

    public HistoryPage(INavigator nav)
    {
        _nav = nav;
        var body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 1080 };
        Controls.Add(body);

        var root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            AutoSize = true,
            Width = 1040
        };

        var header = new Panel { Height = 96, Width = 1040, BackColor = Color.Transparent };
        var headerIcon = new ThemeLabel { TextKind = TextKind.Body, Text = "📜", Font = AppTheme.Emoji(16f) };
        var title = new ThemeLabel { TextKind = TextKind.Display, Text = "  Historial y Seguimiento" };
        var subtitle = new ThemeLabel { TextKind = TextKind.Body, Text = "Consulta anteriores, procesos programados y alertas de cuidado para tus mascotas." };
        _actionBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Agregar Alerta",
            Icon = "➕",
            IconSize = 9f,
            Size = new Size(170, 42),
            Font = AppTheme.Medium(9.5f)
        };
        _actionBtn.Click += (_, _) => OpenAddModal();
        header.Controls.Add(headerIcon);
        header.Controls.Add(title);
        header.Controls.Add(subtitle);
        header.Controls.Add(_actionBtn);
        header.Resize += (_, _) =>
        {
            headerIcon.Location = new Point(0, 2);
            title.Location = new Point(34, 0);
            subtitle.Location = new Point(34, 40);
            _actionBtn.Location = new Point(header.Width - _actionBtn.Width, 26);
        };
        root.Controls.Add(header);

        // Pestañas
        var tabs = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
        _tabHistory = new PillButton { Text = "Consultas Anteriores", IconText = "📜", Width = 170, Height = 38, ActiveColor = AppTheme.SeverityLeve };
        _tabUpcoming = new PillButton { Text = "Próximos Procesos", IconText = "📅", Width = 160, Height = 38, ActiveColor = Color.FromArgb(59, 130, 246) };
        _tabAlerts = new PillButton { Text = "Alertas Activas", IconText = "🔔", Width = 150, Height = 38, ActiveColor = AppTheme.SeverityModerate };
        _tabHistory.Click += (_, _) => ShowTab(0);
        _tabUpcoming.Click += (_, _) => ShowTab(1);
        _tabAlerts.Click += (_, _) => ShowTab(2);
        tabs.Controls.Add(_tabHistory);
        tabs.Controls.Add(_tabUpcoming);
        tabs.Controls.Add(_tabAlerts);
        root.Controls.Add(tabs);

        var contentHost = new Panel { Width = 1040, Height = 600, AutoScroll = true, BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Left };

        _historyList = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Width = 1020, BackColor = Color.Transparent };
        _upcomingList = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Width = 1020, BackColor = Color.Transparent };
        _alertsList = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Width = 1020, BackColor = Color.Transparent };

        contentHost.Controls.Add(_historyList);
        contentHost.Controls.Add(_upcomingList);
        contentHost.Controls.Add(_alertsList);
        root.Controls.Add(contentHost);

        body.Controls.Add(root);

        ShowTab(0);
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        Reload();
    }

    private void Reload()
    {
        var history = _nav.Storage.GetData("historyData", DemoData.CreateInitialHistory());
        var upcoming = _nav.Storage.GetData("upcomingData", DemoData.CreateInitialUpcoming());
        var alerts = _nav.Storage.GetData("alertsData", DemoData.CreateInitialAlerts());

        _historyList.SuspendLayout();
        _historyList.Controls.Clear();
        foreach (var item in history)
        {
            var card = new HistoryCard(item);
            card.DetailClicked += () => OpenHistoryDetail(item);
            card.DeleteClicked += () =>
            {
                history.RemoveAll(h => h.Id == item.Id);
                _nav.Storage.SetData("historyData", history);
                _nav.ShowToast("Consulta eliminada", ToastKind.Info);
                Reload();
            };
            _historyList.Controls.Add(card);
        }
        _historyList.ResumeLayout();

        _upcomingList.SuspendLayout();
        _upcomingList.Controls.Clear();
        foreach (var item in upcoming)
        {
            var card = new UpcomingCard(item);
            card.DeleteClicked += () =>
            {
                upcoming.RemoveAll(u => u.Id == item.Id);
                _nav.Storage.SetData("upcomingData", upcoming);
                _nav.ShowToast("Proceso eliminado", ToastKind.Info);
                Reload();
            };
            _upcomingList.Controls.Add(card);
        }
        _upcomingList.ResumeLayout();

        _alertsList.SuspendLayout();
        _alertsList.Controls.Clear();
        foreach (var item in alerts)
        {
            var card = new AlertCard(item);
            card.DeleteClicked += () =>
            {
                alerts.RemoveAll(a => a.Id == item.Id);
                _nav.Storage.SetData("alertsData", alerts);
                _nav.ShowToast("Alerta eliminada", ToastKind.Info);
                Reload();
            };
            _alertsList.Controls.Add(card);
        }
        _alertsList.ResumeLayout();
    }

    private void ShowTab(int tab)
    {
        _tab = tab;
        _tabHistory.Active = tab == 0;
        _tabUpcoming.Active = tab == 1;
        _tabAlerts.Active = tab == 2;
        _historyList.Visible = tab == 0;
        _upcomingList.Visible = tab == 1;
        _alertsList.Visible = tab == 2;
        _actionBtn.Text = tab switch
        {
            1 => "Agregar Proceso",
            2 => "Agregar Alerta",
            _ => ""
        };
        _actionBtn.Visible = tab != 0;
        _actionBtn.Icon = "➕";
    }

    private void OpenAddModal()
    {
        var owner = FindForm();
        if (owner is null) return;
        if (_tab == 1)
        {
            var modal = new UpcomingModal(_nav.Storage);
            if (ModalHost.ShowDialog(owner, modal) == DialogResult.OK)
            {
                _nav.ShowToast("Proceso programado agregado", ToastKind.Success);
                Reload();
            }
        }
        else if (_tab == 2)
        {
            var modal = new AlertModal(_nav.Storage);
            if (ModalHost.ShowDialog(owner, modal) == DialogResult.OK)
            {
                _nav.ShowToast("Alerta programada correctamente", ToastKind.Success);
                Reload();
            }
        }
    }

    private void OpenHistoryDetail(HistoryItem item)
    {
        var owner = FindForm();
        if (owner is null) return;
        var modal = new HistoryDetailModal(item);
        ModalHost.ShowDialog(owner, modal);
    }
}

/// <summary>Tarjeta de una consulta del historial.</summary>
public sealed class HistoryCard : RoundedPanel
{
    public event Action? DetailClicked;
    public event Action? DeleteClicked;

    public HistoryCard(HistoryItem item)
    {
        Size = new Size(1010, 172);
        CornerRadius = 16;
        Margin = new Padding(0, 0, 0, 14);
        Padding = new Padding(26, 18, 26, 18);
        BorderColor = AppTheme.Border;
        Cursor = Cursors.Hand;

        var dateLabel = new ThemeLabel { TextKind = TextKind.PrimarySmall, Text = "🗓  " + item.Date, AutoSize = true, Location = new Point(0, 0) };
        var petLabel = new ThemeLabel { TextKind = TextKind.SubHeading, Text = item.Pet, AutoSize = true, Location = new Point(0, 22) };
        var resultPill = new PillButton { Text = item.Result, Active = true, Height = 26, Width = 110, ActiveColor = SeverityColorFor(item.Result), Location = new Point(270, 20) };
        resultPill.Enabled = false;
        var statusPill = new Label
        {
            AutoSize = true,
            Text = item.Status,
            ForeColor = item.Status == "Completado" ? AppTheme.SeverityLeve : AppTheme.SeverityModerate,
            BackColor = Color.Transparent,
            Font = AppTheme.Medium(7.5f),
            Location = new Point(610, 24)
        };
        var delete = new RoundedButton { Variant = ButtonVariant.Muted, Text = "🗑️", Icon = null, Size = new Size(30, 30), CornerRadius = 8, Font = AppTheme.Emoji(7.5f), Location = new Point(950, 14), Cursor = Cursors.Default };
        delete.Click += (_, _) => DeleteClicked?.Invoke();
        var summary = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = item.Summary,
            MaximumSize = new Size(940, 40),
            AutoEllipsis = true,
            Location = new Point(0, 58)
        };
        var rec = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = "💡  " + item.Recommendation,
            MaximumSize = new Size(940, 40),
            AutoEllipsis = true,
            Location = new Point(0, 98)
        };
        var details = new ThemeLabel { TextKind = TextKind.PrimarySmall, Text = "Ver detalles  ›", AutoSize = true, Location = new Point(0, 138), Cursor = Cursors.Hand };

        Controls.Add(dateLabel);
        Controls.Add(petLabel);
        Controls.Add(resultPill);
        Controls.Add(statusPill);
        Controls.Add(delete);
        Controls.Add(summary);
        Controls.Add(rec);
        Controls.Add(details);

        // El clic sobre la tarjeta (fuera de los botones hijos) abre el detalle.
        Click += (_, _) => DetailClicked?.Invoke();
        details.Click += (_, _) => DetailClicked?.Invoke();

        AppTheme.ThemeChanged += (_, _) => statusPill.ForeColor = item.Status == "Completado" ? AppTheme.SeverityLeve : AppTheme.SeverityModerate;
    }

    private Color SeverityColorFor(string result)
    {
        if (result.Contains("CRÍTICO", StringComparison.OrdinalIgnoreCase) || result.Contains("Crítico", StringComparison.OrdinalIgnoreCase))
            return AppTheme.SeverityCritical;
        if (result.Contains("MODERADO", StringComparison.OrdinalIgnoreCase) || result.Contains("Moderado", StringComparison.OrdinalIgnoreCase))
            return AppTheme.SeverityModerate;
        return AppTheme.SeverityLeve;
    }
}

/// <summary>Tarjeta de un proceso programado.</summary>
public sealed class UpcomingCard : RoundedPanel
{
    public event Action? DeleteClicked;

    public UpcomingCard(UpcomingItem item)
    {
        Size = new Size(1010, 128);
        CornerRadius = 16;
        Margin = new Padding(0, 0, 0, 14);
        BorderColor = AppTheme.Border;

        var typePill = new PillButton { Text = item.Type, Active = true, Height = 26, Width = 120, ActiveColor = Color.FromArgb(59, 130, 246), Location = new Point(26, 18) };
        typePill.Enabled = false;
        var dateLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "🗓  " + item.Date, AutoSize = true, Location = new Point(170, 22) };
        var petLabel = new ThemeLabel { TextKind = TextKind.SubHeading, Text = item.Pet, AutoSize = true, Location = new Point(26, 52) };
        var summary = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = item.Summary,
            MaximumSize = new Size(700, 44),
            AutoEllipsis = true,
            Location = new Point(26, 84)
        };
        var actionPill = new PillButton { Text = item.Action, Active = true, Height = 26, Width = 140, ActiveColor = AppTheme.SeverityLeve, Location = new Point(820, 20) };
        actionPill.Enabled = false;
        var edit = new RoundedButton { Variant = ButtonVariant.Muted, Text = "✏️", Icon = null, Size = new Size(30, 30), CornerRadius = 8, Font = AppTheme.Emoji(7.5f), Location = new Point(880, 74) };
        var delete = new RoundedButton { Variant = ButtonVariant.Muted, Text = "🗑️", Icon = null, Size = new Size(30, 30), CornerRadius = 8, Font = AppTheme.Emoji(7.5f), Location = new Point(920, 74) };
        edit.Click += (_, _) => { /* edición simple: por ahora notifica */ };
        delete.Click += (_, _) => DeleteClicked?.Invoke();

        Controls.Add(typePill);
        Controls.Add(dateLabel);
        Controls.Add(petLabel);
        Controls.Add(summary);
        Controls.Add(actionPill);
        Controls.Add(edit);
        Controls.Add(delete);
    }
}

/// <summary>Tarjeta de alerta activa.</summary>
public sealed class AlertCard : RoundedPanel
{
    public event Action? DeleteClicked;

    public AlertCard(AlertItem item)
    {
        Size = new Size(1010, 150);
        CornerRadius = 16;
        Margin = new Padding(0, 0, 0, 14);
        BorderColor = AppTheme.Border;

        var severityColor = item.Severity switch
        {
            "Alta" => AppTheme.SeverityCritical,
            "Media" => AppTheme.SeverityModerate,
            _ => AppTheme.SeverityLeve
        };
        var sevPill = new PillButton { Text = item.Severity, Active = true, Height = 26, Width = 90, ActiveColor = severityColor, Location = new Point(26, 18) };
        sevPill.Enabled = false;
        var titleLabel = new ThemeLabel { TextKind = TextKind.SubHeading, Text = item.Title, AutoSize = true, Location = new Point(130, 20) };
        var petLabel = new ThemeLabel { TextKind = TextKind.PrimarySmall, Text = "🐾 Para: " + item.Pet, AutoSize = true, Location = new Point(26, 52) };
        var message = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = item.Message,
            MaximumSize = new Size(700, 44),
            AutoEllipsis = true,
            Location = new Point(26, 80)
        };
        var whenLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = $"{item.Date}  {item.Time} h", AutoSize = true, Location = new Point(820, 22) };
        var bell = new ThemeLabel { TextKind = TextKind.Body, Text = "🔔", Font = AppTheme.Emoji(14f), AutoSize = true, Location = new Point(880, 84) };
        var delete = new RoundedButton { Variant = ButtonVariant.Muted, Text = "🗑️", Icon = null, Size = new Size(30, 30), CornerRadius = 8, Font = AppTheme.Emoji(7.5f), Location = new Point(920, 96) };
        delete.Click += (_, _) => DeleteClicked?.Invoke();

        Controls.Add(sevPill);
        Controls.Add(titleLabel);
        Controls.Add(petLabel);
        Controls.Add(message);
        Controls.Add(whenLabel);
        Controls.Add(bell);
        Controls.Add(delete);
    }
}

/// <summary>Modal de detalle de una consulta.</summary>
public sealed class HistoryDetailModal : ModalForm
{
    public HistoryDetailModal(HistoryItem item)
    {
        Size = new Size(640, 560);
        SetHeader("📜", "Detalle de Consulta", $"{item.Pet} · {item.Date}");

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent,
            Width = 560
        };

        layout.Controls.Add(FieldRow("Estado", item.Status));
        layout.Controls.Add(FieldRow("Resultado", item.Result));
        layout.Controls.Add(FieldRow("Mascota", item.Pet));
        layout.Controls.Add(FieldRow("Fecha", item.Date));
        layout.Controls.Add(FieldBox("Resumen de la consulta", item.Summary));
        layout.Controls.Add(FieldBox("Recomendación", item.Recommendation));

        Content.Controls.Add(layout);
    }

    private static ThemeLabel FieldRow(string label, string value)
    {
        return new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = $"{label}:  {value}",
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 2, 0, 6)
        };
    }

    private static RoundedPanel FieldBox(string label, string value)
    {
        var box = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 12,
            BorderColor = AppTheme.Border,
            Width = 556,
            Height = 96,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0, 6, 0, 10)
        };
        var title = new ThemeLabel { TextKind = TextKind.SubHeading, Text = label, AutoSize = true };
        var text = new ThemeLabel
        {
            TextKind = TextKind.Muted,
            Text = value,
            MaximumSize = new Size(520, 60),
            AutoEllipsis = true,
            Location = new Point(16, 34)
        };
        box.Controls.Add(title);
        box.Controls.Add(text);
        return box;
    }
}

/// <summary>Modal para agregar un proceso programado.</summary>
public sealed class UpcomingModal : ModalForm
{
    private readonly AppStorage _storage;
    private readonly RoundedTextBox _date;
    private readonly RoundedTextBox _type;
    private readonly RoundedTextBox _pet;
    private readonly RoundedTextBox _summary;
    private readonly RoundedTextBox _action;

    public UpcomingModal(AppStorage storage)
    {
        _storage = storage;
        Size = new Size(560, 620);
        SetHeader("📅", "Agregar Proceso Programado", "Programa un seguimiento, vacuna o control para tu mascota");

        _date = MakeField("Fecha", "📅", "Ej. 20 de Mayo, 2024");
        _type = MakeField("Tipo de proceso", "🏷️", "Ej. Seguimiento, Vacuna, Control");
        _pet = MakeField("Mascota", "🐾", "Nombre de la mascota");
        _summary = MakeField("Descripción", "📝", "Detalles del proceso...");
        _summary.Multiline = true;
        _summary.Height = 84;
        _action = MakeField("Acción", "✅", "Ej. Cita Programada");

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Width = 480,
            BackColor = Color.Transparent
        };
        layout.Controls.Add(FieldLabel("Fecha"));
        layout.Controls.Add(_date);
        layout.Controls.Add(FieldLabel("Tipo de proceso"));
        layout.Controls.Add(_type);
        layout.Controls.Add(FieldLabel("Mascota"));
        layout.Controls.Add(_pet);
        layout.Controls.Add(FieldLabel("Descripción"));
        layout.Controls.Add(_summary);
        layout.Controls.Add(FieldLabel("Acción"));
        layout.Controls.Add(_action);

        var save = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Guardar Proceso", Icon = "💾", IconSize = 9f,
            Size = new Size(180, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(0, 14, 0, 0)
        };
        save.Click += (_, _) => Save();
        var cancel = new RoundedButton { Variant = ButtonVariant.Ghost, Text = "Cancelar", Icon = null, Size = new Size(110, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(8, 14, 0, 0) };
        cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        var actions = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, BackColor = Color.Transparent, Dock = DockStyle.Bottom };
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        host.Controls.Add(layout);
        host.Controls.Add(actions);
        actions.Dock = DockStyle.Bottom;
        Content.Controls.Add(host);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_date.Text) || string.IsNullOrWhiteSpace(_pet.Text))
        {
            MessageBox.Show(this, "La fecha y la mascota son obligatorias.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var list = _storage.GetData("upcomingData", DemoData.CreateInitialUpcoming());
        list.Add(new UpcomingItem
        {
            Id = DateTime.UtcNow.Ticks,
            Date = _date.Text.Trim(),
            Pet = _pet.Text.Trim(),
            Type = string.IsNullOrWhiteSpace(_type.Text) ? "Seguimiento" : _type.Text.Trim(),
            Summary = string.IsNullOrWhiteSpace(_summary.Text) ? "Sin descripción" : _summary.Text.Trim(),
            Action = string.IsNullOrWhiteSpace(_action.Text) ? "Cita Programada" : _action.Text.Trim()
        });
        _storage.SetData("upcomingData", list);
        DialogResult = DialogResult.OK;
    }

    private RoundedTextBox MakeField(string placeholder, string icon, string ph)
    {
        return new RoundedTextBox
        {
            IconText = icon,
            PlaceholderText = ph,
            Width = 480,
            Height = 42,
            FieldHeight = 42,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
    }

    private static ThemeLabel FieldLabel(string text) => new()
    {
        TextKind = TextKind.MutedSmall,
        Text = text,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 8, 0, 3)
    };
}

/// <summary>Modal para agregar una alerta activa.</summary>
public sealed class AlertModal : ModalForm
{
    private readonly AppStorage _storage;
    private readonly RoundedComboBox _severity;
    private readonly RoundedTextBox _title;
    private readonly RoundedTextBox _pet;
    private readonly RoundedTextBox _message;
    private readonly RoundedTextBox _date;
    private readonly RoundedTextBox _time;

    public AlertModal(AppStorage storage)
    {
        _storage = storage;
        Size = new Size(560, 660);
        SetHeader("🔔", "Agregar Alerta Activa", "Programa un recordatorio con fecha y hora (formato HH:mm)");

        _severity = new RoundedComboBox { Width = 480, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        _severity.SetItems(new[] { "Baja", "Media", "Alta" });

        _title = MakeField("Título de la alerta", "🏷️", "Ej. Dosis de Desparasitante");
        _pet = MakeField("Mascota", "🐾", "Nombre de la mascota");
        _message = MakeField("Mensaje / Nota", "📝", "Detalle del recordatorio...");
        _message.Multiline = true;
        _message.Height = 84;
        _date = MakeField("Fecha (yyyy-mm-dd)", "📅", "Ej. 2026-12-25");
        _time = MakeField("Hora (HH:mm)", "⏰", "Ej. 12:00");

        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Width = 480,
            BackColor = Color.Transparent
        };
        layout.Controls.Add(FieldLabel("Severidad"));
        layout.Controls.Add(_severity);
        layout.Controls.Add(FieldLabel("Título"));
        layout.Controls.Add(_title);
        layout.Controls.Add(FieldLabel("Mascota"));
        layout.Controls.Add(_pet);
        layout.Controls.Add(FieldLabel("Mensaje"));
        layout.Controls.Add(_message);
        layout.Controls.Add(FieldLabel("Fecha"));
        layout.Controls.Add(_date);
        layout.Controls.Add(FieldLabel("Hora (HH:mm) — la alerta sonará exactamente a esa hora"));
        layout.Controls.Add(_time);

        var save = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Guardar Alerta", Icon = "🔔", IconSize = 9f,
            Size = new Size(170, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(0, 14, 0, 0)
        };
        save.Click += (_, _) => Save();
        var cancel = new RoundedButton { Variant = ButtonVariant.Ghost, Text = "Cancelar", Icon = null, Size = new Size(110, 42), Font = AppTheme.Medium(9.5f), Margin = new Padding(8, 14, 0, 0) };
        cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        var actions = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, BackColor = Color.Transparent, Dock = DockStyle.Bottom };
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        host.Controls.Add(layout);
        host.Controls.Add(actions);
        actions.Dock = DockStyle.Bottom;
        Content.Controls.Add(host);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_title.Text) || string.IsNullOrWhiteSpace(_date.Text) || string.IsNullOrWhiteSpace(_time.Text))
        {
            MessageBox.Show(this, "El título, la fecha y la hora son obligatorios.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var list = _storage.GetData("alertsData", DemoData.CreateInitialAlerts());
        list.Add(new AlertItem
        {
            Id = DateTime.UtcNow.Ticks,
            Severity = _severity.SelectedText,
            Title = _title.Text.Trim(),
            Pet = string.IsNullOrWhiteSpace(_pet.Text) ? "Mi Mascota" : _pet.Text.Trim(),
            Message = string.IsNullOrWhiteSpace(_message.Text) ? "Recordatorio de cuidado." : _message.Text.Trim(),
            Date = _date.Text.Trim(),
            Time = _time.Text.Trim()
        });
        _storage.SetData("alertsData", list);
        DialogResult = DialogResult.OK;
    }

    private RoundedTextBox MakeField(string placeholder, string icon, string ph)
    {
        return new RoundedTextBox
        {
            IconText = icon,
            PlaceholderText = ph,
            Width = 480,
            Height = 42,
            FieldHeight = 42,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
    }

    private static ThemeLabel FieldLabel(string text) => new()
    {
        TextKind = TextKind.MutedSmall,
        Text = text,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 8, 0, 3)
    };
}