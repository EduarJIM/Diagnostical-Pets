using DiagnosticaTuMascota.Core;
using Xunit;

namespace DiagnosticaTuMascota.Tests;

public class AlarmServiceTests
{
    private static AlertItem Alert(string date = "2026-09-24", string time = "12:00") => new()
    {
        Id = 1,
        Severity = "Alta",
        Title = "Dosis",
        Pet = "Max",
        Message = "Dosis mensual",
        Date = date,
        Time = time
    };

    [Fact]
    public void Matches_SameDateAndTime_ReturnsTrue()
    {
        var now = new DateTime(2026, 9, 24, 12, 0, 0);

        Assert.True(AlarmService.Matches(Alert(), now));
    }

    [Fact]
    public void Matches_SameDateDifferentTime_ReturnsFalse()
    {
        var now = new DateTime(2026, 9, 24, 12, 5, 0);

        Assert.False(AlarmService.Matches(Alert(), now));
    }

    [Fact]
    public void Matches_DifferentDate_ReturnsFalse()
    {
        var now = new DateTime(2026, 9, 25, 12, 0, 0);

        Assert.False(AlarmService.Matches(Alert(), now));
    }

    [Fact]
    public void Matches_NullAlert_ReturnsFalse()
    {
        Assert.False(AlarmService.Matches(null, DateTime.Now));
    }

    [Fact]
    public void Matches_SingleDigitDay_PadsToTwoDigits()
    {
        // La alerta usa "2026-02-05" (con cero) y el momento es el día 5
        var now = new DateTime(2026, 2, 5, 8, 30, 0);

        Assert.True(AlarmService.Matches(Alert("2026-02-05", "08:30"), now));
    }

    [Fact]
    public void IsTimeToRing_WithDueAlert_ReturnsTrueAndReturnsAlert()
    {
        var now = new DateTime(2026, 9, 24, 12, 0, 0);
        var alerts = new List<AlertItem> { Alert(), Alert("2026-09-30", "09:00") };

        var rings = AlarmService.IsTimeToRing(alerts, now, out var due);

        Assert.True(rings);
        Assert.NotNull(due);
        Assert.Equal("Dosis", due!.Title);
    }

    [Fact]
    public void IsTimeToRing_NoDueAlert_ReturnsFalse()
    {
        var now = new DateTime(2026, 9, 24, 13, 0, 0);
        var alerts = new List<AlertItem> { Alert() };

        var rings = AlarmService.IsTimeToRing(alerts, now, out var due);

        Assert.False(rings);
        Assert.Null(due);
    }

    [Fact]
    public void IsTimeToRing_EmptyList_ReturnsFalse()
    {
        var rings = AlarmService.IsTimeToRing(Array.Empty<AlertItem>(), DateTime.Now, out var due);

        Assert.False(rings);
        Assert.Null(due);
    }
}