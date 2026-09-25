using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Storage;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

public sealed partial class PetsPage : PageBase
{
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private PageBody body = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private FlowLayoutPanel root = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private Panel header = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel icon = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel title = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private ThemeLabel subtitle = null!;
    /// <summary>Control del diseño (ver InitializeComponent).</summary>
    private RoundedButton addBtn = null!;
    private INavigator _nav = null!;
    private FlowLayoutPanel _cardsHost = null!;
    private List<PetData> _pets = new();

    public PetsPage(INavigator nav)
    {
        _nav = nav;
        InitializeComponent();
    }

    /// <summary>
    /// Constructor solo para el Diseñador de Visual Studio.
    /// Usa un navegador y un almacenamiento en memoria, así la página
    /// se ve en el diseño exactamente igual que en ejecución.
    /// </summary>
    public PetsPage() : this(DesignTimeNavigator.Instance)
    {
        Size = DesignTime.PageCanvas;
        DesignTime.PrimeLayout(this);
    }

    private void InitializeComponent()
    {
        body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 1150 };
        Controls.Add(body);

        root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        // Encabezado
        header = new Panel { Height = 118, Width = 1070, Anchor = AnchorStyles.None, BackColor = Color.Transparent };
        icon = new ThemeLabel { TextKind = TextKind.Body, Text = "🐾", Font = AppTheme.Emoji(16f) };
        title = new ThemeLabel { TextKind = TextKind.Display, Text = "  Mis Mascotas" };
        subtitle = new ThemeLabel { TextKind = TextKind.Body, Text = "Gestiona el perfil de tus mascotas y su información clínica." };
        addBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Agregar Mascota",
            Icon = "➕",
            IconSize = 9f,
            Size = new Size(180, 44),
            Font = AppTheme.Medium(9.5f)
        };
        addBtn.Click += (_, _) => OpenEditor(null);
        header.Controls.Add(icon);
        header.Controls.Add(title);
        header.Controls.Add(subtitle);
        header.Controls.Add(addBtn);
        header.Resize += (_, _) =>
        {
            icon.Location = new Point(0, 4);
            title.Location = new Point(34, 0);
            subtitle.Location = new Point(34, 44);
            addBtn.Location = new Point(header.Width - addBtn.Width, 30);
        };
        root.Controls.Add(header);

        _cardsHost = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            Width = 1070,
            BackColor = Color.Transparent
        };
        root.Controls.Add(_cardsHost);

        body.Controls.Add(root);
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        Reload();
    }

    private void Reload()
    {
        _pets = _nav.Storage.GetData("petsData", DemoData.CreateInitialPets());
        _nav.Storage.SetData("petsData", _pets);
        _cardsHost.SuspendLayout();
        _cardsHost.Controls.Clear();
        foreach (var pet in _pets)
        {
            var card = new PetCard(pet);
            card.EditClicked += () => OpenEditor(pet);
            card.DeleteClicked += () => DeletePet(pet);
            _cardsHost.Controls.Add(card);
        }
        _cardsHost.ResumeLayout();
    }

    private void DeletePet(PetData pet)
    {
        var owner = FindForm();
        var result = MessageBox.Show(owner, $"¿Estás seguro de eliminar a {pet.Name}?", "Eliminar mascota",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        if (result != DialogResult.Yes) return;
        _pets.RemoveAll(p => p.Id == pet.Id);
        _nav.Storage.SetData("petsData", _pets);
        _nav.ShowToast($"Mascota {pet.Name} eliminada", ToastKind.Info);
        Reload();
    }

    private void OpenEditor(PetData? pet)
    {
        var owner = FindForm();
        if (owner is null) return;
        var modal = new PetModalForm(pet, _nav.Storage);
        if (ModalHost.ShowDialog(owner, modal) == DialogResult.OK && modal.Result is { } saved)
        {
            var existing = _pets.FirstOrDefault(p => p.Id == saved.Id);
            if (existing is null) _pets.Add(saved);
            else _pets[_pets.IndexOf(existing)] = saved;
            _nav.Storage.SetData("petsData", _pets);
            _nav.ShowToast(pet is null ? "¡Mascota registrada con éxito!" : "Perfil actualizado con éxito", ToastKind.Success);
            Reload();
        }
    }
}

