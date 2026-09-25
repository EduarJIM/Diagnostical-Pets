namespace DiagnosticaTuMascota.Core;

/// <summary>Datos de triaje de un síntoma según el diccionario médico.</summary>
public sealed record SymptomTriage(SeverityLevel Severity, string MedicalImplication, int Weight);

/// <summary>
/// Motor de triaje veterinario. Traduce una lista de síntomas en un nivel de
/// urgencia (LEVE / MODERADO / CRÍTICO), las implicaciones clínicas y un score.
/// Portado 1:1 de triageDictionary.ts de la versión web.
/// </summary>
public static class TriageEngine
{
    /// <summary>Diccionario oficial de síntomas (portado de triageDictionary.ts).</summary>
    public static readonly IReadOnlyDictionary<string, SymptomTriage> SymptomDictionary =
        new Dictionary<string, SymptomTriage>(StringComparer.Ordinal)
        {
            // Generales
            ["Fiebre"] = new(SeverityLevel.MODERADO, "Indica un proceso infeccioso o inflamatorio activo en el cuerpo.", 2),
            ["Letargo / Debilidad"] = new(SeverityLevel.MODERADO, "Signo inespecífico de enfermedad sistémica, dolor o fiebre.", 2),
            ["Pérdida de peso"] = new(SeverityLevel.MODERADO, "Posible problema metabólico, parasitario o crónico.", 2),
            ["Aumento de sed"] = new(SeverityLevel.MODERADO, "Puede indicar problemas renales, diabetes o infecciones uterinas (piómetra).", 2),
            ["Temblores"] = new(SeverityLevel.CRITICO, "Posible dolor agudo, fiebre muy alta, intoxicación o problema neurológico.", 3),

            // Respiratorios
            ["Tos persistente"] = new(SeverityLevel.MODERADO, "Podría indicar problemas cardíacos, infecciones respiratorias o colapso traqueal.", 2),
            ["Estornudos"] = new(SeverityLevel.LEVE, "Suele deberse a irritantes ambientales o infecciones respiratorias altas leves.", 1),
            ["Dificultad para respirar"] = new(SeverityLevel.CRITICO, "EMERGENCIA: Hipoxia inminente, posible fallo cardíaco, asma felino agudo o líquido en los pulmones.", 3),
            ["Secreción nasal"] = new(SeverityLevel.LEVE, "Infección del tracto respiratorio superior o presencia de un cuerpo extraño.", 1),
            ["Respiración ruidosa"] = new(SeverityLevel.MODERADO, "Obstrucción parcial de las vías respiratorias superiores.", 2),

            // Digestivos
            ["Vómitos"] = new(SeverityLevel.MODERADO, "Irritación gastrointestinal, cambios de dieta, ingestión de cuerpos extraños o fallo orgánico.", 2),
            ["Diarrea"] = new(SeverityLevel.MODERADO, "Gastroenteritis, parásitos intestinales o infecciones virales (ej. parvovirus en cachorros).", 2),
            ["Pérdida de apetito"] = new(SeverityLevel.LEVE, "Primer signo de incomodidad o enfermedad leve, aunque requiere monitoreo.", 1),
            ["Dificultad al tragar"] = new(SeverityLevel.CRITICO, "Posible cuerpo extraño esofágico, reacción alérgica o problema neurológico.", 3),
            ["Abdomen hinchado"] = new(SeverityLevel.CRITICO, "EMERGENCIA: Posible dilatación-torsión gástrica (GDV), hemorragia interna o peritonitis.", 3),

            // Neurológicos
            ["Convulsiones"] = new(SeverityLevel.CRITICO, "EMERGENCIA: Actividad eléctrica anormal en el cerebro por epilepsia, intoxicación o hipoglucemia severa.", 3),
            ["Desorientación"] = new(SeverityLevel.CRITICO, "Afección del sistema nervioso central, intoxicación o evento vascular.", 3),
            ["Incoordinación al caminar"] = new(SeverityLevel.CRITICO, "Síndrome vestibular, trauma espinal, hernia discal o toxinas.", 3),
            ["Parálisis"] = new(SeverityLevel.CRITICO, "EMERGENCIA: Lesión severa de la médula espinal (ej. hernia de disco tipo IVDD), garrapatas paralizantes o tromboembolismo.", 3),
            ["Cambio de conducta"] = new(SeverityLevel.MODERADO, "A menudo asociado a dolor silencioso, senilidad o problemas neurológicos tempranos.", 2)
        };

    /// <summary>Evalúa la lista de síntomas seleccionados.</summary>
    public static TriageResult Evaluate(IEnumerable<string>? selectedSymptoms)
    {
        var symptoms = selectedSymptoms?.ToList() ?? new List<string>();
        if (symptoms.Count == 0)
        {
            return new TriageResult { OverallSeverity = SeverityLevel.LEVE, Implications = new(), Score = 0 };
        }

        int maxWeight = 0;
        int score = 0;
        var implications = new List<string>();

        foreach (var symptom in symptoms)
        {
            if (SymptomDictionary.TryGetValue(symptom, out var data))
            {
                score += data.Weight;
                if (data.Weight > maxWeight) maxWeight = data.Weight;
                implications.Add($"- {symptom}: {data.MedicalImplication}");
            }
        }

        // Regla de sinergia adicional (igual que en la web)
        if (symptoms.Contains("Vómitos") && symptoms.Contains("Diarrea"))
        {
            maxWeight = 3;
            implications.Add("- Sinergia (Vómitos + Diarrea): ALTO RIESGO de deshidratación rápida y posible origen infeccioso grave (ej. Parvovirus).");
        }

        SeverityLevel overall;
        if (maxWeight == 3) overall = SeverityLevel.CRITICO;
        else if (maxWeight == 2 || score >= 4) overall = SeverityLevel.MODERADO;
        else overall = SeverityLevel.LEVE;

        return new TriageResult { OverallSeverity = overall, Implications = implications, Score = score };
    }

    /// <summary>Recomendación base según el nivel de severidad global.</summary>
    public static string GetBaseRecommendation(SeverityLevel severity) => severity switch
    {
        SeverityLevel.CRITICO => "Atención veterinaria inmediata requerida. Riesgo vital.",
        SeverityLevel.MODERADO => "Se sugiere revisión veterinaria pronta para evitar complicaciones.",
        _ => "Monitoreo en casa recomendado."
    };
}