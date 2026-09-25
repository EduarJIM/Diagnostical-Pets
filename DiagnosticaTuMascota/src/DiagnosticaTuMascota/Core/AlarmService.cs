namespace DiagnosticaTuMascota.Core;

/// <summary>
/// Lógica de alarmas: decide si una alerta debe sonar en un momento concreto
/// comparando fecha (yyyy-MM-dd) y hora (HH:mm), igual que useAlarms.ts.
/// </summary>
public static class AlarmService
{
    /// <summary>Indica si la alerta debe sonar en el momento dado.</summary>
    public static bool Matches(AlertItem? alert, DateTime now)
    {
        if (alert is null) return false;
        var dateStr = now.ToString("yyyy-MM-dd");
        var timeStr = now.ToString("HH:mm");
        return string.Equals(alert.Date, dateStr, StringComparison.Ordinal)
            && string.Equals(alert.Time, timeStr, StringComparison.Ordinal);
    }

    /// <summary>Busca la primera alerta que debe sonar en este instante.</summary>
    public static bool IsTimeToRing(IEnumerable<AlertItem> alerts, DateTime now, out AlertItem? due)
    {
        due = alerts.FirstOrDefault(a => Matches(a, now));
        return due is not null;
    }
}