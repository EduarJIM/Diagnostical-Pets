import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { PawPrint, Plus, Edit2, Trash2, X, Syringe, Activity, Stethoscope, ChevronRight, FileText, Clock, CheckCircle2 } from 'lucide-react';
import { getStorageData, setStorageData } from '../utils/storage';
import { HistoryItem } from './History';

export interface PetData {
  id: number;
  name: string;
  age: string;
  breed: string;
  sex: string;
  vaccines: string;
  history: string;
  clinical: string;
  vet: string;
}

const INITIAL_PETS: PetData[] = [
  {
    id: 1,
    name: 'Max',
    age: '3 años',
    breed: 'Golden Retriever',
    sex: 'Macho',
    vaccines: 'Rabia, Parvovirus, Moquillo (Al día)',
    history: 'Ninguna enfermedad grave previa.',
    clinical: 'Alergia leve al pollo.',
    vet: 'Dr. Ramírez (Clínica VetSalud)'
  },
  {
    id: 2,
    name: 'Luna',
    age: '1 año',
    breed: 'Gato Siamés',
    sex: 'Hembra',
    vaccines: 'Triple Felina (Al día)',
    history: 'Infección urinaria leve hace 6 meses.',
    clinical: 'Saludable.',
    vet: 'Dra. Silva (Centro Felino)'
  }
];