/// <summary>Tarjeta de mascota (card de Pets.tsx).</summary>
public sealed class PetCard : RoundedPanel
{
    private readonly PetData _pet;
    private bool _hovered;

    public event Action? EditClicked;
    public event Action? DeleteClicked;

    public PetCard(PetData pet)
    {
        _pet = pet;
        Size = new Size(340, 318);
        CornerRadius = 18;
        Margin = new Padding(0, 0, 18, 18);
        AppTheme.ThemeChanged += (_, _) => Invalidate();

        MouseEnter += (_, _) => { _hovered = true; Invalidate(); };
        MouseLeave += (_, _) => { _hovered = false; Invalidate(); };

        var edit = new RoundedButton { Variant = ButtonVariant.Muted, Text = "✏️", Icon = null, Size = new Size(32, 32), CornerRadius = 8, Font = AppTheme.Emoji(8f) };
        edit.Click += (_, _) => EditClicked?.Invoke();
        var del = new RoundedButton { Variant = ButtonVariant.Muted, Text = "🗑️", Icon = null, Size = new Size(32, 32), CornerRadius = 8, Font = AppTheme.Emoji(8f) };
        del.Click += (_, _) => DeleteClicked?.Invoke();

        var pawBox = new RoundedPanel { FillColor = AppTheme.WithAlpha(AppTheme.Primary, 22), CornerRadius = 12, Size = new Size(56, 56) };
        var paw = new ThemeLabel { TextKind = TextKind.Body, Text = "🐾", Font = AppTheme.Emoji(15f), AutoSize = true, Anchor = AnchorStyles.None };
        pawBox.Controls.Add(paw);
        paw.Location = new Point((56 - paw.Width) / 2, (56 - paw.Height) / 2);

        var nameLabel = new ThemeLabel { TextKind = TextKind.Heading, Text = pet.Name };
        var breedLabel = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = $"{pet.Breed} • {pet.Age}" };

        var info = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        info.Controls.Add(MakeRow("💉", "Vacunas: ", pet.Vaccines));
        info.Controls.Add(MakeRow("📋", "Antecedentes: ", pet.Clinical));
        info.Controls.Add(MakeRow("🩺", "Veterinario: ", pet.Vet));
        info.Width = 296;

        var profileBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Ver perfil completo  ›",
            Icon = null,
            Size = new Size(296, 38),
            CornerRadius = 10,
            Font = AppTheme.Medium(8.5f)
        };
        profileBtn.Click += (_, _) => EditClicked?.Invoke();

        Controls.Add(pawBox);
        Controls.Add(nameLabel);
        Controls.Add(breedLabel);
        Controls.Add(edit);
        Controls.Add(del);
        Controls.Add(info);
        Controls.Add(profileBtn);
        LayoutPanel();

        Resize += (_, _) => LayoutPanel();
    }

    private ThemeLabel MakeRow(string icon, string label, string value)
    {
        var row = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = $"{icon}  ",
            AutoSize = true
        };
        return new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = $"{icon}  {label}{value}",
            AutoSize = true,
            MaximumSize = new Size(296, 34),
            AutoEllipsis = true
        };
    }

    private void LayoutPanel()
    {
        if (Controls.Count < 7) return;
        var pawBox = Controls[0];
        var nameLabel = Controls[1];
        var breedLabel = Controls[2];
        var edit = Controls[3];
        var del = Controls[4];
        var info = Controls[5];
        var profileBtn = Controls[6];

        pawBox.Location = new Point(20, 18);
        nameLabel.Location = new Point(pawBox.Right + 14, 16);
        breedLabel.Location = new Point(pawBox.Right + 14, nameLabel.Bottom + 1);
        edit.Location = new Point(Width - edit.Width - 16, 18);
        del.Location = new Point(Width - del.Width * 2 - 22, 18);
        info.Location = new Point(22, 96);
        profileBtn.Location = new Point(22, Height - profileBtn.Height - 18);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_hovered)
        {
            DrawingHelpers.StrokeRounded(e.Graphics, AppTheme.WithAlpha(AppTheme.Primary, 110), 1.4f,
                new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius);
        }
    }
}

