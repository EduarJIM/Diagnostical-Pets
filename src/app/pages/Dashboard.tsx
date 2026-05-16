import { motion } from 'motion/react';
import { Stethoscope, BookOpen, History, Sparkles, PawPrint } from 'lucide-react';
import { useNavigate } from 'react-router';
import { FeatureCard } from '../components/FeatureCard';
import { DisclaimerBanner } from '../components/DisclaimerBanner';

export function Dashboard() {
  const navigate = useNavigate();

  return (
    <div className="flex-1 h-screen overflow-auto bg-background transition-colors duration-500">
      <div className="grid grid-cols-1 lg:grid-cols-2 h-full">
        <div className="p-12 flex flex-col gap-8">
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.5 }}
          >
            <div className="flex items-center gap-3 mb-2">
              <Sparkles size={24} className="text-primary" />
              <h1 className="text-4xl text-foreground font-bold">Bienvenido</h1>
            </div>
            <p className="text-lg text-muted-foreground">
              Sistema de Orientación de Salud para Mascotas
            </p>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.1 }}
            className="grid gap-4"
          >
            <FeatureCard
              title="Análisis de Síntomas"
              description="Evalúa los síntomas de tu mascota y recibe orientación inmediata"
              icon={Stethoscope}
              gradient="from-primary/10 to-secondary/10"
              onClick={() => navigate('/consultation')}
            />

            <div className="grid grid-cols-2 gap-4">
              <FeatureCard
                title="Mis Mascotas"
                description="Gestiona los perfiles de tus mascotas"
                icon={PawPrint}
                gradient="from-secondary/10 to-blue-500/10"
                onClick={() => navigate('/pets')}
              />
              <FeatureCard
                title="Historial"
                description="Revisa consultas anteriores"
                icon={History}
                gradient="from-purple-500/10 to-indigo-500/10"
                onClick={() => navigate('/history')}
              />
            </div>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.2 }}
          >
            <DisclaimerBanner />
          </motion.div>
        </div>

        <div className="relative flex items-center justify-center border-l border-border transition-colors duration-500 overflow-hidden bg-gradient-to-br from-transparent to-primary/5">
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_50%_50%,var(--color-primary),transparent_70%)] opacity-[0.05]" />

          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.8, delay: 0.3 }}
            className="relative w-80 h-80 z-10 flex items-center justify-center"
          >
            {/* Background Animated Rings */}
            <div className="absolute inset-0 rounded-full border-2 border-primary/20 animate-[ping_3s_cubic-bezier(0,0,0.2,1)_infinite] opacity-20" />
            <div className="absolute inset-8 rounded-full border border-secondary/30 animate-[pulse_4s_ease-in-out_infinite]" />
            
            {/* Floating Hero Image */}
            <motion.div
              animate={{ 
                y: [0, -20, 0],
                rotate: [0, 2, -2, 0]
              }}
              transition={{ 
                duration: 6, 
                repeat: Infinity, 
                ease: "easeInOut" 
              }}
              className="relative w-64 h-64 rounded-full bg-gradient-to-br from-primary via-secondary to-blue-500 flex items-center justify-center shadow-[0_20px_50px_rgba(16,185,129,0.3)] border-4 border-white/10"
            >
              <div className="w-56 h-56 rounded-full bg-background/10 backdrop-blur-3xl flex items-center justify-center">
                <Stethoscope size={90} className="text-white drop-shadow-lg" />
              </div>

              {/* Orbiting Elements */}
              <motion.div 
                animate={{ rotate: 360 }}
                transition={{ duration: 20, repeat: Infinity, ease: "linear" }}
                className="absolute inset-0"
              >
                <div className="absolute -top-4 left-1/2 -translate-x-1/2 w-10 h-10 rounded-xl bg-card border border-border shadow-lg flex items-center justify-center rotate-[-45deg]">
                  <Sparkles size={18} className="text-primary" />
                </div>
              </motion.div>
            </motion.div>
          </motion.div>

          <div className="absolute top-20 left-20 w-32 h-32 bg-primary/10 rounded-full blur-3xl animate-pulse" />
          <div className="absolute bottom-20 right-20 w-40 h-40 bg-secondary/10 rounded-full blur-3xl animate-pulse delay-1000" />
        </div>
      </div>
    </div>
  );
}
