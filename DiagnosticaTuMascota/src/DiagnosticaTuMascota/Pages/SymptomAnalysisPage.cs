using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Controls;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Pages;

/// <summary>
/// Paso 1 del análisis de síntomas (SymptomAnalysis.tsx): categorías con sus
/// píldoras de síntomas, banner de aviso y botón Continuar.
/// </summary>
public sealed class SymptomAnalysisPage : PageBase
{
    private readonly INavigator _nav;
    private readonly HashSet<string> _selected = new();
    private readonly ThemeLabel _countLabel;
    private readonly RoundedButton _continueBtn;
    private readonly Panel _categoryHost;
    private readonly TableLayoutPanel _mainRow;
    private readonly RoundedPanel _contentCard;

    private static readonly (string Name, string Icon, Color C1, Color C2, string[] Symptoms)[] Categories =
    {
        ("Síntomas Generales", "📊", Color.FromArgb(16, 185, 129), Color.FromArgb(20, 184, 166),
            new[] { "Fiebre", "Letargo / Debilidad", "Pérdida de peso", "Aumento de sed", "Temblores" }),
        ("Respiratorios", "💨", Color.FromArgb(59, 130, 246), Color.FromArgb(6, 182, 212),
            new[] { "Tos persistente", "Estornudos", "Dificultad para respirar", "Secreción nasal", "Respiración ruidosa" }),
        ("Digestivos", "🍽️", Color.FromArgb(249, 115, 22), Color.FromArgb(245, 158, 11),
            new[] { "Vómitos", "Diarrea", "Pérdida de apetito", "Dificultad al tragar", "Abdomen hinchado" }),
        ("Neurológicos / Diagnósticos", "🧠", Color.FromArgb(139, 92, 246), Color.FromArgb(99, 102, 241),
            new[] { "Convulsiones", "Desorientación", "Incoordinación al caminar", "Parálisis", "Cambio de conducta" })
    };

    public SymptomAnalysisPage(INavigator nav)
    {
        _nav = nav;
        var body = new PageBody { Dock = DockStyle.Fill, MaxWidth = 1150 };
        Controls.Add(body);

        var root = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
            AutoSize = true,
            Width = 1110
        };

        // Barra superior
        var topBar = new Panel { Height = 46, Width = 1110, BackColor = Color.Transparent };
        var back = new ThemeLabel { TextKind = TextKind.MutedSmall, Text = "←  Volver al Dashboard", Cursor = Cursors.Hand, Anchor = AnchorStyles.Left };
        back.Click += (_, _) => _nav.Navigate(AppPage.Dashboard);
        var stepPill = new PillButton { Text = "Paso 1 de 2", IconText = "📋", Active = true, Width = 130, Height = 34, ActiveColor = AppTheme.Secondary };
        var progress = new ProgressBarSkin { Width = 220, Height = 8, Percent = 50, Anchor = AnchorStyles.Left };
        topBar.Controls.Add(back);
        topBar.Controls.Add(stepPill);
        topBar.Controls.Add(progress);
        topBar.Resize += (_, _) =>
        {
            back.Location = new Point(4, 12);
            progress.Location = new Point(back.Right + 130, 19);
            stepPill.Location = new Point(progress.Right + 14, 6);
        };
        root.Controls.Add(topBar);

