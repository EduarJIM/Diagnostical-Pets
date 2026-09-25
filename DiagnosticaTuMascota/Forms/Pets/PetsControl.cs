using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;
using DiagnosticaTuMascota.Controls;

namespace DiagnosticaTuMascota.Forms.Pets;

public partial class PetsControl : UserControl
{
    private readonly List<PetData> _pets = new()
    {
        new PetData { Name = "Max", Species = "Golden Retriever", Age = "3 anos", Sex = "Macho", Vaccines = "Rabia, Parvovirus, Moquillo (Al dia)", Vet = "Dr. Ramirez (Clinica VetSalud)" },
        new PetData { Name = "Luna", Species = "Gato Sames", Age = "1 ano", Sex = "Hembra", Vaccines = "Triple Felina (Al dia)", Vet = "Dra. Silva (Centro Felino)" }
    };

    public PetsControl()
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

        var contentPanel = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            BackColor = Color.Transparent,
            MinimumSize = new Size(400, 0)
        };

        int y = 0;

        contentPanel.Controls.Add(new Label { Text = "\U0001F43E  Mis Mascotas", Font = AppTheme.TitleH1, ForeColor = AppTheme.Foreground, AutoSize = true, Location = new Point(0, y) });
        y += 48;

        contentPanel.Controls.Add(new Label { Text = "Gestiona el perfil de tus mascotas y su informacion clinica.", Font = AppTheme.Body, ForeColor = AppTheme.MutedForeground, AutoSize = true, Location = new Point(0, y) });
        y += 36;

        var addBtn = new RoundedButton { Text = "+  Agregar Mascota", Location = new Point(0, y), Size = new Size(200, 42), CornerRadius = 12, ButtonColor1 = AppTheme.Primary, ButtonColor2 = AppTheme.Secondary, UseGradient = true, TextColor = Color.White, Font = AppTheme.FontBold(13f) };
        contentPanel.Controls.Add(addBtn);
        y += 56;

        int cardWidth = 320;
        int cardHeight = 190;
        int gap = 20;
        int currentX = 0;
        int currentY = y;

        foreach (var pet in _pets)
        {
            var petCard = CreatePetCard(pet, cardWidth, cardHeight);
            petCard.Location = new Point(currentX, currentY);
            contentPanel.Controls.Add(petCard);
            currentX += cardWidth + gap;
            if (currentX + cardWidth > 700)
            {
                currentX = 0;
                currentY += cardHeight + gap;
            }
        }

        Controls.Add(contentPanel);
    }

    private RoundedPanel CreatePetCard(PetData pet, int width, int height)
    {
        var card = new RoundedPanel
        {
            Size = new Size(width, height),
            BackColor = AppTheme.Card,
            BorderColor = AppTheme.Border,
            BorderWidth = 1,
            CornerRadius = 18,
            Cursor = Cursors.Hand
        };

        card.Paint += (s, e) =>
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle iconBounds = new(16, 16, 46, 46);
            AppTheme.DrawRoundedRectangle(g, iconBounds, 12, Color.FromArgb(25, AppTheme.Primary));
            using Font iconFont = new("Segoe UI Symbol", 22f);
            using SolidBrush iconBrush = new(AppTheme.Primary);
            SizeF iconSize = g.MeasureString("\U0001F43E", iconFont);
            g.DrawString("\U0001F43E", iconFont, iconBrush, iconBounds.X + (46 - iconSize.Width) / 2, iconBounds.Y + (46 - iconSize.Height) / 2);

            using Font nameFont = AppTheme.FontBold(16f);
            using SolidBrush nameBrush = new(AppTheme.Foreground);
            g.DrawString(pet.Name, nameFont, nameBrush, 72, 18);

            using Font infoFont = new("Segoe UI", 10.5f);
            using SolidBrush infoBrush = new(AppTheme.MutedForeground);
            g.DrawString($"{pet.Species} - {pet.Age}", infoFont, infoBrush, 72, 40);

            int infoY = 72;
            using Font labelFont = AppTheme.FontBold(10f);
            using SolidBrush labelBrush = new(AppTheme.MutedForeground);
            using Font valueFont = new("Segoe UI", 10f);
            using SolidBrush valueBrush = new(AppTheme.Foreground);

            g.DrawString("Vacunas:", labelFont, labelBrush, 16, infoY);
            SizeF lbl = g.MeasureString("Vacunas:", labelFont);
            g.DrawString(pet.Vaccines, valueFont, valueBrush, 16 + lbl.Width + 4, infoY);
            infoY += 20;

            g.DrawString("Veterinario:", labelFont, labelBrush, 16, infoY);
            lbl = g.MeasureString("Veterinario:", labelFont);
            g.DrawString(pet.Vet, valueFont, valueBrush, 16 + lbl.Width + 4, infoY);

            using Pen sepPen = new(AppTheme.Border, 1);
            g.DrawLine(sepPen, 16, height - 44, width - 16, height - 44);

            using Font btnFont = AppTheme.FontBold(11f);
            using SolidBrush btnBrush = new(AppTheme.Primary);
            g.DrawString("Ver perfil completo \u2192", btnFont, btnBrush, 16, height - 32);
        };

        return card;
    }
}

public class PetData
{
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public string Age { get; set; } = "";
    public string Sex { get; set; } = "";
    public string Vaccines { get; set; } = "";
    public string Vet { get; set; } = "";
}
