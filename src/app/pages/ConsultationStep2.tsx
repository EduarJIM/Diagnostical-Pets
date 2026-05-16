import { motion } from 'motion/react';
import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router';
import { ChevronRight, ArrowLeft } from 'lucide-react';
import { getStorageData } from '../utils/storage';
import { PetData } from './Pets';

export function ConsultationStep2() {
  const navigate = useNavigate();
  const location = useLocation();
  const { selectedSymptoms = [] } = location.state || {};

  const [pets, setPets] = useState<PetData[]>([]);
  const [selectedPetId, setSelectedPetId] = useState<string>('');
  const [manualPetName, setManualPetName] = useState('');
  const [additionalInfo, setAdditionalInfo] = useState('');

  useEffect(() => {
    const savedPets = getStorageData<PetData[]>('petsData', []);
    setPets(savedPets);
    if (savedPets.length > 0) {
      setSelectedPetId(savedPets[0].id.toString());
    } else {
      setSelectedPetId('manual');
    }
  }, []);

  // Si alguien entra directamente a step2 sin síntomas, volver a paso 1
  if (selectedSymptoms.length === 0) {
    navigate('/consultation');
    return null;
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    let finalPetName = 'Mascota Desconocida';
    if (selectedPetId === 'manual') {
      finalPetName = manualPetName || 'Mascota No Registrada';
    } else {
      const pet = pets.find(p => p.id.toString() === selectedPetId);
      if (pet) finalPetName = pet.name;
    }

    navigate('/result', {
      state: { 
        petName: finalPetName, 
        selectedSymptoms, 
        additionalInfo 
      }
    });
  };

  return (
    <div className="flex-1 h-screen overflow-auto bg-background p-8 md:p-12 transition-colors duration-500 flex flex-col">
      <div className="max-w-3xl mx-auto w-full">
        <div className="flex items-center justify-between mb-8">
          <button 
            onClick={() => navigate('/consultation')}
            className="flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors group"
          >
            <ArrowLeft size={20} className="group-hover:-translate-x-1 transition-transform" />
            Volver a Síntomas
          </button>
          <div className="flex items-center gap-4">
            <div className="h-1.5 w-32 bg-muted rounded-full overflow-hidden border border-border">
              <motion.div 
                className="h-full bg-primary" 
                initial={{ width: '50%' }}
                animate={{ width: '100%' }}
              />
            </div>
            <span className="text-xs text-muted-foreground font-medium uppercase tracking-wider">Paso 2 de 2</span>
          </div>
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-card border border-border rounded-3xl p-8 md:p-10 shadow-lg"
        >
          <div className="mb-8">
            <h1 className="text-3xl text-foreground font-bold mb-2">Asignar Consulta</h1>
            <p className="text-muted-foreground text-lg">Selecciona a cuál de tus mascotas le pertenece este análisis de síntomas.</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="block text-foreground font-bold mb-3">Mascota Registrada</label>
              <select
                value={selectedPetId}
                onChange={(e) => setSelectedPetId(e.target.value)}
                className="w-full px-5 py-4 rounded-2xl bg-background border border-border text-foreground focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary transition-all shadow-sm appearance-none cursor-pointer"
              >
                {pets.map(pet => (
                  <option key={pet.id} value={pet.id.toString()}>{pet.name} ({pet.breed})</option>
                ))}
                <option value="manual">Otra Mascota (No Registrada)</option>
              </select>
            </div>

            {selectedPetId === 'manual' && (
              <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }}>
                <label className="block text-foreground font-bold mb-3">Nombre de la mascota</label>
                <input
                  type="text"
                  value={manualPetName}
                  onChange={(e) => setManualPetName(e.target.value)}
                  placeholder="Ej. Max, Luna..."
                  className="w-full px-5 py-4 rounded-2xl bg-background border border-border text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary transition-all shadow-sm"
                />
              </motion.div>
            )}

            <div>
              <label className="block text-foreground font-bold mb-3">Información adicional o contexto importante</label>
              <textarea
                value={additionalInfo}
                onChange={(e) => setAdditionalInfo(e.target.value)}
                placeholder="Ej. Empezó hace 2 días, comió algo raro..."
                rows={5}
                className="w-full px-5 py-4 rounded-2xl bg-background border border-border text-foreground placeholder:text-muted-foreground/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary transition-all resize-none shadow-sm"
              />
            </div>

            <motion.button
              whileHover={{ scale: 1.01 }}
              whileTap={{ scale: 0.99 }}
              type="submit"
              className="mt-10 w-full py-4 rounded-2xl bg-primary text-white font-bold flex items-center justify-center gap-3 transition-all shadow-xl shadow-primary/20 hover:opacity-90"
            >
              <span className="text-lg">Generar Diagnóstico y Guardar</span>
              <ChevronRight size={22} />
            </motion.button>
          </form>
        </motion.div>
      </div>
    </div>
  );
}
