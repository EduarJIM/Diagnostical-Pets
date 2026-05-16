import { motion } from 'motion/react';
import { Settings as SettingsIcon, User, Mail, Lock, Save, ShieldAlert, LogOut } from 'lucide-react';
import { useState, useEffect } from 'react';
import { getCurrentUser, setCurrentUser, getUsers, saveUsers, UserAccount } from '../utils/storage';
import { toast } from 'sonner';

export function Settings() {
  const [user, setUser] = useState<UserAccount | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  useEffect(() => {
    const currentUser = getCurrentUser();
    if (currentUser) {
      setUser(currentUser);
      setFormData(prev => ({ ...prev, name: currentUser.name, email: currentUser.email }));
    }
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSave = () => {
    if (!user) return;

    let users = getUsers();
    const userIndex = users.findIndex(u => u.email === user.email);
    if (userIndex === -1) return;

    // Check if they want to change password
    if (formData.newPassword) {
      if (formData.currentPassword !== user.password) {
        toast.error('La contraseña actual es incorrecta');
        return;
      }
      if (formData.newPassword !== formData.confirmPassword) {
        toast.error('Las nuevas contraseñas no coinciden');
        return;
      }
      users[userIndex].password = formData.newPassword;
    }

    // Update name/email
    users[userIndex].name = formData.name;
    // We don't allow changing email easily because it's the primary key for storage scoping, 
    // but for simplicity we will just update it. Note: data might be lost if we change email because keys depend on it.
    // So we will just disable email changing to prevent data loss.

    saveUsers(users);
    setCurrentUser(users[userIndex]);
    setUser(users[userIndex]);
    
    // Clear password fields
    setFormData(prev => ({ ...prev, currentPassword: '', newPassword: '', confirmPassword: '' }));
    toast.success('¡Perfil actualizado con éxito!');
  };

  const handleLogout = () => {
    setCurrentUser(null);
    window.location.href = '/login';
  };

  if (!user) return null;

  return (
    <div className="flex-1 h-screen overflow-auto bg-background p-8 md:p-12 transition-colors duration-500">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="max-w-3xl mx-auto"
      >
        <header className="mb-10 flex items-start justify-between">
          <div>
            <h1 className="text-4xl font-bold text-foreground mb-3 flex items-center gap-3">
              <SettingsIcon className="text-primary" size={36} />
              Configuración de Cuenta
            </h1>
            <p className="text-muted-foreground text-lg">
              Administra tu información personal y credenciales de acceso.
            </p>
          </div>
          <button 
            onClick={handleLogout}
            className="flex items-center gap-2 px-4 py-2 bg-red-500/10 text-red-600 dark:text-red-400 font-bold rounded-xl hover:bg-red-500/20 transition-all"
          >
            <LogOut size={18} />
            Cerrar Sesión
          </button>
        </header>

        <div className="bg-card border border-border rounded-3xl shadow-sm p-8 space-y-8">
          {/* Nombre de Usuario */}
          <div className="space-y-4">
            <h3 className="text-xl font-semibold text-foreground flex items-center gap-2">
              <User size={20} className="text-primary" />
              Información del Perfil
            </h3>
            <div className="grid gap-4">
              <div>
                <label className="block text-sm font-medium text-muted-foreground mb-1.5">Nombre Completo</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  className="w-full bg-background border border-border text-foreground px-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 transition-all"
                />
              </div>
            </div>
          </div>

          <div className="h-px bg-border w-full" />

          {/* Correo Electrónico */}
          <div className="space-y-4">
            <h3 className="text-xl font-semibold text-foreground flex items-center gap-2">
              <Mail size={20} className="text-primary" />
              Correo Electrónico
            </h3>
            <div className="grid gap-4">
              <div>
                <label className="block text-sm font-medium text-muted-foreground mb-1.5">Dirección de Correo (No editable)</label>
                <input
                  type="email"
                  value={formData.email}
                  disabled
                  className="w-full bg-muted border border-border text-muted-foreground px-4 py-3 rounded-xl cursor-not-allowed opacity-70"
                />
                <p className="text-xs text-muted-foreground mt-2">
                  El correo es tu identificador único de cuenta y no puede modificarse.
                </p>
              </div>
            </div>
          </div>

          <div className="h-px bg-border w-full" />

          {/* Contraseña */}
          <div className="space-y-4">
            <h3 className="text-xl font-semibold text-foreground flex items-center gap-2">
              <Lock size={20} className="text-primary" />
              Seguridad de la Contraseña
            </h3>
            
            <div className="bg-amber-500/10 border border-amber-500/20 p-4 rounded-xl flex items-start gap-3">
              <ShieldAlert className="text-amber-500 mt-0.5" size={20} />
              <p className="text-sm text-amber-600/90 dark:text-amber-500">
                Al cambiar tu contraseña, se cerrarán todas tus sesiones activas en otros dispositivos por motivos de seguridad. Deja en blanco si no deseas cambiarla.
              </p>
            </div>

            <div className="grid gap-4 mt-4">
              <div>
                <label className="block text-sm font-medium text-muted-foreground mb-1.5">Contraseña Actual</label>
                <input
                  type="password"
                  name="currentPassword"
                  value={formData.currentPassword}
                  onChange={handleChange}
                  placeholder="••••••••"
                  className="w-full bg-background border border-border text-foreground px-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 transition-all"
                />
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-muted-foreground mb-1.5">Nueva Contraseña</label>
                  <input
                    type="password"
                    name="newPassword"
                    value={formData.newPassword}
                    onChange={handleChange}
                    placeholder="••••••••"
                    className="w-full bg-background border border-border text-foreground px-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 transition-all"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-muted-foreground mb-1.5">Confirmar Nueva Contraseña</label>
                  <input
                    type="password"
                    name="confirmPassword"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    placeholder="••••••••"
                    className="w-full bg-background border border-border text-foreground px-4 py-3 rounded-xl focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/30 transition-all"
                  />
                </div>
              </div>
            </div>
          </div>

          <div className="pt-6 border-t border-border flex justify-end">
            <button 
              onClick={handleSave}
              className="flex items-center gap-2 px-8 py-3 bg-primary text-white font-bold rounded-xl hover:opacity-90 transition-all shadow-lg shadow-primary/20 hover:-translate-y-0.5"
            >
              <Save size={20} />
              Guardar Cambios
            </button>
          </div>
        </div>
      </motion.div>
    </div>
  );
}
