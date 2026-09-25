using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiagnosticaTuMascota.Theme;

namespace DiagnosticaTuMascota.Controls;

/// <summary>Estilos de texto predefinidos vinculados al tema (fuente + color).</summary>
public enum TextKind
{
    Display, Title, Heading, SubHeading, Body, BodyStrong, Small,
    Muted, MutedSmall, TinyMuted, Primary, PrimarySmall, Destructive, DestructiveSmall
}

/// <summary>
/// Label que se adapta automáticamente al tema actual (colores y fuentes).
/// Reemplaza al sistema de clases de Tailwind (text-foreground, text-muted-foreground, etc.).
/// </summary>
public class ThemeLabel : Label
{
    private TextKind _textKind = TextKind.Body;

    public ThemeLabel()
    {
        AutoSize = true;
        UseMnemonic = false;
        AppTheme.ThemeChanged += (_, _) => ApplyTheme();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public TextKind TextKind
    {
        get => _textKind;
        set { _textKind = value; ApplyTheme(); }
    }

    /// <summary>
    /// AutoEllipsis solo funciona con render GDI (UseCompatibleTextRendering = false).
    /// Cuando el texto contiene emoji usamos GDI+ para que se dibujen a color, por lo que
    /// se desactiva la elipsis para evitar texto superpuesto.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public new bool AutoEllipsis
    {
        get => base.AutoEllipsis;
        set
        {
            if (value && DrawingHelpers.ContainsEmoji(Text)) return; // GDI+ no soporta AutoEllipsis
            base.AutoEllipsis = value;
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        RefreshTextRendering();
    }

    /// <summary>Cambia a GDI+ cuando el texto contiene emoji para renderizarlos a color.</summary>
    private void RefreshTextRendering()
    {
        bool hasEmoji = DrawingHelpers.ContainsEmoji(Text);
        UseCompatibleTextRendering = hasEmoji;
        if (hasEmoji) base.AutoEllipsis = false;
    }

    /// <summary>Aplica colores y fuentes según el tema actual.</summary>
    public void ApplyTheme()
    {
        switch (_textKind)
        {
            case TextKind.Display: Font = AppTheme.Heading(24f); ForeColor = AppTheme.Foreground; break;
            case TextKind.Title: Font = AppTheme.Heading(18f); ForeColor = AppTheme.Foreground; break;
            case TextKind.Heading: Font = AppTheme.Medium(13f); ForeColor = AppTheme.Foreground; break;
            case TextKind.SubHeading: Font = AppTheme.Medium(11f); ForeColor = AppTheme.Foreground; break;
            case TextKind.Body: Font = AppTheme.Body(10f); ForeColor = AppTheme.Foreground; break;
            case TextKind.BodyStrong: Font = AppTheme.Medium(10f); ForeColor = AppTheme.Foreground; break;
            case TextKind.Small: Font = AppTheme.Small(8.5f); ForeColor = AppTheme.Foreground; break;
            case TextKind.Muted: Font = AppTheme.Body(10f); ForeColor = AppTheme.MutedForeground; break;
            case TextKind.MutedSmall: Font = AppTheme.Small(8.5f); ForeColor = AppTheme.MutedForeground; break;
            case TextKind.TinyMuted: Font = AppTheme.Tiny(7.5f); ForeColor = AppTheme.MutedForeground; break;
            case TextKind.Primary: Font = AppTheme.Medium(10f); ForeColor = AppTheme.Primary; break;
            case TextKind.PrimarySmall: Font = AppTheme.Medium(8.5f); ForeColor = AppTheme.Primary; break;
            case TextKind.Destructive: Font = AppTheme.Medium(10f); ForeColor = AppTheme.Destructive; break;
            case TextKind.DestructiveSmall: Font = AppTheme.Medium(8.5f); ForeColor = AppTheme.Destructive; break;
        }
        Invalidate();
    }
}