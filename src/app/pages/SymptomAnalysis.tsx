import { useState } from 'react';
import { motion } from 'motion/react';
import { 
  Activity, 
  Wind, 
  Utensils, 
  Brain, 
  ChevronRight, 
  ArrowLeft,
  Sparkles
} from 'lucide-react';
import { useNavigate } from 'react-router';

const SYMPTOMS_CATEGORIES = [
  {
    id: 'general',
    name: 'Síntomas Generales',
    icon: <Activity size={20} />,
    color: 'from-emerald-500/20 to-teal-500/20',
    symptoms: ['Fiebre', 'Letargo / Debilidad', 'Pérdida de peso', 'Aumento de sed', 'Temblores']
  },
  {
    id: 'respiratory',
    name: 'Respiratorios',
    icon: <Wind size={20} />,
    color: 'from-blue-500/20 to-cyan-500/20',
    symptoms: ['Tos persistente', 'Estornudos', 'Dificultad para respirar', 'Secreción nasal', 'Respiración ruidosa']
  },
  {
    id: 'digestive',
    name: 'Digestivos',
    icon: <Utensils size={20} />,
    color: 'from-orange-500/20 to-amber-500/20',
    symptoms: ['Vómitos', 'Diarrea', 'Pérdida de apetito', 'Dificultad al tragar', 'Abdomen hinchado']
  },
  {
    id: 'neurological',
    name: 'Neurológicos / Diagnósticos',
    icon: <Brain size={20} />,
    color: 'from-purple-500/20 to-indigo-500/20',
    symptoms: ['Convulsiones', 'Desorientación', 'Incoordinación al caminar', 'Parálisis', 'Cambio de conducta']
  }
];

export function SymptomAnalysis() {
  const navigate = useNavigate();
  const [selectedSymptoms, setSelectedSymptoms] = useState<string[]>([]);
  const [activeCategory, setActiveCategory] = useState(SYMPTOMS_CATEGORIES[0].id);

  const toggleSymptom = (symptom: string) => {
    setSelectedSymptoms(prev => 
      prev.includes(symptom) 
        ? prev.filter(s => s !== symptom) 
        : [...prev, symptom]
    );
  };

  const currentCategory = SYMPTOMS_CATEGORIES.find(c => c.id === activeCategory)!;

  return (
    <div className="p-8 max-w-5xl mx-auto h-full flex flex-col transition-colors duration-500">
      <div className="flex items-center justify-between mb-8">
        <button 
          onClick={() => navigate('/')}
          className="flex items-center gap-2 text-muted-foreground hover:text-foreground transition-colors group"
        >
          <ArrowLeft size={20} className="group-hover:-translate-x-1 transition-transform" />
          Volver al Dashboard
        </button>
        <div className="flex items-center gap-4">
          <div className="h-1.5 w-32 bg-muted rounded-full overflow-hidden border border-border">
            <motion.div 
              className="h-full bg-primary" 
              initial={{ width: 0 }}
              animate={{ width: '50%' }}
            />
          </div>
          <span className="text-xs text-muted-foreground font-medium uppercase tracking-wider">Paso 1 de 2</span>
        </div>
      </div>

      <header className="mb-10">
        <h1 className="text-4xl font-bold text-foreground mb-3">Análisis de Síntomas</h1>
        <p className="text-muted-foreground text-lg">
          Selecciona todos los síntomas que has observado en tu mascota para un análisis preliminar.
        </p>
      </header>

      <div className="grid grid-cols-1 lg:grid-cols-[280px_1fr] gap-8 flex-1 min-h-0">
        {/* Categories Sidebar */}
        <aside className="space-y-2">
          {SYMPTOMS_CATEGORIES.map((cat) => (
            <button
              key={cat.id}
              onClick={() => setActiveCategory(cat.id)}
              className={`w-full flex items-center gap-3 px-4 py-4 rounded-2xl border transition-all duration-300 ${
                activeCategory === cat.id
                  ? 'bg-primary/10 border-primary/30 text-foreground shadow-lg shadow-primary/5'
                  : 'bg-transparent border-transparent text-muted-foreground hover:bg-muted hover:text-foreground'
              }`}
            >
              <div className={`w-8 h-8 rounded-lg flex items-center justify-center bg-gradient-to-br ${cat.color} ${activeCategory === cat.id ? 'text-primary' : 'text-muted-foreground'}`}>
                {cat.icon}
              </div>
              <span className="font-semibold text-sm">{cat.name}</span>
            </button>
          ))}
        </aside>

        {/* Symptoms Grid */}
        <main className="bg-card border border-border rounded-3xl p-8 relative overflow-hidden shadow-sm">
          <div className="absolute top-0 right-0 p-8 opacity-5 text-primary">
            {currentCategory.icon}
          </div>

          <h3 className="text-xl font-bold text-foreground mb-6 flex items-center gap-2">
            <Sparkles size={18} className="text-primary" />
            {currentCategory.name}
          </h3>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {currentCategory.symptoms.map((symptom) => (
              <button
                key={symptom}
                onClick={() => toggleSymptom(symptom)}
                className={`flex items-center justify-between px-5 py-4 rounded-2xl border transition-all duration-200 group ${
                  selectedSymptoms.includes(symptom)
                    ? 'bg-primary/10 border-primary/50 text-primary'
                    : 'bg-background border-border text-muted-foreground hover:border-primary/30 hover:bg-muted'
                }`}
              >
                <span className="font-medium">{symptom}</span>
                <div className={`w-5 h-5 rounded-full border-2 transition-all flex items-center justify-center ${
                  selectedSymptoms.includes(symptom)
                    ? 'bg-primary border-primary'
                    : 'border-border'
                }`}>
                  {selectedSymptoms.includes(symptom) && (
                    <motion.div
                      initial={{ scale: 0 }}
                      animate={{ scale: 1 }}
                      className="w-2 h-2 bg-white rounded-full"
                    />
                  )}
                </div>
              </button>
            ))}
          </div>
        </main>
      </div>

      <footer className="mt-10 py-6 border-t border-border flex items-center justify-between">
        <div className="text-sm">
          <span className="text-muted-foreground">Seleccionados: </span>
          <span className="text-primary font-bold">{selectedSymptoms.length} síntomas</span>
        </div>
        
        <button
          onClick={() => navigate('/consultation/step2', { state: { selectedSymptoms } })}
          disabled={selectedSymptoms.length === 0}
          className={`flex items-center gap-2 px-8 py-3 rounded-xl font-bold transition-all ${
            selectedSymptoms.length > 0
              ? 'bg-primary text-primary-foreground shadow-lg shadow-primary/20 hover:opacity-90 active:scale-[0.98]'
              : 'bg-muted text-muted-foreground cursor-not-allowed border border-border'
          }`}
        >
          Continuar Análisis
          <ChevronRight size={20} />
        </button>
      </footer>
    </div>
  );
}
