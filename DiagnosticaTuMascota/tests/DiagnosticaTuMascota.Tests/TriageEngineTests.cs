using DiagnosticaTuMascota.Core;
using Xunit;

namespace DiagnosticaTuMascota.Tests;

public class TriageEngineTests
{
    [Fact]
    public void Evaluate_EmptySymptoms_ReturnsLeveWithZeroScore()
    {
        var result = TriageEngine.Evaluate(Array.Empty<string>());

        Assert.Equal(SeverityLevel.LEVE, result.OverallSeverity);
        Assert.Equal(0, result.Score);
        Assert.Empty(result.Implications);
    }

    [Fact]
    public void Evaluate_NullSymptoms_ReturnsLeve()
    {
        var result = TriageEngine.Evaluate(null);

        Assert.Equal(SeverityLevel.LEVE, result.OverallSeverity);
    }

    [Fact]
    public void Evaluate_SingleLeveSymptom_ReturnsLeve()
    {
        var result = TriageEngine.Evaluate(new[] { "Estornudos" });

        Assert.Equal(SeverityLevel.LEVE, result.OverallSeverity);
        Assert.Equal(1, result.Score);
    }

    [Fact]
    public void Evaluate_SingleModerateSymptom_ReturnsModerado()
    {
        var result = TriageEngine.Evaluate(new[] { "Fiebre" });

        Assert.Equal(SeverityLevel.MODERADO, result.OverallSeverity);
        Assert.Equal(2, result.Score);
    }

    [Fact]
    public void Evaluate_SingleCriticalSymptom_ReturnsCritico()
    {
        var result = TriageEngine.Evaluate(new[] { "Convulsiones" });

        Assert.Equal(SeverityLevel.CRITICO, result.OverallSeverity);
        Assert.Equal(3, result.Score);
    }

    [Fact]
    public void Evaluate_VomitosAndDiarrea_ReturnsCriticoWithSynergyImplication()
    {
        var result = TriageEngine.Evaluate(new[] { "Vómitos", "Diarrea" });

        Assert.Equal(SeverityLevel.CRITICO, result.OverallSeverity);
        Assert.Contains(result.Implications, i => i.Contains("Sinergia", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Evaluate_TwoModerateSymptoms_ScoreReachesModerado()
    {
        // 2 + 2 = 4 → la regla score >= 4 eleva a MODERADO (aquí ya es MODERADO por peso, pero el score se suma)
        var result = TriageEngine.Evaluate(new[] { "Fiebre", "Tos persistente" });

        Assert.Equal(4, result.Score);
        Assert.Equal(SeverityLevel.MODERADO, result.OverallSeverity);
    }

    [Fact]
    public void Evaluate_UnknownSymptom_IsIgnored()
    {
        var result = TriageEngine.Evaluate(new[] { "Síntoma inexistente ABC" });

        Assert.Equal(SeverityLevel.LEVE, result.OverallSeverity);
        Assert.Equal(0, result.Score);
        Assert.Empty(result.Implications);
    }

    [Fact]
    public void Evaluate_DictionaryContainsAllTwentySymptoms()
    {
        Assert.Equal(20, TriageEngine.SymptomDictionary.Count);
    }

    [Fact]
    public void Evaluate_Implication_IsFormattedWithDash()
    {
        var result = TriageEngine.Evaluate(new[] { "Fiebre" });

        var implication = Assert.Single(result.Implications);
        Assert.StartsWith("- Fiebre:", implication);
        Assert.Contains("proceso infeccioso", implication);
    }

    [Theory]
    [InlineData(SeverityLevel.LEVE, "Monitoreo en casa")]
    [InlineData(SeverityLevel.MODERADO, "revisión veterinaria pronta")]
    [InlineData(SeverityLevel.CRITICO, "inmediata")]
    public void GetBaseRecommendation_MatchesSeverity(SeverityLevel level, string expectedPart)
    {
        var recommendation = TriageEngine.GetBaseRecommendation(level);

        Assert.Contains(expectedPart, recommendation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Evaluate_CriticalSymptomOverridesModerateScores()
    {
        // Fiebre (2) + Convulsiones (3) → CRÍTICO aunque la suma sea 5
        var result = TriageEngine.Evaluate(new[] { "Fiebre", "Convulsiones" });

        Assert.Equal(SeverityLevel.CRITICO, result.OverallSeverity);
    }
}