        var headerIcon = new ThemeLabel { TextKind = TextKind.Body, Text = "🩺", Font = AppTheme.Emoji(15f), Anchor = AnchorStyles.Left };
        var title = new ThemeLabel { TextKind = TextKind.Display, Text = "  ¿Qué síntomas presenta tu mascota?", Anchor = AnchorStyles.Left };
        var subtitle = new ThemeLabel
        {
            TextKind = TextKind.Body,
            Text = "Selecciona todos los síntomas que observes. Esta orientación NO sustituye la consulta veterinaria.",
            MaximumSize = new Size(1080, 40),
            AutoEllipsis = true,
            Anchor = AnchorStyles.Left
        };
        var headerRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, BackColor = Color.Transparent };
        headerRow.Controls.Add(headerIcon);
        headerRow.Controls.Add(title);
        root.Controls.Add(headerRow);
        subtitle.Margin = new Padding(38, 0, 0, 14);
        root.Controls.Add(subtitle);

        var disclaimer = new MedicalDisclaimer { Width = 1110, Height = 104 };
        root.Controls.Add(disclaimer);

        // Fila principal: categorías + tarjeta de síntomas
        _mainRow = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, Width = 1110, Height = 480, BackColor = Color.Transparent };
        var colLeft = new ColumnStyle(SizeType.Absolute, 320);
        var colRight = new ColumnStyle(SizeType.Percent, 100);
        _mainRow.ColumnStyles.Add(colLeft);
        _mainRow.ColumnStyles.Add(colRight);

        _categoryHost = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 6, 6, 0),
            BackColor = Color.Transparent
        };
        _mainRow.Controls.Add(_categoryHost, 0, 0);

        _contentCard = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 18,
            BorderColor = AppTheme.Border,
            Dock = DockStyle.Fill,
            Padding = new Padding(26, 20, 26, 20)
        };
        _mainRow.Controls.Add(_contentCard, 1, 0);

        root.Controls.Add(_mainRow);

        // Barra inferior con contador y botón continuar
        var bottomBar = new RoundedPanel
        {
            FillColor = AppTheme.Card,
            CornerRadius = 18,
            BorderColor = AppTheme.Border,
            Height = 76,
            Width = 1110,
            Padding = new Padding(26, 14, 26, 14)
        };
        _countLabel = new ThemeLabel { TextKind = TextKind.Body, Text = "Síntomas seleccionados: 0", Anchor = AnchorStyles.Left };
        _continueBtn = new RoundedButton
        {
            Variant = ButtonVariant.Primary,
            Text = "Continuar Análisis",
            Icon = "➡️",
            IconSize = 9f,
            Size = new Size(210, 44),
            Font = AppTheme.Medium(10f),
            Enabled = false,
            Anchor = AnchorStyles.Right
        };
        _continueBtn.Click += (_, _) =>
        {
            _nav.Navigate(AppPage.ConsultationStep2, new NavState { SelectedSymptoms = _selected.ToList() });
        };
        bottomBar.Controls.Add(_countLabel);
        bottomBar.Controls.Add(_continueBtn);
        bottomBar.Resize += (_, _) =>
        {
            _countLabel.Location = new Point(26, (bottomBar.Height - _countLabel.Height) / 2);
            _continueBtn.Location = new Point(bottomBar.Width - _continueBtn.Width - 26, 16);
        };
        root.Controls.Add(bottomBar);

        body.Controls.Add(root);

        BuildCategories();
        SelectCategory(0);
    }

    private void BuildCategories()
    {
        int i = 0;
        foreach (var cat in Categories)
        {
            var chip = new CategoryChip(cat.Name, cat.Icon, cat.C1, cat.C2, cat.Symptoms.Length, i == 0);
            int idx = i;
            chip.Click += (_, _) => SelectCategory(idx);
            _categoryHost.Controls.Add(chip);
            i++;
        }
    }

    private void SelectCategory(int index)
    {
        int i = 0;
        foreach (Control c in _categoryHost.Controls)
        {
            if (c is CategoryChip chip) chip.Active = i == index;
            i++;
        }

        var cat = Categories[index];
        _contentCard.Controls.Clear();

        var header = new ThemeLabel { TextKind = TextKind.Heading, Text = $"{cat.Icon}  {cat.Name}", AutoSize = true, Location = new Point(0, 0) };
        _contentCard.Controls.Add(header);

        var note = new ThemeLabel
        {
            TextKind = TextKind.MutedSmall,
            Text = "Haz clic en cada síntoma para marcarlo o desmarcarlo.",
            AutoSize = true,
            Location = new Point(0, header.Height + 8)
        };
        _contentCard.Controls.Add(note);

        _symptomsGridTop = 46;
        RebuildSymptoms(index);
    }

    private int _symptomsGridTop;
    private void RebuildSymptoms(int index)
    {
        // Eliminar chips antiguos
        foreach (var chip in _contentCard.Controls.OfType<SymptomChip>().ToList())
        {
            _contentCard.Controls.Remove(chip);
            chip.Dispose();
        }

        var cat = Categories[index];
        int cols = 2;
        int colW = (_contentCard.Width - Padding.Horizontal - 48) / cols;

        int y = _symptomsGridTop;
        int col = 0;
        foreach (var symptom in cat.Symptoms)
        {
            var chip = new SymptomChip(symptom)
            {
                Width = colW,
                Location = new Point(col * (colW + 16), y),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            chip.Selected = _selected.Contains(symptom);
            chip.SelectionChanged += (s, _) =>
            {
                if (s is not SymptomChip sc) return;
                if (sc.Selected) _selected.Add(sc.Text);
                else _selected.Remove(sc.Text);
                UpdateCounter();
            };
            _contentCard.Controls.Add(chip);
            col++;
            if (col >= cols) { col = 0; y += 56; }
        }
        _contentCard.Height = 480;
    }

    private void UpdateCounter()
    {
        _countLabel.Text = $"Síntomas seleccionados: {_selected.Count}";
        _continueBtn.Enabled = _selected.Count > 0;
    }

    public override void OnNavigatedTo(NavState? state)
    {
        base.OnNavigatedTo(state);
        _selected.Clear();
        UpdateCounter();
        // Reconstruir chips para reflejar deselección
        int idx = 0, i = 0;
        foreach (Control c in _categoryHost.Controls)
        {
            if (c is CategoryChip chip && chip.Active) idx = i;
            i++;
        }
        SelectCategory(idx);
    }
}

