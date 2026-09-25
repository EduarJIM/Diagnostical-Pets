# Diagnostica tu Mascota

Sistema integral de orientacion de salud para mascotas, disponible como aplicacion web y de escritorio.

## Descripcion

**Diagnostica tu Mascota** es una herramienta que permite a los duenos de mascotas evaluar el estado de salud de sus animales de compania mediante un analisis de sintomas. Ofrece orientacion general basada en protocolos de triaje veterinario, sin reemplazar el diagnostico profesional.

### Funcionalidades principales

- **Analisis de sintomas**: Evaluacion interactiva por categorias (general, respiratorio, digestivo, neurologico)
- **Gestion de mascotas**: Registro y administracion de perfiles clinicos (vacunas, veterinario, historial)
- **Diagnosticos**: Resultados con nivel de urgencia, implicaciones clinicas y primeros auxilios
- **Historial y seguimiento**: Consultas anteriores, proximos procesos y alertas de salud
- **Configuracion**: Perfil de usuario, correo y seguridad de contrasena
- **Tema claro/oscuro**: Toggle de tema con colores personalizados

## Estructura del proyecto

```
Diagnostical-Pets/
├── src/                          # Frontend web (React + Vite + Tailwind)
├── public/
├── guidelines/
├── DiagnosticaTuMascota/         # Aplicacion de escritorio (C# WinForms)
│   ├── src/DiagnosticaTuMascota/
│   │   ├── Controls/             # Controles personalizados GDI+
│   │   ├── Core/                 # Auth, triaje, alarmas, modelos
│   │   ├── Pages/                # Paginas (UserControls)
│   │   ├── Storage/              # Persistencia local
│   │   └── Theme/                # Paleta de colores y helpers
│   └── tests/                    # Pruebas unitarias
├── README.md
└── ATTRIBUTIONS.md
```

> La aplicacion de escritorio es **standalone**: no tiene backend ni servidor, toda la
> logica corre en el cliente y guarda los datos en archivos locales.

## Aplicacion Web (frontend)

Tecnologias: React 18, Vite, TypeScript, Tailwind CSS 4, MUI, Radix UI, Framer Motion

### Ejecucion

```bash
npm install
npm run dev
```

## Aplicacion de Escritorio (C# WinForms)

Tecnologias: C# / WinForms / GDI+ personalizado. **Sin backend**: toda la logica
(auth, triaje, historial) corre en el cliente y se persiste en archivos locales.

### Requisitos

- Windows 10 / 11
- **.NET 10 SDK** (el proyecto apunta a `net10.0-windows`) o Visual Studio 2026 con la
  carga de trabajo "Desarrollo de aplicaciones de escritorio .NET".
- En una maquina que solo tiene **.NET 8**, compila indicando el framework:
  `dotnet run -p:AppTargetFramework=net8.0-windows --project src/DiagnosticaTuMascota`

### Como abrirla en otra maquina

```bash
git clone https://github.com/EduarJIM/Diagnostical-Pets.git
cd Diagnostical-Pets/DiagnosticaTuMascota
dotnet restore
dotnet run --project src/DiagnosticaTuMascota
```

Tambien puedes abrir `DiagnosticaTuMascota.sln` en Visual Studio y pulsar F5.

Para generar el ejecutable: `dotnet publish src/DiagnosticaTuMascota -c Release -r win-x64 --self-contained false`

### Pruebas

```bash
dotnet test tests/DiagnosticaTuMascota.Tests
```

> Las pruebas y la app solo **ejecutan** en Windows (`Microsoft.WindowsDesktop.App`).
> Desde Linux/macOS se pueden **compilar** gracias a `EnableWindowsTargeting=true`,
> pero no ejecutarse.

### Paginas implementadas

| Pagina | Descripcion |
|--------|-------------|
| Login | Formulario de inicio de sesion con tarjeta centrada |
| Registro | Creacion de cuenta con validacion de campos |
| Recuperar contrasena | Proceso de 3 pasos (email, telefono, nueva contrasena) |
| Dashboard | Hero con animacion, tarjetas de features, disclaimer medico |
| Mis Mascotas | Grid de tarjetas de mascotas con perfil clinico |
| Analisis de Sintomas | Seleccion por categorias con checkboxes interactivos |
| Consulta Paso 2 | Asignacion de consulta con combo y campo de contexto |
| Resultado Diagnostico | Banner de severidad, implicaciones y acciones |
| Historial | 3 tabs: consultas, proximos procesos, alertas |
| Configuracion | Perfil, correo (deshabilitado), cambio de contrasena |

### Controles personalizados

- `RoundedPanel` / `RoundedButton` / `RoundedTextBox` / `RoundedComboBox` - controles con esquinas redondeadas
- `GradientPanel` - Panel con relleno degradado
- `PulsingIconBox` - Circulo con animacion de pulso
- `ToggleSwitch` - Interruptor de tema claro/oscuro
- `SidebarControl` - Navegacion lateral compartida

### Arquitectura

- **MainForm**: Contenedor principal con sidebar y panel de contenido con scroll
- **UserControls**: Cada pagina es un UserControl que se intercambia en el panel de contenido
- **Core**: `AuthService`, `TriageEngine`, `AlarmService`, `Models` (logica de negocio)
- **Storage**: `AppStorage` (persistencia local) y `DemoData` (datos de ejemplo)
- **AppTheme**: Paleta de colores centralizada (light/dark) y helpers de dibujo GDI+

## Licencia

Proyecto de uso educativo. Basado en el disenio de [Pet Health App UI Design](https://www.figma.com/design/XJ2DDH1CMFb985NKXuOLoz/Pet-Health-App-UI-Design).

## Contribuidores

- **EduarJIM** - Desarrollo web
