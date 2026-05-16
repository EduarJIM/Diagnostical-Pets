import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { Clock, FileText, X, AlertCircle, CheckCircle2, ChevronRight, History as HistoryIcon, CalendarClock, BellRing, Plus, Trash2 } from 'lucide-react';
import { getStorageData, setStorageData } from '../utils/storage';

export interface HistoryItem {
  id: number;
  date: string;
  pet: string;
  status: string;
  result: string;
  summary: string;
  recommendation: string;
}

export interface UpcomingItem {
  id: number;
  date: string;
  pet: string;
  type: string;
  summary: string;
  action: string;
}

export interface AlertItem {
  id: number;
  severity: string;
  title: string;
  pet: string;
  message: string;
  date: string;
  time: string;
}

const INITIAL_HISTORY: HistoryItem[] = [
  { 
    id: 1, 
    date: '14 de Mayo, 2024', 
    pet: 'Max', 
    status: 'Completado', 
    result: 'Caso Leve',
    summary: 'Se observó letargo leve y falta de apetito. No hay signos de distress respiratorio.',
    recommendation: 'Observación por 24 horas. Mantener hidratación. Si los síntomas persisten, acudir a consulta física.'
  }
];

const INITIAL_UPCOMING: UpcomingItem[] = [
  {
    id: 1,
    date: '20 de Mayo, 2024',
    pet: 'Max',
    type: 'Seguimiento',
    summary: 'Revisión de evolución tras tratamiento por alergia leve.',
    action: 'Cita Programada'
  }
];

const INITIAL_ALERTS: AlertItem[] = [
  {
    id: 1,
    severity: 'Alta',
    title: 'Dosis de Desparasitante',
    pet: 'Max',
    message: 'Hoy corresponde la dosis mensual de desparasitante oral.',
    date: new Date().toISOString().split('T')[0],
    time: '12:00'
  }
];

