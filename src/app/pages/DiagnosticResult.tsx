import { motion } from 'motion/react';
import { useLocation, useNavigate } from 'react-router';
import { useEffect, useRef } from 'react';
import { AlertCircle, ArrowLeft, Home, Activity, CheckCircle2, AlertTriangle, Thermometer } from 'lucide-react';
import { evaluateSymptoms } from '../utils/triageDictionary';
import { getStorageData, setStorageData } from '../utils/storage';
import { HistoryItem } from './History';

export function DiagnosticResult() {
  const location = useLocation();
  const navigate = useNavigate();
  const { petName = 'Tu Mascota', selectedSymptoms = [], additionalInfo = '' } = location.state || {};

  // Si se ingresa directamente sin datos, regresar al dashboard
  if (selectedSymptoms.length === 0) {
    navigate('/');
    return null;
  }

  const triage = evaluateSymptoms(selectedSymptoms);

  const isCritical = triage.overallSeverity === 'CRÍTICO';
  const isModerate = triage.overallSeverity === 'MODERADO';

  const themeColor = isCritical ? 'red' : isModerate ? 'amber' : 'emerald';
  const bgColor = isCritical ? 'bg-red-500' : isModerate ? 'bg-amber-500' : 'bg-emerald-500';
  const textDarkColor = isCritical ? 'text-red-600 dark:text-red-400' : isModerate ? 'text-amber-600 dark:text-amber-400' : 'text-emerald-600 dark:text-emerald-400';
  const bgLightColor = isCritical ? 'bg-red-500/10' : isModerate ? 'bg-amber-500/10' : 'bg-emerald-500/10';
  const borderColor = isCritical ? 'border-red-500/20' : isModerate ? 'border-amber-500/20' : 'border-emerald-500/20';

  const savedRef = useRef(false);

  useEffect(() => {
    if (savedRef.current || selectedSymptoms.length === 0) return;
    
    const historyData = getStorageData<HistoryItem[]>('historyData', []);
    
    // Crear recomendación base
    let baseRec = 'Monitoreo en casa recomendado.';
    if (isCritical) baseRec = 'Atención veterinaria inmediata requerida. Riesgo vital.';
    else if (isModerate) baseRec = 'Se sugiere revisión veterinaria pronta para evitar complicaciones.';

    const newItem: HistoryItem = {
      id: Date.now(),
      date: new Date().toLocaleDateString('es-ES', { year: 'numeric', month: 'long', day: 'numeric' }),
      pet: petName,
      status: 'Completado',
      result: triage.overallSeverity,
      summary: `Síntomas: ${selectedSymptoms.join(', ')}.\n${additionalInfo ? `Notas: ${additionalInfo}` : ''}`,
      recommendation: baseRec
    };
    
    setStorageData('historyData', [newItem, ...historyData]);
    window.dispatchEvent(new Event('storage'));
    
    savedRef.current = true;
  }, [petName, selectedSymptoms, additionalInfo, triage.overallSeverity, isCritical, isModerate]);

  return (
    <div className="flex-1 h-screen overflow-auto bg-background p-8 md:p-12 transition-colors duration-500 relative">
      {/* Background glow matching severity */}
      <div className={`absolute top-0 left-0 w-full h-96 ${bgColor} opacity-[0.03] blur-3xl -z-10`} />

      <div className="max-w-4xl mx-auto">
        <header className="mb-10">
          <button 
            onClick={() => navigate('/')}
            className="flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors group mb-6"
          >
            <ArrowLeft size={20} className="group-hover:-translate-x-1 transition-transform" />
            Volver al inicio
          </button>
          <h1 className="text-4xl text-foreground font-black mb-3">
            Análisis Clínico de {petName}
          </h1>
          <p className="text-muted-foreground text-lg">
            Evaluación preliminar basada en protocolos de triaje veterinario.
          </p>
        </header>

        <motion.div
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          className={`rounded-[2rem] border-2 ${borderColor} ${bgLightColor} p-8 md:p-10 mb-8 flex flex-col md:flex-row items-center gap-8 shadow-2xl relative overflow-hidden`}
        >
          {/* Animated background element */}
          <motion.div 
            animate={{ scale: [1, 1.05, 1], opacity: [0.5, 0.8, 0.5] }}
            transition={{ duration: 4, repeat: Infinity }}
            className={`absolute -right-20 -top-20 w-64 h-64 rounded-full ${bgColor} blur-3xl opacity-20`}
          />

          <div className={`w-32 h-32 rounded-full ${bgColor} flex items-center justify-center flex-shrink-0 text-white shadow-xl shadow-${themeColor}-500/30 z-10`}>
            {isCritical ? <AlertTriangle size={64} /> : isModerate ? <Thermometer size={64} /> : <CheckCircle2 size={64} />}
          </div>

          <div className="text-center md:text-left z-10">
            <div className={`inline-block px-4 py-1.5 rounded-full ${bgColor} text-white text-sm font-bold tracking-widest uppercase mb-4`}>
              Nivel de Urgencia: {triage.overallSeverity}
            </div>
            <h2 className="text-3xl font-bold text-foreground mb-4">
              {isCritical && "Atención Veterinaria Inmediata Requerida"}
              {isModerate && "Se Sugiere Revisión Veterinaria Pronta"}
              {!isCritical && !isModerate && "Monitoreo en Casa Recomendado"}
            </h2>
            <p className="text-foreground/80 text-lg leading-relaxed">
              {isCritical && "Los síntomas reportados representan un riesgo potencial de vida o daño orgánico severo. No intentes tratamientos caseros. Dirígete a la clínica de emergencias más cercana."}
              {isModerate && "Tu mascota presenta signos de incomodidad o enfermedad sistémica que requieren evaluación médica para evitar complicaciones."}
              {!isCritical && !isModerate && "Los síntomas parecen leves o transitorios. Mantén a tu mascota en observación durante 24-48 horas. Si los síntomas persisten o empeoran, acude al veterinario."}
            </p>
          </div>
        </motion.div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
            className="bg-card border border-border rounded-3xl p-8 shadow-sm"
          >
            <h3 className="text-xl font-bold text-foreground mb-6 flex items-center gap-3">
              <Activity className={textDarkColor} size={24} />
              Implicaciones Clínicas
            </h3>
            <div className="space-y-4">
              {triage.implications.map((imp, idx) => (
                <div key={idx} className="flex items-start gap-3">
                  <div className={`mt-1.5 w-2 h-2 rounded-full flex-shrink-0 ${bgColor}`} />
                  <p className="text-foreground/90">{imp.replace('- ', '')}</p>
                </div>
              ))}
            </div>
            {additionalInfo && (
              <div className="mt-6 pt-6 border-t border-border">
                <span className="text-sm font-bold text-muted-foreground block mb-2">Notas Adicionales del Propietario:</span>
                <p className="text-foreground/80 italic">"{additionalInfo}"</p>
              </div>
            )}
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
            className="bg-card border border-border rounded-3xl p-8 shadow-sm flex flex-col"
          >
            <h3 className="text-xl font-bold text-foreground mb-6 flex items-center gap-3">
              <AlertCircle className={textDarkColor} size={24} />
              Primeros Auxilios / Acciones
            </h3>
            <ul className="space-y-4 text-foreground/90 flex-1">
              {isCritical && (
                <>
                  <li className="flex items-center gap-3"><span className="font-bold text-red-500">•</span> Mantén la calma y contacta a tu veterinario de inmediato.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-red-500">•</span> Minimiza el estrés y el movimiento de la mascota.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-red-500">•</span> No administres medicamentos humanos bajo ninguna circunstancia.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-red-500">•</span> Si hay dificultad respiratoria, asegúrate de que el área esté bien ventilada.</li>
                </>
              )}
              {isModerate && (
                <>
                  <li className="flex items-center gap-3"><span className="font-bold text-amber-500">•</span> Programa una cita veterinaria para hoy o mañana.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-amber-500">•</span> Ofrécele agua fresca, pero no la fuerces a beber o comer.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-amber-500">•</span> Mantén a tu mascota en un lugar tranquilo y cómodo.</li>
                </>
              )}
              {!isCritical && !isModerate && (
                <>
                  <li className="flex items-center gap-3"><span className="font-bold text-emerald-500">•</span> Observa de cerca su comportamiento durante las próximas horas.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-emerald-500">•</span> Asegúrate de que tenga acceso a agua fresca.</li>
                  <li className="flex items-center gap-3"><span className="font-bold text-emerald-500">•</span> Evita cambios bruscos en su dieta actual.</li>
                </>
              )}
            </ul>
          </motion.div>
        </div>

        <div className="mt-8 p-6 bg-muted/50 rounded-2xl border border-border text-center">
          <p className="text-sm text-muted-foreground font-medium">
            <strong className="text-foreground">Aviso Legal:</strong> Este análisis es una herramienta de orientación basada en síntomas generales y no sustituye el diagnóstico profesional de un médico veterinario cualificado.
          </p>
        </div>

        <div className="mt-8 flex justify-center">
          <button 
            onClick={() => navigate('/')}
            className="px-8 py-4 bg-primary text-white font-bold rounded-2xl flex items-center gap-3 hover:opacity-90 transition-all shadow-lg shadow-primary/20 hover:-translate-y-1"
          >
            <Home size={20} />
            Finalizar y Volver al Inicio
          </button>
        </div>
      </div>
    </div>
  );
}
