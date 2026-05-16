import { useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route, useLocation } from 'react-router';
import { Sidebar } from './components/Sidebar';
import { Dashboard } from './pages/Dashboard';
import { ConsultationStep2 } from './pages/ConsultationStep2';
import { DiagnosticResult } from './pages/DiagnosticResult';
import { History } from './pages/History';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { ForgotPassword } from './pages/ForgotPassword';
import { SymptomAnalysis } from './pages/SymptomAnalysis';
import { Settings } from './pages/Settings';
import { Pets } from './pages/Pets';
import { Toaster } from 'sonner';
import { useAlarms } from './hooks/useAlarms';
import { ActiveAlarmModal } from './components/ActiveAlarmModal';
import { getCurrentUser } from './utils/storage';
import { Navigate } from 'react-router';

function AppContent() {
  const location = useLocation();
  const [theme, setTheme] = useState<'light' | 'dark'>('dark');
  const isAuthRoute = location.pathname === '/login' || location.pathname === '/register' || location.pathname === '/forgot-password';
  
  const user = getCurrentUser();

  const { activeAlarm, dismissAlarm } = useAlarms();

  if (!user && !isAuthRoute) {
    return <Navigate to="/login" replace />;
  }

  if (user && isAuthRoute) {
    return <Navigate to="/" replace />;
  }

  const toggleTheme = () => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  };

  useEffect(() => {
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }, [theme]);

  return (
    <div className={`flex h-screen bg-background text-foreground transition-colors duration-500 ${theme}`}>
      {!isAuthRoute && <Sidebar theme={theme} toggleTheme={toggleTheme} />}
      <main className="flex-1 overflow-auto relative">
        {/* Decorative Background Elements */}
        <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none -z-10">
          <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] bg-primary/10 rounded-full blur-[120px] animate-pulse" />
          <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] bg-secondary/10 rounded-full blur-[120px] animate-pulse delay-700" />
        </div>

        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/consultation" element={<SymptomAnalysis />} />
          <Route path="/consultation/step2" element={<ConsultationStep2 />} />
          <Route path="/result" element={<DiagnosticResult />} />
          <Route path="/history" element={<History />} />
          <Route path="/settings" element={<Settings />} />
          <Route path="/pets" element={<Pets />} />
        </Routes>
        <Toaster position="top-right" richColors theme={theme} />
        <ActiveAlarmModal alarm={activeAlarm} onDismiss={dismissAlarm} />
      </main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AppContent />
    </BrowserRouter>
  );
}