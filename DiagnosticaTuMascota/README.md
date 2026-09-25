# Diagnostica tu Mascota — Aplicación de Escritorio (WinForms)

Port a escritorio C# / .NET 10 (WinForms) de la aplicación web **"Diagnostica tu
Mascota"**, con el mismo diseño visual (tema claro/oscuro, sidebar, tarjetas,
toasts) y todos los flujos funcionales.

> **📖 Documentación del proyecto:** [`COMO-FUNCIONA.md`](COMO-FUNCIONA.md) explica
> **qué es Windows Forms**, qué funciones de la librería usa el proyecto, cómo
> arranca la aplicación y qué hace cada archivo.

> El triaje veterinario está portado 1:1 desde `triageDictionary.ts` de la web
> y está cubierto por pruebas unitarias (TDD con xUnit). Los resultados son
> **orientativos** y no sustituyen la consulta veterinaria.

## Requisitos

- **.NET 10 SDK**. Descargable desde https://dotnet.microsoft.com/download/dotnet/10.0
- **Visual Studio 2026** (Community o superior) con la carga de trabajo
  **"Desarrollo de escritorio con .NET"** (opcional — el CLI basta para
  compilar, probar y ejecutar).

## Abrir en Visual Studio 2026

1. Instala el SDK de .NET 10 y VS 2026 con la carga de trabajo *Desarrollo de
   escritorio con .NET*.
2. Abre el archivo de solución:
   `DiagnosticaTuMascota.sln` (raíz de esta carpeta).
3. Establece `src\DiagnosticaTuMascota` como proyecto de inicio (clic derecho →
   *Establecer como proyecto de inicio*).
4. Presiona **F5** (o Ctrl+F5) para ejecutar.
5. Para **editar la interfaz visualmente**: clic derecho sobre cualquier página de
   `src\DiagnosticaTuMascota\Pages\` → *Ver → Diseñador*.

## Compilar y ejecutar desde línea de comandos

```powershell
dotnet build DiagnosticaTuMascota.sln

# Ejecutar la aplicación (WinForms)
dotnet run --project src\DiagnosticaTuMascota
```

El ejecutable queda en
`src\DiagnosticaTuMascota\bin\Debug\net10.0-windows\DiagnosticaTuMascota.exe`.

## Pruebas (TDD con xUnit)

Toda la lógica de negocio (triaje, autenticación, alarmas, almacenamiento) se
desarrolló con TDD: primero la prueba en rojo, luego la implementación mínima
para ponerla en verde.

```powershell
dotnet test DiagnosticaTuMascota.sln
# Superado: 56, Con error: 0
```

Los archivos de prueba están en `tests\DiagnosticaTuMascota.Tests`:

| Archivo                       | Cubre                                        |
|-------------------------------|----------------------------------------------|
| `TriageEngineTests.cs`        | Diccionario de síntomas, niveles y sinergias |
| `AuthServiceTests.cs`         | Login, registro, recuperación de contraseña  |
| `AlarmServiceTests.cs`        | Comparación fecha/hora de alertas            |
| `AppStorageTests.cs`          | Alcance por usuario, JSON, usuarios          |

## Estructura del proyecto

```
DiagnosticaTuMascota/
├─ src/DiagnosticaTuMascota/          # Aplicación WinForms
│  ├─ Core/                           # Lógica pura (sin UI): TriageEngine,
│  │                                  #   AuthService, AlarmService, Models
│  ├─ Storage/                        # AppStorage + JsonFileStore + DemoData
│  ├─ Theme/                          # Paleta/colores y helpers de dibujo GDI+
│  ├─ Controls/                       # Botones, inputs, sidebar, modal, toasts…
│  └─ Pages/                          # Las 10 pantallas de la app
├─ tests/DiagnosticaTuMascota.Tests/  # Pruebas unitarias xUnit
├─ COMO-FUNCIONA.md                   # Guía: qué es WinForms y cómo funciona la app
└─ DiagnosticaTuMascota.sln
```

## Páginas

1. **Login** · 2. **Registro** · 3. **Recuperar contraseña** (3 pasos) ·
4. **Dashboard** · 5. **Mis Mascotas** · 6. **Análisis de Síntomas** ·
7. **Asignación de Consulta** (paso 2) · 8. **Resultado del Diagnóstico** ·
9. **Historial y Seguimiento** · 10. **Configuración**

## Persistencia de datos

Los datos se guardan como JSON en `%AppData%\DiagnosticaTuMascota\`, con un
**espacio aislado por usuario** (`key_usuario@ejemplo_com.json`): cada cuenta
tiene sus propias mascotas, historial y alertas. El tema claro/oscuro se guarda
de forma global.

## Notas

- **Alarmas:** la app revisa cada 5 segundos si alguna alerta activa coincide
  con la fecha/hora actual (igual que `useAlarms.ts`). El sonido usa
  `Console.Beep`; en equipos sin tarjeta de sonido se omite silenciosamente.
- **Tema:** por defecto arranca en oscuro, igual que la web.
- **Cuenta de demostración:** al registrarse se crea una cuenta con las
  mascotas de ejemplo (Max y Luna), una consulta histórica, un seguimiento
  programado y una alerta activa.