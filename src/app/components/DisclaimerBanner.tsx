import { AlertTriangle } from 'lucide-react';

export function DisclaimerBanner() {
  return (
    <div className="relative overflow-hidden rounded-2xl border border-destructive/20 bg-destructive/5 p-6 shadow-sm">
      <div className="flex items-start gap-4">
        <div className="w-10 h-10 rounded-lg bg-destructive/10 flex items-center justify-center flex-shrink-0">
          <AlertTriangle size={20} className="text-destructive" />
        </div>
        <div className="flex-1">
          <h4 className="text-base font-bold text-foreground mb-2">Aviso Médico Importante</h4>
          <p className="text-sm text-muted-foreground leading-relaxed">
            Esta herramienta proporciona orientación general y NO reemplaza el diagnóstico veterinario profesional.
            Siempre consulte con un veterinario certificado para obtener un diagnóstico preciso y tratamiento adecuado para su mascota.
          </p>
        </div>
      </div>

      <div className="absolute top-0 right-0 w-40 h-40 bg-[#ff5252]/10 rounded-full blur-3xl -z-10" />
    </div>
  );
}
