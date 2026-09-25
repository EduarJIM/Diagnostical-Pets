namespace DiagnosticaTuMascota.Core;

/// <summary>Niveles de severidad del triaje. LEVE=1, MODERADO=2, CRÍTICO=3.</summary>
public enum SeverityLevel
{
    LEVE = 1,
    MODERADO = 2,
    CRITICO = 3
}

public static class SeverityExtensions
{
    public static string ToLabel(this SeverityLevel level) => level switch
    {
        SeverityLevel.LEVE => "LEVE",
        SeverityLevel.MODERADO => "MODERADO",
        SeverityLevel.CRITICO => "CRÍTICO",
        _ => level.ToString()
    };
}

public sealed class UserAccount
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class PetData
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Age { get; set; } = "";
    public string Breed { get; set; } = "";
    public string Sex { get; set; } = "";
    public string Vaccines { get; set; } = "";
    public string History { get; set; } = "";
    public string Clinical { get; set; } = "";
    public string Vet { get; set; } = "";
}

public sealed class HistoryItem
{
    public long Id { get; set; }
    public string Date { get; set; } = "";
    public string Pet { get; set; } = "";
    public string Status { get; set; } = "Completado";
    public string Result { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Recommendation { get; set; } = "";
}

public sealed class UpcomingItem
{
    public long Id { get; set; }
    public string Date { get; set; } = "";
    public string Pet { get; set; } = "";
    public string Type { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Action { get; set; } = "";
}

public sealed class AlertItem
{
    public long Id { get; set; }
    public string Severity { get; set; } = "Media";
    public string Title { get; set; } = "";
    public string Pet { get; set; } = "";
    public string Message { get; set; } = "";
    /// <summary>Fecha en formato yyyy-MM-dd.</summary>
    public string Date { get; set; } = "";
    /// <summary>Hora en formato HH:mm.</summary>
    public string Time { get; set; } = "";
}

public sealed class TriageResult
{
    public SeverityLevel OverallSeverity { get; set; } = SeverityLevel.LEVE;
    public List<string> Implications { get; set; } = new();
    public int Score { get; set; }
}