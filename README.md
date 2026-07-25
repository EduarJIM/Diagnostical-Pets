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
├── src/                          # Aplicacion web (React + Vite + Tailwind)
├── public/
├── guidelines/
├── DiagnosticaTuMascota/         # Aplicacion de escritorio (C# WinForms .NET 10)
│   ├── Controls/                 # Controles personalizados GDI+
│   ├── Forms/                    # Formularios y paginas
│   ├── Sidebar/                  # Navegacion lateral
│   ├── Theme/                    # Paleta de colores y helpers
│   └── Program.cs
├── README.md
└── ATTRIBUTIONS.md
```

## Aplicacion Web

Tecnologias: React 18, Vite, Tailwind CSS 4, Radix UI, Framer Motion

### Ejecucion

```bash
npm install
npm run dev
```

## Aplicacion de Escritorio (C# WinForms)

Tecnologias: .NET 10, WinForms, GDI+ personalizado

### Requisitos

- .NET 10 SDK
- Visual Studio Community 2026 (recomendado)

### Ejecucion

```bash
cd DiagnosticaTuMascota
dotnet run
```

O abrir `DiagnosticaTuMascota.csproj` en Visual Studio y presionar F5.

### Paginas implementadas

| Pagina | Descripcion |
|--------|-------------|
| Login | Formulario de inicio de sesion con tarjeta centrada |
| Registro | Creacion de cuenta con validacion de campos |
| Recuperar contrasena | Proceso de 3 pasos (email, telefono, nueva contrasena) |
| Dashboard | Hero con animacion, tarjetas de features, disclaimer medico |
| Mis Mascotas | Grid de tarjetas de mascotas con perfil clinico |
| Analisis de Sintomas | Selecion por categorias con checkboxes interactivos |
| Consulta Paso 2 | Asignacion de consulta con combo y campo de contexto |
| Resultado Diagnostico | Banner de severidad, implicaciones y acciones |
| Historial | 3 tabs: consultas, proximos procesos, alertas |
| Configuracion | Perfil, correo (deshabilitado), cambio de contrasena |

### Controles personalizados

- `RoundedPanel` - Panel con esquinas redondeadas
- `RoundedButton` - Boton con gradiente y estados hover/pressed
- `RoundedTextBox` - Campo de texto con borde redondeado y placeholder
- `RoundedComboBox` - ComboBox owner-draw con borde redondeado
- `GradientPanel` - Panel con relleno degradado
- `GlowCircle` - Circulo con animacion de pulso
- `ToggleSwitch` - Interruptor de tema claro/oscuro
- `SidebarControl` - Navegacion lateral compartida

### Arquitectura

- **MainForm**: Contenedor principal con sidebar y panel de contenido con scroll
- **UserControls**: Cada pagina es un UserControl que se intercambia en el panel de contenido
- **AppTheme**: Paleta de colores centralizada (light/dark), fuentes y helpers de dibujo GDI+
- **Scroll**: Manejado por el contenedor de contenido, las paginas se expanden con `AutoSize`
- **Navegacion**: Sidebar emite eventos de navegacion, MainForm intercambia el contenido

## Licencia

Proyecto de uso educativo. Basado en el disenio de [Pet Health App UI Design](https://www.figma.com/design/XJ2DDH1CMFb985NKXuOLoz/Pet-Health-App-UI-Design).

## Contribuidores

- **EduarJIM** - Desarrollo web y de escritorio
