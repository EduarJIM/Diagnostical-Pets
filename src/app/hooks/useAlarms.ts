import { useState, useEffect, useRef, useCallback } from 'react';
import { AlertItem } from '../pages/History';

export function useAlarms() {
  const [activeAlarm, setActiveAlarm] = useState<AlertItem | null>(null);
  
  const audioContextRef = useRef<AudioContext | null>(null);
  const intervalRef = useRef<number | null>(null);

  const startAudioLoop = useCallback(() => {
    try {
      if (!audioContextRef.current) {
        audioContextRef.current = new (window.AudioContext || (window as any).webkitAudioContext)();
      }
      
      const audioCtx = audioContextRef.current;
      
      if (audioCtx.state === 'suspended') {
        audioCtx.resume();
      }

      const createBeep = () => {
        const osc = audioCtx.createOscillator();
        const gain = audioCtx.createGain();
        
        osc.type = 'square';
        osc.frequency.setValueAtTime(800, audioCtx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(1200, audioCtx.currentTime + 0.1);
        osc.frequency.exponentialRampToValueAtTime(800, audioCtx.currentTime + 0.2);
        
        gain.gain.setValueAtTime(0, audioCtx.currentTime);
        gain.gain.linearRampToValueAtTime(0.5, audioCtx.currentTime + 0.05);
        gain.gain.exponentialRampToValueAtTime(0.01, audioCtx.currentTime + 0.4);
        
        osc.connect(gain);
        gain.connect(audioCtx.destination);
        
        osc.start(audioCtx.currentTime);
        osc.stop(audioCtx.currentTime + 0.5);
      };

      const createDoubleBeep = () => {
        createBeep();
        setTimeout(createBeep, 700); // Segundo pitido después de 700ms
      };

      createDoubleBeep();
      
      intervalRef.current = window.setInterval(() => {
        createDoubleBeep();
      }, 6000);

    } catch (e) {
      console.error("Audio playback failed", e);
    }
  }, []);

  const stopAudioLoop = useCallback(() => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
  }, []);

  const dismissAlarm = useCallback(() => {
    stopAudioLoop();
    
    if (activeAlarm) {
      const rawData = localStorage.getItem('alertsData');
      if (rawData) {
        const alerts: AlertItem[] = JSON.parse(rawData);
        const newAlerts = alerts.filter(a => a.id !== activeAlarm.id);
        localStorage.setItem('alertsData', JSON.stringify(newAlerts));
        
        window.dispatchEvent(new Event('storage'));
      }
    }
    
    setActiveAlarm(null);
  }, [activeAlarm, stopAudioLoop]);

  useEffect(() => {
    const checkInterval = setInterval(() => {
      // Don't trigger a new alarm if one is already active
      if (activeAlarm) return;

      try {
        const rawData = localStorage.getItem('alertsData');
        if (!rawData) return;
        const alerts: AlertItem[] = JSON.parse(rawData);

        const now = new Date();
        const currentLocalTimeStr = now.toTimeString().slice(0, 5); // "HH:MM"
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const currentDateStr = `${year}-${month}-${day}`;

        for (const alert of alerts) {
          if (alert.date === currentDateStr && alert.time === currentLocalTimeStr) {
            setActiveAlarm(alert);
            startAudioLoop();
            break; 
          }
        }
      } catch (e) {
        console.error('Error checking alarms:', e);
      }
    }, 5000);

    return () => {
      clearInterval(checkInterval);
      stopAudioLoop();
    };
  }, [activeAlarm, startAudioLoop, stopAudioLoop]);

  return { activeAlarm, dismissAlarm };
}
