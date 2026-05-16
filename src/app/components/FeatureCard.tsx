import { motion } from 'motion/react';
import { LucideIcon } from 'lucide-react';

interface FeatureCardProps {
  title: string;
  description: string;
  icon: LucideIcon;
  gradient: string;
  onClick?: () => void;
}

export function FeatureCard({ title, description, icon: Icon, gradient, onClick }: FeatureCardProps) {
  return (
    <motion.button
      onClick={onClick}
      whileHover={{ scale: 1.02, y: -4 }}
      whileTap={{ scale: 0.98 }}
      className={`
        relative p-6 rounded-3xl text-left w-full overflow-hidden
        bg-gradient-to-br ${gradient}
        border border-border/50
        transition-all duration-300
        shadow-sm hover:shadow-xl hover:shadow-primary/5
        group
      `}
    >
      <div className="relative z-10">
        <div className="w-12 h-12 rounded-xl bg-card border border-border flex items-center justify-center mb-4 group-hover:scale-110 transition-transform shadow-sm">
          <Icon size={24} className="text-primary" />
        </div>
        <h3 className="text-xl font-bold text-foreground mb-2 group-hover:text-primary transition-colors">{title}</h3>
        <p className="text-sm text-muted-foreground leading-relaxed">{description}</p>
      </div>

      <div className="absolute inset-0 bg-gradient-to-br from-white/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
    </motion.button>
  );
}
