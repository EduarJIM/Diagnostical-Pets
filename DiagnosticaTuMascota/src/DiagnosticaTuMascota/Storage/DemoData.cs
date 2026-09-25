using DiagnosticaTuMascota.Core;

namespace DiagnosticaTuMascota.Storage;

/// <summary>
/// Datos de ejemplo iniciales equivalentes a los definidos en la versión web
/// (Pets.tsx, History.tsx): mascotas Max y Luna, una consulta histórica, un
/// seguimiento programado y una alerta activa.
/// </summary>
public static class DemoData
{
    public static List<PetData> CreateInitialPets() => new()
    {
        new PetData
        {
            Id = 1,
            Name = "Max",
            Age = "3 años",
            Breed = "Golden Retriever",
            Sex = "Macho",
            Vaccines = "Rabia, Parvovirus, Moquillo (Al día)",
            History = "Ninguna enfermedad grave previa.",
            Clinical = "Alergia leve al pollo.",
            Vet = "Dr. Ramírez (Clínica VetSalud)"
        },
        new PetData
        {
            Id = 2,
            Name = "Luna",
            Age = "1 año",
            Breed = "Gato Siamés",
            Sex = "Hembra",
            Vaccines = "Triple Felina (Al día)",
            History = "Infección urinaria leve hace 6 meses.",
            Clinical = "Saludable.",
            Vet = "Dra. Silva (Centro Felino)"
        }
    };

    public static List<HistoryItem> CreateInitialHistory() => new()
    {
        new HistoryItem
        {
            Id = 1,
            Date = "14 de Mayo, 2024",
            Pet = "Max",
            Status = "Completado",
            Result = "Caso Leve",
            Summary = "Se observó letargo leve y falta de apetito. No hay signos de distress respiratorio.",
            Recommendation = "Observación por 24 horas. Mantener hidratación. Si los síntomas persisten, acudir a consulta física."
        }
    };

    public static List<UpcomingItem> CreateInitialUpcoming() => new()
    {
        new UpcomingItem
        {
            Id = 1,
            Date = "20 de Mayo, 2024",
            Pet = "Max",
            Type = "Seguimiento",
            Summary = "Revisión de evolución tras tratamiento por alergia leve.",
            Action = "Cita Programada"
        }
    };

    public static List<AlertItem> CreateInitialAlerts() => new()
    {
        new AlertItem
        {
            Id = 1,
            Severity = "Alta",
            Title = "Dosis de Desparasitante",
            Pet = "Max",
            Message = "Hoy corresponde la dosis mensual de desparasitante oral.",
            Date = DateTime.Today.ToString("yyyy-MM-dd"),
            Time = "12:00"
        }
    };
}