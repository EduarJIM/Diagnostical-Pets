export type SeverityLevel = 'CRÍTICO' | 'MODERADO' | 'LEVE';

export interface TriageData {
  severity: SeverityLevel;
  medicalImplication: string;
  weight: number; // 3 = Critical, 2 = Moderate, 1 = Mild
}

export const SYMPTOM_DICTIONARY: Record<string, TriageData> = {
  // Generales
  'Fiebre': { severity: 'MODERADO', medicalImplication: 'Indica un proceso infeccioso o inflamatorio activo en el cuerpo.', weight: 2 },
  'Letargo / Debilidad': { severity: 'MODERADO', medicalImplication: 'Signo inespecífico de enfermedad sistémica, dolor o fiebre.', weight: 2 },
  'Pérdida de peso': { severity: 'MODERADO', medicalImplication: 'Posible problema metabólico, parasitario o crónico.', weight: 2 },
  'Aumento de sed': { severity: 'MODERADO', medicalImplication: 'Puede indicar problemas renales, diabetes o infecciones uterinas (piómetra).', weight: 2 },
  'Temblores': { severity: 'CRÍTICO', medicalImplication: 'Posible dolor agudo, fiebre muy alta, intoxicación o problema neurológico.', weight: 3 },

  // Respiratorios
  'Tos persistente': { severity: 'MODERADO', medicalImplication: 'Podría indicar problemas cardíacos, infecciones respiratorias o colapso traqueal.', weight: 2 },
  'Estornudos': { severity: 'LEVE', medicalImplication: 'Suele deberse a irritantes ambientales o infecciones respiratorias altas leves.', weight: 1 },
  'Dificultad para respirar': { severity: 'CRÍTICO', medicalImplication: 'EMERGENCIA: Hipoxia inminente, posible fallo cardíaco, asma felino agudo o líquido en los pulmones.', weight: 3 },
  'Secreción nasal': { severity: 'LEVE', medicalImplication: 'Infección del tracto respiratorio superior o presencia de un cuerpo extraño.', weight: 1 },
  'Respiración ruidosa': { severity: 'MODERADO', medicalImplication: 'Obstrucción parcial de las vías respiratorias superiores.', weight: 2 },

  // Digestivos
  'Vómitos': { severity: 'MODERADO', medicalImplication: 'Irritación gastrointestinal, cambios de dieta, ingestión de cuerpos extraños o fallo orgánico.', weight: 2 },
  'Diarrea': { severity: 'MODERADO', medicalImplication: 'Gastroenteritis, parásitos intestinales o infecciones virales (ej. parvovirus en cachorros).', weight: 2 },
  'Pérdida de apetito': { severity: 'LEVE', medicalImplication: 'Primer signo de incomodidad o enfermedad leve, aunque requiere monitoreo.', weight: 1 },
  'Dificultad al tragar': { severity: 'CRÍTICO', medicalImplication: 'Posible cuerpo extraño esofágico, reacción alérgica o problema neurológico.', weight: 3 },
  'Abdomen hinchado': { severity: 'CRÍTICO', medicalImplication: 'EMERGENCIA: Posible dilatación-torsión gástrica (GDV), hemorragia interna o peritonitis.', weight: 3 },

  // Neurológicos
  'Convulsiones': { severity: 'CRÍTICO', medicalImplication: 'EMERGENCIA: Actividad eléctrica anormal en el cerebro por epilepsia, intoxicación o hipoglucemia severa.', weight: 3 },
  'Desorientación': { severity: 'CRÍTICO', medicalImplication: 'Afección del sistema nervioso central, intoxicación o evento vascular.', weight: 3 },
  'Incoordinación al caminar': { severity: 'CRÍTICO', medicalImplication: 'Síndrome vestibular, trauma espinal, hernia discal o toxinas.', weight: 3 },
  'Parálisis': { severity: 'CRÍTICO', medicalImplication: 'EMERGENCIA: Lesión severa de la médula espinal (ej. hernia de disco tipo IVDD), garrapatas paralizantes o tromboembolismo.', weight: 3 },
  'Cambio de conducta': { severity: 'MODERADO', medicalImplication: 'A menudo asociado a dolor silencioso, senilidad o problemas neurológicos tempranos.', weight: 2 },
};

export function evaluateSymptoms(selectedSymptoms: string[]) {
  if (!selectedSymptoms || selectedSymptoms.length === 0) {
    return { overallSeverity: 'LEVE', implications: [], score: 0 };
  }

  let maxWeight = 0;
  const implications: string[] = [];
  let score = 0;

  selectedSymptoms.forEach(symptom => {
    const data = SYMPTOM_DICTIONARY[symptom];
    if (data) {
      score += data.weight;
      if (data.weight > maxWeight) maxWeight = data.weight;
      implications.push(`- ${symptom}: ${data.medicalImplication}`);
    }
  });

  // Reglas adicionales de sinergia
  if (selectedSymptoms.includes('Vómitos') && selectedSymptoms.includes('Diarrea')) {
    maxWeight = 3; // Riesgo grave de deshidratación
    implications.push('- Sinergia (Vómitos + Diarrea): ALTO RIESGO de deshidratación rápida y posible origen infeccioso grave (ej. Parvovirus).');
  }

  let overallSeverity: SeverityLevel = 'LEVE';
  if (maxWeight === 3) overallSeverity = 'CRÍTICO';
  else if (maxWeight === 2 || score >= 4) overallSeverity = 'MODERADO'; // Varios moderados = Moderado/Crítico

  return { overallSeverity, implications, score };
}