/// <summary>Modal de registro/edición de mascota (con historial automático al editar).</summary>
public sealed class PetModalForm : ModalForm
{
    private readonly PetData? _original;
    private readonly AppStorage _storage;

    private readonly RoundedTextBox _name;
    private readonly RoundedTextBox _breed;
    private readonly RoundedTextBox _age;
    private readonly RoundedComboBox _sex;
    private readonly RoundedTextBox _vaccines;
    private readonly RoundedTextBox _history;
    private readonly RoundedTextBox _clinical;
    private readonly RoundedTextBox _vet;

    public PetData? Result { get; private set; }

    public PetModalForm(PetData? original, AppStorage storage)
    {
        _original = original;
        _storage = storage;
        Size = new Size(680, _original is null ? 640 : 760);
        StartPosition = FormStartPosition.CenterParent;

        SetHeader(_original is null ? "🐾" : "✏️",
            _original is null ? "Registrar Nueva Mascota" : "Editar Perfil de Mascota",
            "Completa la información clínica de tu mascota");

        _name = MakeField("Nombre", "👤", "Ej. Max", original?.Name ?? "");
        _breed = MakeField("Especie / Raza", "🐕", "Ej. Golden Retriever", original?.Breed ?? "");
        _age = MakeField("Edad", "🎂", "Ej. 3 años", original?.Age ?? "");
        _sex = new RoundedComboBox { Width = 262, Height = 42, FieldHeight = 42, Anchor = AnchorStyles.None };
        _sex.SetItems(new[] { "Macho", "Hembra" });
        if (!string.IsNullOrEmpty(original?.Sex)) _sex.SelectedItem = original.Sex;
        else _sex.SelectedIndex = 0;

        _vaccines = MakeField("Registro de Vacunas", "💉", "Ej: Rabia, Parvovirus (Al día)", original?.Vaccines ?? "");
        _history = MakeField("Registro de Enfermedades Previas", "📋", "Describe cualquier enfermedad que haya tenido...", original?.History ?? "");
        _clinical = MakeField("Antecedentes Clínicos (Alergias, Condiciones)", "🧬", "Ej: Alergia al pollo, asma...", original?.Clinical ?? "");
        _vet = MakeField("Veterinario Tratante / Clínica", "🏥", "Nombre del doctor o centro veterinario", original?.Vet ?? "");

        _history.Multiline = true;
        _history.Height = 76;
        _clinical.Multiline = true;
        _clinical.Height = 76;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 12,
            BackColor = Color.Transparent,
            Padding = new Padding(30, 0, 30, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 12; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddPair(layout, 0, "Nombre", _name, "Especie / Raza", _breed);
        AddPair(layout, 1, "Edad", _age, "Sexo", _sex);
        AddFull(layout, 2, "Registro de Vacunas", _vaccines);
        AddFull(layout, 3, "Enfermedades Previas (Historia clínica)", _history);
        AddFull(layout, 4, "Antecedentes Clínicos", _clinical);
        AddFull(layout, 5, "Veterinario Tratante / Clínica", _vet);

        var actions = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            BackColor = Color.Transparent,
            Dock = DockStyle.Bottom,
            Padding = new Padding(30, 0, 30, 6)
        };
        var save = new RoundedButton
        {
            Variant = ButtonVariant.Primary, Text = "Guardar Perfil", Icon = "💾", IconSize = 9f,
            Size = new Size(160, 42), Font = AppTheme.Medium(9.5f)
        };
        save.Click += (_, _) => Save();
        var cancel = new RoundedButton
        {
            Variant = ButtonVariant.Ghost, Text = "Cancelar", Icon = null,
            Size = new Size(110, 42), Font = AppTheme.Medium(9.5f)
        };
        cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);

