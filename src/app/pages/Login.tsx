import { motion } from 'motion/react';
import { Mail, Lock, LogIn, Stethoscope } from 'lucide-react';
import { useNavigate, Link } from 'react-router';
import { useState } from 'react';
import { getUsers, setCurrentUser } from '../utils/storage';
import { toast } from 'sonner';

export function Login() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({ email: '', password: '' });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();
    
    const users = getUsers();
    const user = users.find(u => u.email === formData.email && u.password === formData.password);

    if (user) {
      setCurrentUser(user);
      toast.success(`¡Bienvenido de nuevo, ${user.name}!`);
      // Reload is sometimes necessary to trigger the router re-evaluation on protected routes
      window.location.href = '/';
    } else {
      toast.error('Correo o contraseña incorrectos');
    }
  };

  return (
    <div className="min-h-screen w-full flex items-center justify-center bg-background relative overflow-hidden transition-colors duration-500">
      {/* Background Glows */}
      <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-primary/10 rounded-full blur-[120px]" />
      <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-secondary/10 rounded-full blur-[120px]" />

      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-md p-8 z-10 bg-card border border-border rounded-3xl shadow-2xl"
      >
        <div className="flex flex-col items-center mb-10">
          <motion.div
            initial={{ rotate: -20, scale: 0.5 }}
            animate={{ rotate: 0, scale: 1 }}
            transition={{ type: "spring", stiffness: 260, damping: 20 }}
            className="w-16 h-16 rounded-2xl bg-gradient-to-br from-primary to-secondary flex items-center justify-center mb-6 shadow-xl shadow-primary/20"
          >
            <Stethoscope size={32} className="text-white" />
          </motion.div>
          <h1 className="text-3xl font-bold text-foreground mb-2">Bienvenido de nuevo</h1>
          <p className="text-muted-foreground text-center">
            Ingresa a tu cuenta de Diagnostica tu Mascota
          </p>
        </div>

        <form onSubmit={handleLogin} className="space-y-6">
          <div className="space-y-2">
            <label className="text-sm text-muted-foreground ml-1">Correo electrónico</label>
            <div className="relative group">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                <Mail size={20} />
              </div>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                placeholder="usuario@ejemplo.com"
                className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all placeholder:text-muted-foreground/50"
                required
              />
            </div>
          </div>

          <div className="space-y-2">
            <label className="text-sm text-muted-foreground ml-1">Contraseña</label>
            <div className="relative group">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                <Lock size={20} />
              </div>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                placeholder="••••••••"
                className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all placeholder:text-muted-foreground/50"
                required
              />
            </div>
            <div className="flex justify-end">
              <Link to="/forgot-password" className="text-xs text-primary hover:underline font-medium">
                ¿Olvidaste tu contraseña?
              </Link>
            </div>
          </div>

          <button
            type="submit"
            className="w-full bg-gradient-to-r from-primary to-secondary text-white py-3 rounded-xl font-bold flex items-center justify-center gap-2 hover:opacity-90 transition-all shadow-lg shadow-primary/20 active:scale-[0.98]"
          >
            <LogIn size={20} />
            Iniciar Sesión
          </button>
        </form>

        <div className="mt-8 text-center">
          <p className="text-muted-foreground text-sm">
            ¿No tienes una cuenta?{' '}
            <Link to="/register" className="text-primary hover:underline font-bold">
              Regístrate aquí
            </Link>
          </p>
        </div>
      </motion.div>
    </div>
  );
}