export function Pets() {
  const [pets, setPets] = useState<PetData[]>(() => getStorageData('petsData', INITIAL_PETS));
  const [historyData, setHistoryData] = useState<HistoryItem[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedPet, setSelectedPet] = useState<PetData | null>(null);

  useEffect(() => {
    setStorageData('petsData', pets);
  }, [pets]);

  useEffect(() => {
    setHistoryData(getStorageData<HistoryItem[]>('historyData', []));
    const handleStorageChange = () => setHistoryData(getStorageData<HistoryItem[]>('historyData', []));
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const handleOpenModal = (pet: PetData | null = null) => {
    setSelectedPet(pet);
    setIsModalOpen(true);
  };

  const handleDelete = (id: number) => {
    if (window.confirm("¿Estás seguro de eliminar esta mascota?")) {
      setPets(prev => prev.filter(p => p.id !== id));
    }
  };

  const handleSave = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const petData: PetData = {
      id: selectedPet ? selectedPet.id : Date.now(),
      name: formData.get('name') as string,
      breed: formData.get('breed') as string,
      age: formData.get('age') as string,
      sex: formData.get('sex') as string,
      vaccines: formData.get('vaccines') as string,
      history: formData.get('history') as string,
      clinical: formData.get('clinical') as string,
      vet: formData.get('vet') as string,
    };

    if (selectedPet) {
      setPets(prev => prev.map(p => p.id === petData.id ? petData : p));
    } else {
      setPets(prev => [...prev, petData]);
    }
    setIsModalOpen(false);
  };

  return (
    <div className="flex-1 h-screen overflow-auto bg-background p-8 md:p-12 transition-colors duration-500">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="max-w-6xl mx-auto"
      >
        <header className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-4xl font-bold text-foreground mb-3 flex items-center gap-3">
              <PawPrint className="text-primary" size={36} />
              Mis Mascotas
            </h1>
            <p className="text-muted-foreground text-lg">
              Gestiona el perfil de tus mascotas y su información clínica.
            </p>
          </div>
          <button 
            onClick={() => handleOpenModal()}
            className="flex items-center gap-2 px-6 py-3 bg-primary text-white font-bold rounded-xl hover:opacity-90 transition-all shadow-lg shadow-primary/20 hover:-translate-y-0.5"
          >
            <Plus size={20} />
            Agregar Mascota
          </button>
        </header>

        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {pets.map((pet) => (
            <motion.div
              key={pet.id}
              className="bg-card border border-border rounded-3xl p-6 shadow-sm hover:shadow-xl hover:shadow-primary/5 transition-all group flex flex-col"
            >
              <div className="flex justify-between items-start mb-4">
                <div className="flex items-center gap-4">
                  <div className="w-14 h-14 rounded-2xl bg-primary/10 flex items-center justify-center">
                    <PawPrint size={28} className="text-primary" />
                  </div>
                  <div>
                    <h3 className="text-2xl font-bold text-foreground">{pet.name}</h3>
                    <p className="text-muted-foreground font-medium">{pet.breed} • {pet.age}</p>
                  </div>
                </div>
                <div className="flex gap-2 relative z-10">
                  <button onClick={(e) => { e.stopPropagation(); handleOpenModal(pet); }} className="w-8 h-8 rounded-lg bg-muted flex items-center justify-center text-muted-foreground hover:text-primary transition-colors">
                    <Edit2 size={14} />
                  </button>
                  <button onClick={(e) => { e.stopPropagation(); handleDelete(pet.id); }} className="w-8 h-8 rounded-lg bg-muted flex items-center justify-center text-muted-foreground hover:text-destructive transition-colors">
                    <Trash2 size={14} />
                  </button>
                </div>
              </div>

              <div className="space-y-3 mt-6 flex-1">
                <div className="flex items-start gap-3 text-sm">
                  <Syringe size={16} className="text-primary mt-0.5 flex-shrink-0" />
                  <div>
                    <span className="font-semibold text-foreground">Vacunas: </span>
                    <span className="text-muted-foreground">{pet.vaccines}</span>
                  </div>
                </div>
                <div className="flex items-start gap-3 text-sm">
                  <Activity size={16} className="text-primary mt-0.5 flex-shrink-0" />
                  <div>
                    <span className="font-semibold text-foreground">Antecedentes: </span>
                    <span className="text-muted-foreground">{pet.clinical}</span>
                  </div>
                </div>
                <div className="flex items-start gap-3 text-sm">
                  <Stethoscope size={16} className="text-primary mt-0.5 flex-shrink-0" />
                  <div>
                    <span className="font-semibold text-foreground">Veterinario: </span>
                    <span className="text-muted-foreground">{pet.vet}</span>
                  </div>
                </div>
              </div>

              <div className="mt-6 pt-4 border-t border-border w-full">
                <button onClick={() => handleOpenModal(pet)} className="w-full py-2.5 bg-primary/10 text-primary rounded-xl text-sm font-bold hover:bg-primary hover:text-white transition-colors flex items-center justify-center gap-2 group-hover:bg-primary group-hover:text-white">
                  Ver perfil completo <ChevronRight size={16} className="group-hover:translate-x-1 transition-transform" />
                </button>
              </div>
            </motion.div>
          ))}
          {pets.length === 0 && (
            <div className="col-span-full py-12 text-center text-muted-foreground">
              <PawPrint size={48} className="mx-auto mb-4 opacity-20" />
              <p>Aún no has registrado ninguna mascota.</p>
            </div>
          )}
        </div>
      </motion.div>

      <AnimatePresence>
        {isModalOpen && (
          <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4">
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setIsModalOpen(false)}
              className="absolute inset-0 bg-black/40 backdrop-blur-md"
            />
            
            <motion.div
              initial={{ scale: 0.95, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              exit={{ scale: 0.95, opacity: 0 }}
              className="relative w-full max-w-2xl bg-card border border-border rounded-[2rem] p-8 shadow-2xl overflow-y-auto max-h-[90vh]"
            >
              <button 
                onClick={() => setIsModalOpen(false)}
                className="absolute top-6 right-6 w-10 h-10 rounded-full bg-muted flex items-center justify-center text-muted-foreground hover:text-foreground transition-all z-10"
              >
                <X size={20} />
              </button>

              <h2 className="text-2xl font-bold text-foreground mb-6 flex items-center gap-2">
                <PawPrint className="text-primary" />
                {selectedPet ? 'Editar Perfil de Mascota' : 'Registrar Nueva Mascota'}
              </h2>

              <form onSubmit={handleSave}>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-muted-foreground">Nombre</label>
                    <input name="name" required type="text" defaultValue={selectedPet?.name} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-muted-foreground">Especie / Raza</label>
                    <input name="breed" required type="text" defaultValue={selectedPet?.breed} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-muted-foreground">Edad</label>
                    <input name="age" type="text" defaultValue={selectedPet?.age} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-muted-foreground">Sexo</label>
                    <select name="sex" defaultValue={selectedPet?.sex || ""} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 appearance-none">
                      <option value="" disabled>Seleccione</option>
                      <option value="Macho">Macho</option>
                      <option value="Hembra">Hembra</option>
                    </select>
                  </div>
                  
                  <div className="space-y-1 md:col-span-2">
                    <label className="text-sm font-medium text-muted-foreground">Registro de Vacunas</label>
                    <input name="vaccines" type="text" defaultValue={selectedPet?.vaccines} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30" placeholder="Ej: Rabia, Parvovirus (Al día)" />
                  </div>
                  
                  <div className="space-y-1 md:col-span-2">
                    <label className="text-sm font-medium text-muted-foreground">Registro de Enfermedades Previas</label>
                    <textarea name="history" defaultValue={selectedPet?.history} rows={2} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 resize-none" placeholder="Describe cualquier enfermedad que haya tenido..."></textarea>
                  </div>
                  
                  <div className="space-y-1 md:col-span-2">
                    <label className="text-sm font-medium text-muted-foreground">Antecedentes Clínicos (Alergias, Condiciones)</label>
                    <textarea name="clinical" defaultValue={selectedPet?.clinical} rows={2} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 resize-none" placeholder="Ej: Alergia al pollo, asma..."></textarea>
                  </div>
                  
                  <div className="space-y-1 md:col-span-2">
                    <label className="text-sm font-medium text-muted-foreground">Veterinario Tratante / Clínica</label>
                    <input name="vet" type="text" defaultValue={selectedPet?.vet} className="w-full bg-background border border-border text-foreground px-4 py-2.5 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30" placeholder="Nombre del doctor o centro veterinario" />
                  </div>
                </div>

                <div className="mt-8 flex justify-end gap-3">
                  <button type="button" onClick={() => setIsModalOpen(false)} className="px-6 py-2.5 rounded-xl font-medium text-muted-foreground hover:bg-muted transition-colors">
                    Cancelar
                  </button>
                  <button type="submit" className="px-6 py-2.5 bg-primary text-white rounded-xl font-bold hover:opacity-90 shadow-lg shadow-primary/20 transition-all">
                    Guardar Perfil
                  </button>
                </div>
              </form>

              {/* Historial Automático */}
              {selectedPet && (
                <div className="mt-12 pt-8 border-t border-border">
                  <h3 className="text-xl font-bold text-foreground mb-4 flex items-center gap-2">
                    <HistoryIcon size={20} className="text-primary" />
                    Historial de Análisis (Automático)
                  </h3>
                  <div className="space-y-3">
                    {historyData.filter(h => h.pet === selectedPet.name).length > 0 ? (
                      historyData.filter(h => h.pet === selectedPet.name).map(item => (
                        <div key={item.id} className="p-4 rounded-xl bg-muted/30 border border-border flex items-start gap-4">
                          <div className="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                            <FileText size={20} className="text-primary" />
                          </div>
                          <div>
                            <div className="flex items-center gap-2 mb-1">
                              <span className="font-bold text-foreground">{item.date}</span>
                              <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold uppercase ${
                                item.result === 'CRÍTICO' ? 'bg-red-500/10 text-red-600' :
                                item.result === 'MODERADO' ? 'bg-amber-500/10 text-amber-600' :
                                'bg-emerald-500/10 text-emerald-600'
                              }`}>
                                {item.result}
                              </span>
                            </div>
                            <p className="text-sm text-muted-foreground whitespace-pre-wrap">{item.summary}</p>
                          </div>
                        </div>
                      ))
                    ) : (
                      <p className="text-muted-foreground text-sm italic">No hay consultas registradas para esta mascota aún.</p>
                    )}
                  </div>
                </div>
              )}
            </motion.div>
          </div>
        )}
      </AnimatePresence>
    </div>
  );
}

const HistoryIcon = ({ size, className }: { size: number, className?: string }) => (
  <svg xmlns="http://www.w3.org/2000/svg" width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}><path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8"/><path d="M3 3v5h5"/><path d="M12 7v5l4 2"/></svg>
);
