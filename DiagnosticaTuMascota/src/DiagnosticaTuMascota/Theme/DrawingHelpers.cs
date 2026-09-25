using System.Drawing.Drawing2D;

namespace DiagnosticaTuMascota.Theme;

/// <summary>Helpers de dibujo GDI+ (esquinas redondeadas, degradados, textos centrados).</summary>
public static class DrawingHelpers
{
    /// <summary>Crea la ruta de un rectángulo de esquinas redondeadas.</summary>
    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        // Clampar el radio a la mitad del lado menor: evita que la silueta "salte" a un
        // rectángulo plano cuando el radio pedido excede el tamaño del control.
        radius = Math.Max(0, Math.Min(radius, Math.Min(bounds.Width / 2, bounds.Height / 2)));
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            path.CloseFigure();
            return path;
        }

        int diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);           // Esquina superior izquierda
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);           // Superior derecha
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);             // Inferior derecha
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);            // Inferior izquierda
        path.CloseFigure();
        return path;
    }

    /// <summary>Rellenar un rectángulo redondeado con color sólido.</summary>
    public static void FillRounded(Graphics g, Color color, Rectangle bounds, int radius)
    {
        using var path = RoundedRect(bounds, radius);
        using var brush = new SolidBrush(color);
        g.FillPath(brush, path);
    }

    /// <summary>
    /// Dibuja el contorno redondeado con el trazo completamente inscrito dentro de los
    /// límites del control. GDI+ centra el pen sobre la ruta: sin inscribirlo, la mitad
    /// del trazo queda fuera y se recorta en el borde → grosor disparejo (grueso arriba/
    /// izquierda, fino abajo/derecha) y esquinas "cortadas". Este helper garantiza
    /// siluetas limpias y uniformes en botones, campos, chips y tarjetas.
    /// </summary>
    public static void StrokeRounded(Graphics g, Color color, float width, Rectangle bounds, int radius)
    {
        if (width <= 0 || bounds.Width <= 0 || bounds.Height <= 0) return;
        int half = (int)Math.Ceiling(width / 2f);
        var r = new Rectangle(bounds.X + half, bounds.Y + half, bounds.Width - half * 2, bounds.Height - half * 2);
        if (r.Width <= 0 || r.Height <= 0) return;
        int rr = Math.Max(0, radius - half);
        using var pen = new Pen(color, width);
        using var path = RoundedRect(r, rr);
        g.DrawPath(pen, path);
    }

    /// <summary>Dibujar el contorno de un rectángulo redondeado (borde inscrito y uniforme).</summary>
    public static void DrawRounded(Graphics g, Color color, float width, Rectangle bounds, int radius)
    {
        StrokeRounded(g, color, width, bounds, radius);
    }

    /// <summary>Rellenar un rectángulo redondeado con degradado lineal.</summary>
    public static void FillGradientRounded(Graphics g, Color c1, Color c2, Rectangle bounds, int radius, float angle = 45f)
    {
        using var path = RoundedRect(bounds, radius);
        using var brush = new LinearGradientBrush(bounds, c1, c2, angle);
        g.FillPath(brush, path);
    }

    /// <summary>Texto horizontal y verticalmente centrado en el rectángulo.</summary>
    public static void DrawCenteredText(Graphics g, string text, Font font, Color color, Rectangle rect)
    {
        TextRenderer.DrawText(g, text, font, rect, color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    /// <summary>
    /// Dibuja un emoji A COLOR con GDI+ (Graphics.DrawString). TextRenderer/GDI dibuja
    /// los emoji en monocromo o como recuadros; GDI+ los renderiza a color en Windows 10/11.
    /// El color del pincel solo aplica a símbolos monocromos (☀/☾); los emoji de color
    /// usan su paleta integrada.
    /// </summary>
    public static void DrawEmoji(Graphics g, string? emoji, Font font, Rectangle rect, Color? tint = null)
    {
        if (string.IsNullOrEmpty(emoji)) return;
        using var fmt = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoClip
        };
        using var brush = new SolidBrush(tint ?? Color.White);
        // Ajuste fino óptico: los glifos emoji suelen quedar ligeramente bajos con GDI+.
        var r = new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
        g.DrawString(emoji, font, brush, r, fmt);
    }

    /// <summary>
    /// Detecta si una cadena contiene emoji (pares sustitutos o símbolos BMP del rango emoji).
    /// Útil para decidir entre render GDI (nítido) y GDI+ (emoji a color).
    /// </summary>
    public static bool ContainsEmoji(string? text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        foreach (char c in text)
        {
            if (char.IsSurrogate(c) || c >= 0x2600) return true;
        }
        return false;
    }

    /// <summary>Fuente emoji predeterminada para iconos.</summary>
    public static Font DefaultEmojiFont(float size = 12f) => new("Segoe UI Emoji", size, FontStyle.Regular, GraphicsUnit.Point);

    /// <summary>
    /// Dibuja una "sombra suave" simulada: varios contornos redondeados con alpha creciente.
    /// </summary>
    public static void DrawSoftShadow(Graphics g, Color shadowColor, Rectangle bounds, int radius, int depth = 6)
    {
        for (int i = depth; i >= 1; i--)
        {
            int alpha = 4 + (depth - i) * 2;
            if (alpha > 32) alpha = 32;
            var rect = new Rectangle(bounds.X + i, bounds.Y + i, bounds.Width, bounds.Height);
            using var pen = new Pen(Color.FromArgb(alpha, shadowColor), 1f);
            using var path = RoundedRect(rect, radius);
            g.DrawPath(pen, path);
        }
    }
}