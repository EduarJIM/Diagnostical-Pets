# Diagnostica tu Mascota

Sistema integral de orientacion de salud para mascotas, disponible como aplicacion web.

> Este repositorio contiene **solo el frontend** (React + Vite). La aplicacion de escritorio y el servidor quedan fuera del repositorio.

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
├── src/                          # Codigo de la aplicacion (React + Vite)
├── public/
├── guidelines/
├── README.md
└── ATTRIBUTIONS.md
```

## Aplicacion Web (frontend)

Tecnologias: React 18, Vite, TypeScript, Tailwind CSS 4, MUI, Radix UI, Framer Motion

### Ejecucion

```bash
npm install
npm run dev
```

## Licencia

Proyecto de uso educativo. Basado en el disenio de [Pet Health App UI Design](https://www.figma.com/design/XJ2DDH1CMFb985NKXuOLoz/Pet-Health-App-UI-Design).

## Contribuidores

- **EduarJIM** - Desarrollo web