/// <summary>Selector de categoría de síntomas (ícono degradado + nombre).</summary>
public sealed class CategoryChip : UserControl
{
    private readonly string _icon;
    private readonly Color _c1;
    private readonly Color _c2;
    private bool _active;
    private bool _hovered;

    public CategoryChip(string label, string icon, Color c1, Color c2, int count, bool active = false)
    {
        Text = label;
        _icon = icon;
        _c1 = c1;
        _c2 = c2;
        _active = active;
        Size = new Size(304, 66);
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // los rellenos son traslúcidos: requiere el fondo del padre
        AppTheme.ThemeChanged += (_, _) => Invalidate();
        _ = count; // el contador se muestra en el paint
    }

    public bool Active
    {
        get => _active;
        set { _active = value; Invalidate(); }
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using (var path = DrawingHelpers.RoundedRect(rect, 14))
        {
            using (var brush = new SolidBrush(_active ? AppTheme.WithAlpha(AppTheme.Primary, 16) : (_hovered ? AppTheme.Muted : AppTheme.Card)))
            {
                g.FillPath(brush, path);
            }
            DrawingHelpers.StrokeRounded(g, _active ? AppTheme.WithAlpha(AppTheme.Primary, 140) : AppTheme.Border, 1f, rect, 14);
        }

        // Caja de ícono degradada
        var iconBox = new Rectangle(14, (Height - 40) / 2, 40, 40);
        using (var ipath = DrawingHelpers.RoundedRect(iconBox, 10))
        using (var brush = new LinearGradientBrush(iconBox, _c1, _c2, 45f))
        {
            g.FillPath(brush, ipath);
        }
        using (var emoji = AppTheme.Emoji(11f))
        {
            DrawingHelpers.DrawEmoji(g, _icon, emoji, iconBox);
        }

        using (var font = AppTheme.Medium(9.5f))
        {
            TextRenderer.DrawText(g, Text, font, new Rectangle(66, 4, Width - 80, 28),
                AppTheme.Foreground, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        using (var small = AppTheme.Small(7.5f))
        {
            TextRenderer.DrawText(g, "Selecciona síntomas", small, new Rectangle(66, 30, Width - 80, 20),
                AppTheme.MutedForeground, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
    }
}

/// <summary>Píldora de síntoma seleccionable.</summary>
public sealed class SymptomChip : UserControl
{
    private bool _selected;
    private bool _hovered;

    public event EventHandler? SelectionChanged;

    public SymptomChip(string symptom)
    {
        Text = symptom;
        Height = 48;
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        AppTheme.ThemeChanged += (_, _) => Invalidate();
    }

    public bool Selected
    {
        get => _selected;
        set { _selected = value; Invalidate(); }
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        _selected = !_selected;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using (var path = DrawingHelpers.RoundedRect(rect, 12))
        {
            using (var brush = new SolidBrush(_selected ? AppTheme.WithAlpha(AppTheme.Primary, 18) : (_hovered ? AppTheme.Muted : AppTheme.Card)))
            {
                g.FillPath(brush, path);
            }
            DrawingHelpers.StrokeRounded(g, _selected ? AppTheme.WithAlpha(AppTheme.Primary, 160) : (_hovered ? AppTheme.WithAlpha(AppTheme.Primary, 110) : AppTheme.Border), 1.2f, rect, 12);
        }

        var radioRect = new Rectangle(Width - 28, (Height - 16) / 2, 16, 16);
        if (_selected)
        {
            using var brush = new SolidBrush(AppTheme.Primary);
            g.FillEllipse(brush, radioRect);
            // Aro interior: trazo inscrito para que quede completo y centrado.
            var dotRect = new Rectangle(radioRect.X + 4, radioRect.Y + 4, 8, 8);
            using (var p = new Pen(AppTheme.Card, 3f) { Alignment = PenAlignment.Inset })
            {
                g.DrawEllipse(p, dotRect);
            }
        }
        else
        {
            using var brush = new SolidBrush(Color.Transparent);
            g.FillEllipse(brush, radioRect);
            using var p = new Pen(AppTheme.MutedForeground, 1.2f) { Alignment = PenAlignment.Inset };
            g.DrawEllipse(p, radioRect);
        }

        using (var font = AppTheme.Body(9.5f))
        {
            var color = _selected ? AppTheme.Primary : AppTheme.Foreground;
            TextRenderer.DrawText(g, Text, font, new Rectangle(16, 0, Width - 52, Height), color,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }
    }
}

/// <summary>Barra de progreso fina con relleno primary.</summary>
public sealed class ProgressBarSkin : Control
{
    private int _percent;
    private Color _color;

    public ProgressBarSkin()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent; // sin caja gris detrás de la barra
        _color = AppTheme.Primary;
        AppTheme.ThemeChanged += (_, _) =>
        {
            if (!CustomColor) _color = AppTheme.Primary;
            Invalidate();
        };
    }

    /// <summary>Tamaño preferido real (evita colapso en filas AutoSize).</summary>
    public override Size GetPreferredSize(Size proposedSize) => new Size(Width, Height);

    public bool CustomColor { get; set; }
    public int Percent { get => _percent; set { _percent = Math.Clamp(value, 0, 100); Invalidate(); } }
    public Color Color
    {
        get => _color;
        set { _color = value; CustomColor = true; Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = DrawingHelpers.RoundedRect(rect, Height / 2))
        using (var brush = new SolidBrush(AppTheme.Muted))
        {
            g.FillPath(brush, path);
        }
        if (_percent > 0)
        {
            var fill = new Rectangle(0, 0, (int)(Width * _percent / 100f), Height);
            using var path = DrawingHelpers.RoundedRect(fill, Height / 2);
            using var brush = new SolidBrush(_color);
            g.FillPath(brush, path);
        }
    }
}