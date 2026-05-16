import { Sun, Moon } from 'lucide-react';
import { motion } from 'motion/react';

interface ThemeToggleProps {
  theme: 'light' | 'dark';
  toggleTheme: () => void;
}

export function ThemeToggle({ theme, toggleTheme }: ThemeToggleProps) {
  return (
    <button
      onClick={toggleTheme}
      className={`
        relative w-14 h-7 rounded-full transition-colors duration-300 flex items-center p-1
        ${theme === 'light' ? 'bg-amber-100' : 'bg-slate-800'}
        border border-border shadow-sm
      `}
    >
      <motion.div
        animate={{ 
          x: theme === 'dark' ? 28 : 0,
          rotate: theme === 'dark' ? 180 : 0 
        }}
        transition={{ type: "spring", stiffness: 400, damping: 25 }}
        className={`
          z-10 w-5 h-5 rounded-full flex items-center justify-center shadow-md
          ${theme === 'light' 
            ? 'bg-amber-400 text-amber-900 shadow-amber-200' 
            : 'bg-slate-200 text-slate-900 shadow-black/20'}
        `}
      >
        {theme === 'light' ? (
          <Sun size={12} strokeWidth={3} />
        ) : (
          <Moon size={12} strokeWidth={3} />
        )}
      </motion.div>
      
      <div className="absolute inset-0 flex justify-around items-center px-2 pointer-events-none opacity-40">
        <Sun size={10} className={theme === 'light' ? 'text-amber-500' : 'text-slate-500'} />
        <Moon size={10} className={theme === 'dark' ? 'text-slate-300' : 'text-slate-500'} />
      </div>
    </button>
  );
}
