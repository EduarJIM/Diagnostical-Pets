import { motion } from 'motion/react';
import { Home, Stethoscope, BookOpen, History, Settings, LogOut, PawPrint } from 'lucide-react';
import { useLocation, Link, useNavigate } from 'react-router';
import { ThemeToggle } from './ThemeToggle';

interface NavItemProps {
  to: string;
  icon: React.ReactNode;
  label: string;
  isActive: boolean;
}

function NavItem({ to, icon, label, isActive }: NavItemProps) {
  return (
    <Link
      to={to}
      className={`
        flex items-center gap-3 px-4 py-3 rounded-lg transition-all duration-200
        ${isActive
          ? 'bg-primary text-primary-foreground shadow-lg shadow-primary/20'
          : 'text-muted-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground'
        }
      `}
    >
      <span className="w-5 h-5">{icon}</span>
      <span>{label}</span>
    </Link>
  );
}

interface SidebarProps {
  theme: 'light' | 'dark';
  toggleTheme: () => void;
}

export function Sidebar({ theme, toggleTheme }: SidebarProps) {
  const location = useLocation();

  const navItems = [
    { to: '/', icon: <Home size={20} />, label: 'Dashboard' },
    { to: '/pets', icon: <PawPrint size={20} />, label: 'Mis Mascotas' },
    { to: '/consultation', icon: <Stethoscope size={20} />, label: 'Análisis de Síntomas' },
    { to: '/history', icon: <History size={20} />, label: 'Historial' },
  ];

  const navigate = useNavigate();

  return (
    <aside className="w-64 h-screen bg-sidebar border-r border-sidebar-border flex flex-col transition-colors duration-500 shadow-xl z-50">
      <div className="p-6">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-2">
            <motion.div 
              animate={{ 
                scale: [1, 1.1, 1],
                filter: ["drop-shadow(0 0 0px var(--color-primary))", "drop-shadow(0 0 8px var(--color-primary))", "drop-shadow(0 0 0px var(--color-primary))"]
              }}
              transition={{ 
                duration: 3, 
                repeat: Infinity,
                ease: "easeInOut" 
              }}
              className="w-9 h-9 rounded-xl bg-primary flex items-center justify-center shadow-lg shadow-primary/20"
            >
              <Stethoscope size={20} className="text-white" />
            </motion.div>
            <div className="leading-tight">
              <h1 className="text-sm font-bold text-foreground tracking-tight">Diagnostica</h1>
              <p className="text-[10px] text-muted-foreground font-medium">tu Mascota</p>
            </div>
          </div>
          <ThemeToggle theme={theme} toggleTheme={toggleTheme} />
        </div>
      </div>

      <nav className="flex-1 p-4 space-y-2">
        {navItems.map((item) => (
          <NavItem
            key={item.to}
            to={item.to}
            icon={item.icon}
            label={item.label}
            isActive={location.pathname === item.to}
          />
        ))}
      </nav>

      <div className="p-4 space-y-1 border-t border-sidebar-border">
        <button 
          onClick={() => navigate('/settings')}
          className="flex items-center gap-3 px-4 py-3 rounded-lg text-muted-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground transition-all duration-200 w-full group"
        >
          <Settings size={20} className="group-hover:rotate-45 transition-transform" />
          <span>Configuración</span>
        </button>
        <button 
          onClick={() => {
            import('../utils/storage').then(m => m.setCurrentUser(null));
            window.location.href = '/login';
          }}
          className="flex items-center gap-3 px-4 py-3 rounded-lg text-destructive/70 hover:bg-destructive/10 hover:text-destructive transition-all duration-200 w-full group"
        >
          <LogOut size={20} className="group-hover:-translate-x-1 transition-transform" />
          <span>Cerrar Sesión</span>
        </button>
      </div>
    </aside>
  );
}