export function History() {
  const [historyData, setHistoryData] = useState<HistoryItem[]>(() => getStorageData('historyData', INITIAL_HISTORY));
  const [upcomingData, setUpcomingData] = useState<UpcomingItem[]>(() => getStorageData('upcomingData', INITIAL_UPCOMING));
  const [alertsData, setAlertsData] = useState<AlertItem[]>(() => getStorageData('alertsData', INITIAL_ALERTS));

  const [activeTab, setActiveTab] = useState<'history' | 'upcoming' | 'alerts'>('history');
  
  const [selectedHistoryItem, setSelectedHistoryItem] = useState<HistoryItem | null>(null);
  
  // Modals state
  const [isHistoryModalOpen, setIsHistoryModalOpen] = useState(false);
  const [isUpcomingModalOpen, setIsUpcomingModalOpen] = useState(false);
  const [isAlertModalOpen, setIsAlertModalOpen] = useState(false);

  // Sync to local storage
  useEffect(() => { setStorageData('historyData', historyData); }, [historyData]);
  useEffect(() => { setStorageData('upcomingData', upcomingData); }, [upcomingData]);
  useEffect(() => { setStorageData('alertsData', alertsData); }, [alertsData]);

  // Handlers for deleting
  const deleteHistory = (id: number) => {
    if (confirm('¿Eliminar consulta?')) setHistoryData(prev => prev.filter(i => i.id !== id));
  };
  const deleteUpcoming = (id: number) => {
    if (confirm('¿Eliminar seguimiento?')) setUpcomingData(prev => prev.filter(i => i.id !== id));
  };
  const deleteAlert = (id: number) => {
    if (confirm('¿Descartar alerta?')) setAlertsData(prev => prev.filter(i => i.id !== id));
  };

  // Submit Handlers
  const handleAddHistory = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const newItem: HistoryItem = {
      id: Date.now(),
      date: formData.get('date') as string,
      pet: formData.get('pet') as string,
      status: formData.get('status') as string,
      result: formData.get('result') as string,
      summary: formData.get('summary') as string,
      recommendation: formData.get('recommendation') as string,
    };
    setHistoryData([newItem, ...historyData]);
    setIsHistoryModalOpen(false);
  };

  const handleAddUpcoming = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const newItem: UpcomingItem = {
      id: Date.now(),
      date: formData.get('date') as string,
      pet: formData.get('pet') as string,
      type: formData.get('type') as string,
      summary: formData.get('summary') as string,
      action: formData.get('action') as string,
    };
    setUpcomingData([newItem, ...upcomingData]);
    setIsUpcomingModalOpen(false);
  };

  const handleAddAlert = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const newItem: AlertItem = {
      id: Date.now(),
      severity: formData.get('severity') as string,
      title: formData.get('title') as string,
      pet: formData.get('pet') as string,
      message: formData.get('message') as string,
      date: formData.get('date') as string,
      time: formData.get('time') as string,
    };
    setAlertsData([newItem, ...alertsData]);
    setIsAlertModalOpen(false);
  };

  return (
    <div className="flex-1 h-screen overflow-auto bg-background p-8 md:p-12 transition-colors duration-500">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="max-w-5xl mx-auto"
      >
        <header className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-4xl font-bold text-foreground mb-3 flex items-center gap-3">
              <HistoryIcon className="text-primary" size={36} />
              Historial y Seguimiento
            </h1>
            <p className="text-muted-foreground text-lg"> Revisa y guarda diagnósticos, próximos procesos y alertas de salud. </p>
          </div>
          
          {/* Action buttons based on active tab */}
          {activeTab === 'history' && (
            <button onClick={() => setIsHistoryModalOpen(true)} className="flex items-center gap-2 px-5 py-2.5 bg-primary text-white font-bold rounded-xl hover:opacity-90 transition-all shadow-md w-fit">
              <Plus size={18} /> Nueva Consulta
            </button>
          )}
          {activeTab === 'upcoming' && (
            <button onClick={() => setIsUpcomingModalOpen(true)} className="flex items-center gap-2 px-5 py-2.5 bg-blue-500 text-white font-bold rounded-xl hover:opacity-90 transition-all shadow-md w-fit">
              <Plus size={18} /> Nuevo Seguimiento
            </button>
          )}
          {activeTab === 'alerts' && (
            <button onClick={() => setIsAlertModalOpen(true)} className="flex items-center gap-2 px-5 py-2.5 bg-amber-500 text-white font-bold rounded-xl hover:opacity-90 transition-all shadow-md w-fit">
              <Plus size={18} /> Nueva Alerta
            </button>
          )}
        </header>

        <div className="flex gap-4 mb-8 overflow-x-auto pb-2">
          <button 
            onClick={() => setActiveTab('history')}
            className={`flex items-center gap-2 px-6 py-3 rounded-full font-bold transition-all whitespace-nowrap ${
              activeTab === 'history' ? 'bg-primary text-white shadow-lg shadow-primary/20' : 'bg-card border border-border text-muted-foreground hover:bg-muted'
            }`}
          >
            <HistoryIcon size={18} /> Consultas Anteriores
          </button>
          <button 
            onClick={() => setActiveTab('upcoming')}
            className={`flex items-center gap-2 px-6 py-3 rounded-full font-bold transition-all whitespace-nowrap ${
              activeTab === 'upcoming' ? 'bg-blue-500 text-white shadow-lg shadow-blue-500/20' : 'bg-card border border-border text-muted-foreground hover:bg-muted'
            }`}
          >
            <CalendarClock size={18} /> Próximos Procesos
          </button>
          <button 
            onClick={() => setActiveTab('alerts')}
            className={`flex items-center gap-2 px-6 py-3 rounded-full font-bold transition-all whitespace-nowrap ${
              activeTab === 'alerts' ? 'bg-amber-500 text-white shadow-lg shadow-amber-500/20' : 'bg-card border border-border text-muted-foreground hover:bg-muted'
            }`}
          >
            <BellRing size={18} /> Alertas Activas
            <span className="w-5 h-5 rounded-full bg-white/20 text-xs flex items-center justify-center ml-1">{alertsData.length}</span>
          </button>
        </div>

        {/* --- LISTA DE CONSULTAS --- */}
        {activeTab === 'history' && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="grid gap-4">
            {historyData.map((item) => (
              <motion.div
                layoutId={`history-${item.id}`}
                key={item.id}
                className="p-6 rounded-2xl bg-card border border-border hover:border-primary/50 transition-all cursor-pointer shadow-sm group relative overflow-hidden flex items-center justify-between"
                onClick={() => setSelectedHistoryItem(item)}
              >
                <div className="flex items-center gap-5 relative z-10">
                  <div className="w-14 h-14 rounded-2xl bg-primary/10 flex items-center justify-center flex-shrink-0 group-hover:scale-110 transition-transform">
                    <FileText size={28} className="text-primary" />
                  </div>
                  <div>
                    <div className="flex items-center gap-3 mb-1">
                      <h3 className="text-xl font-bold text-foreground">{item.pet}</h3>
                      <span className={`px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider ${
                        item.result.includes('Leve') ? 'bg-emerald-500/10 text-emerald-600' : 
                        item.result.includes('Moderado') ? 'bg-amber-500/10 text-amber-600' : 
                        'bg-blue-500/10 text-blue-600'
                      }`}>
                        {item.result}
                      </span>
                    </div>
                    <div className="flex items-center gap-4 text-sm text-muted-foreground">
                      <div className="flex items-center gap-1.5"><Clock size={14} /><span>{item.date}</span></div>
                      <div className="flex items-center gap-1.5 text-emerald-600 font-medium"><CheckCircle2 size={14} /><span>{item.status}</span></div>
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-3 relative z-10">
                  <button onClick={(e) => { e.stopPropagation(); deleteHistory(item.id); }} className="p-2 text-muted-foreground hover:text-red-500 hover:bg-red-500/10 rounded-lg transition-colors">
                    <Trash2 size={18} />
                  </button>
                  <ChevronRight className="text-muted-foreground group-hover:text-primary transition-all" />
                </div>
              </motion.div>
            ))}
            {historyData.length === 0 && <p className="text-muted-foreground text-center py-8">No hay consultas registradas.</p>}
          </motion.div>
        )}

        {/* --- LISTA DE SEGUIMIENTO --- */}
        {activeTab === 'upcoming' && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="grid gap-4">
            {upcomingData.map((item) => (
              <div key={item.id} className="p-6 rounded-2xl bg-card border border-border flex flex-col md:flex-row justify-between items-start md:items-center gap-6 hover:border-blue-500/50 transition-all shadow-sm group">
                <div className="flex items-start gap-4">
                  <div className="w-12 h-12 rounded-full bg-blue-500/10 flex items-center justify-center flex-shrink-0 mt-1">
                    <CalendarClock size={24} className="text-blue-500" />
                  </div>
                  <div>
                    <div className="flex items-center gap-3 mb-1">
                      <h3 className="text-lg font-bold text-foreground">{item.pet} <span className="text-muted-foreground font-normal">| {item.type}</span></h3>
                    </div>
                    <p className="text-muted-foreground mb-2">{item.summary}</p>
                    <div className="flex items-center gap-2 text-sm font-semibold text-blue-600 dark:text-blue-400">
                      <Clock size={16} />{item.date}
                    </div>
                  </div>
                </div>
                <div className="flex items-center gap-3 w-full md:w-auto">
                  <button onClick={() => deleteUpcoming(item.id)} className="p-2 text-muted-foreground hover:text-red-500 hover:bg-red-500/10 rounded-lg transition-colors">
                    <Trash2 size={18} />
                  </button>
                  <button className="px-5 py-2 rounded-xl bg-blue-500/10 text-blue-600 dark:text-blue-400 font-bold hover:bg-blue-500 hover:text-white transition-colors flex-1 md:flex-none">
                    {item.action}
                  </button>
                </div>
              </div>
            ))}
            {upcomingData.length === 0 && <p className="text-muted-foreground text-center py-8">No hay seguimientos programados.</p>}
          </motion.div>
        )}

        {/* --- LISTA DE ALERTAS --- */}
        {activeTab === 'alerts' && (
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="grid gap-4">
            {alertsData.map((alert) => (
              <div key={alert.id} className={`p-6 rounded-2xl border flex items-start gap-5 shadow-sm transition-all ${
                alert.severity === 'Alta' ? 'bg-red-500/5 border-red-500/20 hover:border-red-500/50' : 'bg-amber-500/5 border-amber-500/20 hover:border-amber-500/50'
              }`}>
                <div className={`w-12 h-12 rounded-full flex items-center justify-center flex-shrink-0 ${
                  alert.severity === 'Alta' ? 'bg-red-500/20 text-red-500' : 'bg-amber-500/20 text-amber-500'
                }`}>
                  <AlertCircle size={24} />
                </div>
                <div className="flex-1">
                  <div className="flex items-center gap-3 mb-1">
                    <h3 className="text-lg font-bold text-foreground">{alert.title}</h3>
                    <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold uppercase ${
                      alert.severity === 'Alta' ? 'bg-red-500 text-white' : 'bg-amber-500 text-white'
                    }`}>
                      Prioridad {alert.severity}
                    </span>
                  </div>
                  <p className="text-foreground font-medium mb-1">Mascota: {alert.pet}</p>
                  <div className="flex items-center gap-2 text-sm font-semibold text-amber-600 dark:text-amber-500 mb-1">
                    <Clock size={16} />
                    {alert.date} a las {alert.time}
                  </div>
                  <p className="text-muted-foreground text-sm">{alert.message}</p>
                </div>
                <button onClick={() => deleteAlert(alert.id)} className="text-muted-foreground hover:text-foreground">
                  <X size={20} />
                </button>
              </div>
            ))}
            {alertsData.length === 0 && <p className="text-muted-foreground text-center py-8">No hay alertas activas.</p>}
          </motion.div>
        )}
      </motion.div>

      {/* --- DETALLES DE CONSULTA (MODAL) --- */}
      <AnimatePresence>
        {selectedHistoryItem && (
          <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4">
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setSelectedHistoryItem(null)} className="absolute inset-0 bg-black/40 backdrop-blur-md" />
            <motion.div layoutId={`history-${selectedHistoryItem.id}`} className="relative w-full max-w-xl bg-card border border-border rounded-[2.5rem] p-8 md:p-10 shadow-2xl overflow-y-auto max-h-[90vh]">
              <button onClick={() => setSelectedHistoryItem(null)} className="absolute top-6 right-6 w-10 h-10 rounded-full bg-muted flex items-center justify-center text-muted-foreground hover:text-foreground transition-all z-10">
                <X size={20} />
              </button>
              <div className="space-y-8">
                <div className="flex items-center gap-4">
                  <div className="w-16 h-16 rounded-3xl bg-primary/10 flex items-center justify-center">
                    <FileText size={32} className="text-primary" />
                  </div>
                  <div>
                    <h2 className="text-3xl font-bold text-foreground">{selectedHistoryItem.pet}</h2>
                    <p className="text-muted-foreground">{selectedHistoryItem.date}</p>
                  </div>
                </div>
                <div className="grid gap-6">
                  <div className="bg-muted/50 p-6 rounded-3xl border border-border">
                    <h4 className="text-primary font-bold flex items-center gap-2 mb-3"><AlertCircle size={20} /> Resumen del Diagnóstico</h4>
                    <p className="text-foreground leading-relaxed">{selectedHistoryItem.summary}</p>
                  </div>
                  <div className="bg-primary/5 p-6 rounded-3xl border border-primary/20">
                    <h4 className="text-primary font-bold flex items-center gap-2 mb-3"><CheckCircle2 size={20} /> Recomendaciones</h4>
                    <p className="text-foreground leading-relaxed">{selectedHistoryItem.recommendation}</p>
                  </div>
                </div>
                <button onClick={() => setSelectedHistoryItem(null)} className="w-full py-4 bg-primary text-white rounded-2xl font-bold shadow-lg shadow-primary/20 hover:opacity-90 transition-all">
                  Entendido
                </button>
              </div>
            </motion.div>
          </div>
        )}
      </AnimatePresence>

      {/* --- FORMULARIOS --- */}
      <AnimatePresence>
        {isHistoryModalOpen && (
          <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4">
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsHistoryModalOpen(false)} className="absolute inset-0 bg-black/40 backdrop-blur-md" />
            <motion.div initial={{ scale: 0.95 }} animate={{ scale: 1 }} exit={{ scale: 0.95 }} className="relative w-full max-w-xl bg-card border border-border rounded-3xl p-8 shadow-2xl z-10">
              <h2 className="text-2xl font-bold mb-6">Guardar Consulta Anterior</h2>
              <form onSubmit={handleAddHistory} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Nombre Mascota</label>
                    <input name="pet" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Fecha</label>
                    <input name="date" required type="date" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Resultado</label>
                    <input name="result" placeholder="Ej: Caso Leve" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Estado</label>
                    <input name="status" defaultValue="Completado" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                </div>
                <div>
                  <label className="block text-sm text-muted-foreground mb-1">Resumen / Síntomas</label>
                  <textarea name="summary" required rows={2} className="w-full bg-background border border-border p-2.5 rounded-xl resize-none"></textarea>
                </div>
                <div>
                  <label className="block text-sm text-muted-foreground mb-1">Recomendaciones</label>
                  <textarea name="recommendation" required rows={2} className="w-full bg-background border border-border p-2.5 rounded-xl resize-none"></textarea>
                </div>
                <div className="flex justify-end gap-3 mt-6">
                  <button type="button" onClick={() => setIsHistoryModalOpen(false)} className="px-5 py-2 text-muted-foreground">Cancelar</button>
                  <button type="submit" className="px-5 py-2 bg-primary text-white font-bold rounded-xl">Guardar Consulta</button>
                </div>
              </form>
            </motion.div>
          </div>
        )}

        {isUpcomingModalOpen && (
          <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4">
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsUpcomingModalOpen(false)} className="absolute inset-0 bg-black/40 backdrop-blur-md" />
            <motion.div initial={{ scale: 0.95 }} animate={{ scale: 1 }} exit={{ scale: 0.95 }} className="relative w-full max-w-xl bg-card border border-border rounded-3xl p-8 shadow-2xl z-10">
              <h2 className="text-2xl font-bold mb-6 text-blue-500">Programar Seguimiento</h2>
              <form onSubmit={handleAddUpcoming} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Mascota</label>
                    <input name="pet" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Fecha</label>
                    <input name="date" required type="date" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Tipo</label>
                    <input name="type" placeholder="Ej: Vacunación, Revisión" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Acción</label>
                    <input name="action" defaultValue="Cita Programada" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                </div>
                <div>
                  <label className="block text-sm text-muted-foreground mb-1">Detalles</label>
                  <textarea name="summary" required rows={2} className="w-full bg-background border border-border p-2.5 rounded-xl resize-none"></textarea>
                </div>
                <div className="flex justify-end gap-3 mt-6">
                  <button type="button" onClick={() => setIsUpcomingModalOpen(false)} className="px-5 py-2 text-muted-foreground">Cancelar</button>
                  <button type="submit" className="px-5 py-2 bg-blue-500 text-white font-bold rounded-xl">Programar</button>
                </div>
              </form>
            </motion.div>
          </div>
        )}

        {isAlertModalOpen && (
          <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4">
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsAlertModalOpen(false)} className="absolute inset-0 bg-black/40 backdrop-blur-md" />
            <motion.div initial={{ scale: 0.95 }} animate={{ scale: 1 }} exit={{ scale: 0.95 }} className="relative w-full max-w-xl bg-card border border-border rounded-3xl p-8 shadow-2xl z-10">
              <h2 className="text-2xl font-bold mb-6 text-amber-500">Crear Alerta</h2>
              <form onSubmit={handleAddAlert} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Mascota</label>
                    <input name="pet" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Prioridad</label>
                    <select name="severity" defaultValue="Media" className="w-full bg-background border border-border p-2.5 rounded-xl appearance-none">
                      <option value="Media">Media</option>
                      <option value="Alta">Alta</option>
                    </select>
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Fecha de la Alarma</label>
                    <input name="date" required type="date" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Hora de la Alarma</label>
                    <input name="time" required type="time" className="w-full bg-background border border-border p-2.5 rounded-xl" />
                  </div>
                </div>
                <div>
                  <label className="block text-sm text-muted-foreground mb-1">Título de Alerta</label>
                  <input name="title" required className="w-full bg-background border border-border p-2.5 rounded-xl" />
                </div>
                <div>
                  <label className="block text-sm text-muted-foreground mb-1">Mensaje / Motivo</label>
                  <textarea name="message" required rows={2} className="w-full bg-background border border-border p-2.5 rounded-xl resize-none"></textarea>
                </div>
                <div className="flex justify-end gap-3 mt-6">
                  <button type="button" onClick={() => setIsAlertModalOpen(false)} className="px-5 py-2 text-muted-foreground">Cancelar</button>
                  <button type="submit" className="px-5 py-2 bg-amber-500 text-white font-bold rounded-xl">Crear Alerta</button>
                </div>
              </form>
            </motion.div>
          </div>
        )}
      </AnimatePresence>
    </div>
  );
}
