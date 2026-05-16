import { motion, AnimatePresence } from 'motion/react';
import { AlertCircle, CheckCircle2, Clock } from 'lucide-react';
import { AlertItem } from '../pages/History';

interface ActiveAlarmModalProps {
  alarm: AlertItem | null;
  onDismiss: () => void;
}

export function ActiveAlarmModal({ alarm, onDismiss }: ActiveAlarmModalProps) {
  return (
    <AnimatePresence>
      {alarm && (
        <div className="fixed inset-0 flex items-center justify-center z-[10000] p-4">
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="absolute inset-0 bg-black/60 backdrop-blur-sm"
          />
          
          <motion.div
            initial={{ scale: 0.9, y: 20, opacity: 0 }}
            animate={{ scale: 1, y: 0, opacity: 1 }}
            exit={{ scale: 0.9, y: 20, opacity: 0 }}
            className="relative w-full max-w-lg bg-card border-2 border-primary/50 rounded-[2.5rem] p-8 md:p-12 shadow-2xl overflow-hidden flex flex-col items-center text-center"
          >
            {/* Pulsing background effect */}
            <div className="absolute inset-0 bg-primary/5 animate-pulse" />
            
            <div className="relative z-10 w-full flex flex-col items-center">
              <motion.div 
                animate={{ rotate: [0, -10, 10, -10, 10, 0] }}
                transition={{ duration: 1, repeat: Infinity, repeatDelay: 1 }}
                className="w-24 h-24 rounded-full bg-primary/10 flex items-center justify-center text-primary mb-6 shadow-[0_0_30px_rgba(var(--color-primary),0.3)]"
              >
                <AlertCircle size={48} />
              </motion.div>

              <div className="bg-primary/20 text-primary px-4 py-1.5 rounded-full text-sm font-bold tracking-widest uppercase mb-4 flex items-center gap-2">
                <Clock size={16} /> ¡Es la hora!
              </div>

              <h2 className="text-4xl font-black text-foreground mb-2 leading-tight">
                {alarm.title}
              </h2>
              
              <h3 className="text-2xl font-bold text-primary mb-6">
                Para: {alarm.pet}
              </h3>

              <div className="bg-muted/50 w-full p-6 rounded-3xl border border-border mb-8">
                <p className="text-lg text-foreground font-medium leading-relaxed">
                  {alarm.message}
                </p>
              </div>

              <button
                onClick={onDismiss}
                className="w-full group relative py-5 bg-primary text-white rounded-2xl font-bold text-lg shadow-xl shadow-primary/30 hover:shadow-primary/50 transition-all hover:-translate-y-1 flex items-center justify-center gap-3 overflow-hidden"
              >
                <span className="relative z-10 flex items-center gap-2">
                  <CheckCircle2 size={24} />
                  Marcar como Hecho
                </span>
                <div className="absolute inset-0 bg-white/20 translate-y-full group-hover:translate-y-0 transition-transform duration-300 ease-out" />
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </AnimatePresence>
  );
}