        Content.Controls.Add(layout);
        if (!actions.Visible) return;
        var contentLayout = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        contentLayout.Controls.Add(layout);
        contentLayout.Controls.Add(actions);
        actions.Dock = DockStyle.Bottom;
        Content.Controls.Clear();
        Content.Controls.Add(contentLayout);

        // Historial automático al editar
        if (_original is not null)
        {
            var historySection = BuildAutoHistory(_original.Name);
            contentLayout.Controls.Add(historySection);
            historySection.Dock = DockStyle.Bottom;
        }
    }

    private void AddPair(TableLayoutPanel layout, int row, string label1, Control field1, string label2, Control field2)
    {
        var l1 = FieldLabel(label1);
        var l2 = FieldLabel(label2);
        layout.Controls.Add(l1, 0, row * 2);
        layout.Controls.Add(l2, 1, row * 2);
        layout.Controls.Add(field1, 0, row * 2 + 1);
        layout.Controls.Add(field2, 1, row * 2 + 1);
    }

    private void AddFull(TableLayoutPanel layout, int row, string label, Control field)
    {
        var l = FieldLabel(label);
        layout.Controls.Add(l, 0, row * 2);
        layout.SetColumnSpan(l, 2);
        if (field is RoundedTextBox rt && rt.Multiline) rt.Width = 586;
        field.Width = 586;
        field.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        layout.Controls.Add(field, 0, row * 2 + 1);
        layout.SetColumnSpan(field, 2);
    }

    private static ThemeLabel FieldLabel(string text) => new() { TextKind = TextKind.MutedSmall, Text = text, Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 3) };

    private RoundedTextBox MakeField(string label, string icon, string placeholder, string value)
    {
        return new RoundedTextBox
        {
            IconText = icon,
            PlaceholderText = placeholder,
            Text = value,
            Width = 262,
            Height = 42,
            FieldHeight = 42,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
    }

    private Control BuildAutoHistory(string petName)
    {
        var historyData = _storage.GetData("historyData", DemoData.CreateInitialHistory());
        var items = historyData.Where(h => h.Pet == petName).ToList();

        var box = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 14,
            Height = items.Count == 0 ? 70 : 130,
            Width = 620,
            BorderColor = AppTheme.Border,
            Padding = new Padding(24, 16, 24, 16)
        };

        var header = new ThemeLabel { TextKind = TextKind.SubHeading, Text = "📜  Historial de Análisis (Automático)", Dock = DockStyle.Top };
        box.Controls.Add(header);

        var listHost = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, BackColor = Color.Transparent, Dock = DockStyle.Top };
        if (items.Count == 0)
        {
            listHost.Controls.Add(new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "No hay consultas registradas para esta mascota aún.", Dock = DockStyle.Top, Margin = new Padding(0, 8, 0, 0) });
        }
        else
        {
            foreach (var item in items.Take(3))
            {
                listHost.Controls.Add(new ThemeLabel
                {
                    TextKind = TextKind.MutedSmall,
                    Text = $"• {item.Date}  [ {item.Result} ]  {item.Summary}",
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 6, 0, 0),
                    MaximumSize = new Size(560, 44),
                    AutoEllipsis = true
                });
            }
            box.Height = 150 + Math.Min(3, items.Count) * 28;
        }
        box.Controls.Add(listHost);
        return box;
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_name.Text) || string.IsNullOrWhiteSpace(_breed.Text))
        {
            MessageBox.Show(this, "El nombre y la raza son obligatorios.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        Result = new PetData
        {
            Id = _original?.Id ?? DateTime.UtcNow.Ticks,
            Name = _name.Text.Trim(),
            Breed = _breed.Text.Trim(),
            Age = _age.Text.Trim(),
            Sex = _sex.SelectedText,
            Vaccines = _vaccines.Text.Trim(),
            History = _history.Text.Trim(),
            Clinical = _clinical.Text.Trim(),
            Vet = _vet.Text.Trim()
        };
        DialogResult = DialogResult.OK;
    }
}