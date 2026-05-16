import { motion } from 'motion/react';
import { Mail, Lock, Phone, ArrowLeft, KeyRound, CheckCircle2 } from 'lucide-react';
import { useNavigate, Link } from 'react-router';
import { useState } from 'react';
import { getUsers, saveUsers } from '../utils/storage';
import { toast } from 'sonner';

export function ForgotPassword() {
  const navigate = useNavigate();
  const [step, setStep] = useState(1);
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleVerifyEmail = (e: React.FormEvent) => {
    e.preventDefault();
    const users = getUsers();
    const user = users.find(u => u.email === email);
    if (user) {
      setStep(2);
    } else {
      toast.error('No se encontró ninguna cuenta con ese correo');
    }
  };

  const handleVerifyPhone = (e: React.FormEvent) => {
    e.preventDefault();
    const users = getUsers();
    const user = users.find(u => u.email === email && u.phone === phone);
    if (user) {
      setStep(3);
    } else {
      toast.error('El número de teléfono no coincide con nuestros registros');
    }
  };

  const handleResetPassword = (e: React.FormEvent) => {
    e.preventDefault();
    if (newPassword !== confirmPassword) {
      toast.error('Las contraseñas no coinciden');
      return;
    }

    const users = getUsers();
    const userIndex = users.findIndex(u => u.email === email);
    if (userIndex !== -1) {
      users[userIndex].password = newPassword;
      saveUsers(users);
      toast.success('¡Contraseña restablecida con éxito!');
      navigate('/login');
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
        className="w-full max-w-md p-8 z-10 bg-card border border-border rounded-3xl shadow-2xl"
      >
        <Link 
          to="/login" 
          className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-primary transition-colors mb-6 group"
        >
          <ArrowLeft size={16} className="group-hover:-translate-x-1 transition-transform" />
          Volver al inicio de sesión
        </Link>

        <div className="flex flex-col items-center mb-8">
          <motion.div
            initial={{ rotate: -20, scale: 0.5 }}
            animate={{ rotate: 0, scale: 1 }}
            className="w-16 h-16 rounded-2xl bg-gradient-to-br from-primary to-secondary flex items-center justify-center mb-6 shadow-xl shadow-primary/20"
          >
            <KeyRound size={32} className="text-white" />
          </motion.div>
          <h1 className="text-3xl font-bold text-foreground mb-2">Restablecer Contraseña</h1>
          <p className="text-muted-foreground text-center text-sm">
            {step === 1 && "Ingresa tu correo para comenzar el proceso"}
            {step === 2 && "Verifica tu identidad con tu número de teléfono"}
            {step === 3 && "Ingresa tu nueva contraseña"}
          </p>
        </div>

        {/* Progress Dots */}
        <div className="flex justify-center gap-2 mb-8">
          {[1, 2, 3].map((s) => (
            <div 
              key={s} 
              className={`h-1.5 rounded-full transition-all duration-300 ${
                s === step ? 'w-8 bg-primary' : 'w-2 bg-muted'
              }`} 
            />
          ))}
        </div>

        {step === 1 && (
          <form onSubmit={handleVerifyEmail} className="space-y-6">
            <div className="space-y-2">
              <label className="text-sm text-muted-foreground ml-1">Correo electrónico</label>
              <div className="relative group">
                <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                  <Mail size={20} />
                </div>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="usuario@ejemplo.com"
                  className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all"
                  required
                />
              </div>
            </div>
            <button
              type="submit"
              className="w-full bg-primary text-white py-3 rounded-xl font-bold hover:opacity-90 transition-all shadow-lg shadow-primary/20"
            >
              Continuar
            </button>
          </form>
        )}

        {step === 2 && (
          <form onSubmit={handleVerifyPhone} className="space-y-6">
            <div className="space-y-2">
              <label className="text-sm text-muted-foreground ml-1">Número de teléfono</label>
              <div className="relative group">
                <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                  <Phone size={20} />
                </div>
                <input
                  type="tel"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  placeholder="+1 234 567 890"
                  className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all"
                  required
                />
              </div>
              <p className="text-[11px] text-muted-foreground mt-2 px-1">
                Por seguridad, requerimos el teléfono registrado en tu cuenta.
              </p>
            </div>
            <button
              type="submit"
              className="w-full bg-primary text-white py-3 rounded-xl font-bold hover:opacity-90 transition-all shadow-lg shadow-primary/20"
            >
              Verificar Identidad
            </button>
          </form>
        )}

        {step === 3 && (
          <form onSubmit={handleResetPassword} className="space-y-6">
            <div className="space-y-4">
              <div className="space-y-2">
                <label className="text-sm text-muted-foreground ml-1">Nueva Contraseña</label>
                <div className="relative group">
                  <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                    <Lock size={20} />
                  </div>
                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    placeholder="••••••••"
                    className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all"
                    required
                  />
                </div>
              </div>
              <div className="space-y-2">
                <label className="text-sm text-muted-foreground ml-1">Confirmar Nueva Contraseña</label>
                <div className="relative group">
                  <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground group-focus-within:text-primary transition-colors">
                    <CheckCircle2 size={20} />
                  </div>
                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    placeholder="••••••••"
                    className="w-full bg-background border border-border text-foreground pl-12 pr-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all"
                    required
                  />
                </div>
              </div>
            </div>
            <button
              type="submit"
              className="w-full bg-gradient-to-r from-primary to-secondary text-white py-3 rounded-xl font-bold hover:opacity-90 transition-all shadow-lg shadow-primary/20"
            >
              Actualizar Contraseña
            </button>
          </form>
        )}
      </motion.div>
    </div>
  );
}
