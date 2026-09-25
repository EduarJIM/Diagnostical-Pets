# DiagnosticaTuMascota — Cómo funciona

Guía de referencia de la aplicación de escritorio: **qué es Windows Forms**, cómo está
construido este proyecto y **qué funciones de la librería usa**.

---

## Índice

1. [¿Qué es Windows Forms?](#1-qué-es-windows-forms)
2. [¿Qué es esta aplicación?](#2-qué-es-esta-aplicación)
3. [Ciclo de vida: cómo arranca la app](#3-ciclo-de-vida-cómo-arranca-la-app)
4. [Las funciones de la librería que usa la app](#4-las-funciones-de-la-librería-que-usa-la-app)
5. [Mapa de archivos del proyecto](#5-mapa-de-archivos-del-proyecto)
6. [Controles personalizados](#6-controles-personalizados)
7. [Abrir y usar Visual Studio 2026](#7-abrir-y-usar-visual-studio-2026)
8. [Dónde aprender más](#8-dónde-aprender-más)

---

## 1. ¿Qué es Windows Forms?

**Windows Forms (WinForms)** es la biblioteca de Microsoft incluida en .NET para crear
**aplicaciones de escritorio nativas de Windows**. Está en el espacio de nombres
`System.Windows.Forms` y forma parte del paquete `Microsoft.WindowsDesktop.App`.

No es una página web, no necesita navegador y no se ejecuta en Linux ni en macOS:
genera un ejecutable `.exe` real de escritorio.

### Qué te da la librería

| Pieza | Para qué sirve | Ejemplo en esta app |
|---|---|---|
| `Form` | Ventana principal / ventanas | `MainForm`, `ModalForm`, `PetModalForm` |
| `UserControl` | Bloque de UI reutilizable | Todas las páginas, `RoundedButton`, `NavItem` |
| `Control` | Cualquier control visual | `SeverityBadge`, `HeroArt`, `ProgressBarSkin` |
| Layout (`Dock`, `Anchor`) | Posicionar sin código manual | Menú lateral pegado a la izquierda |
| Eventos (`Click`, `Resize`) | Reaccionar a la interacción del usuario | Botones que navegan entre páginas |
| GDI+ (`Graphics`) | Dibujar formas, bordes redondeados, degradados | `RoundedPanel`, `GradientPanel` |
| `Timer` | Acciones periódicas en el hilo de UI | Icono que "latido", toast que desaparece |

### WinForms vs otras opciones

| | WinForms | WPF | Web (React) |
|---|---|---|---|
| Sistema | Solo Windows | Solo Windows | Cualquiera con navegador |
| Tecnología | Dibujo con GDI+ | XAML + vectores | HTML/CSS/JS |
| Diseñador visual | ✅ Incluido | ✅ Incluido | ❌ (es código web) |
| Ideal para | Formularios y utilidades | Interfaces muy animadas | Aplicaciones web |

> Este proyecto tiene **dos versiones**: la web (carpeta raíz, React) y la de escritorio
> (esta carpeta, WinForms). La de escritorio es la que se compila y ejecuta en Windows.

### El Diseñador de Visual Studio

Visual Studio incluye un **diseñador visual** para WinForms: abres un `Form` o
`UserControl` y puedes **arrastrar, mover y redimensionar controles con el mouse**
en vez de escribir coordenadas a mano.

En este proyecto la interfaz **se construye por código** dentro de
`InitializeComponent()`, por eso no hay archivos `*.Designer.cs` ni `*.resx`
generados. Eso no impide abrir el Diseñador: los controles están declarados como
campos de la clase, que es exactamente lo que el Diseñador necesita para editarlos.

---

## 2. ¿Qué es esta aplicación?

**DiagnosticaTuMascota** (Diagnostical Pets) es una app de escritorio para registrar
mascotas, describir síntomas y obtener una orientación básica sobre qué tan urgente es
el caso.

- **Lenguaje:** C# 12
- **Framework:** .NET 10 — `net10.0-windows`
- **Interfaz:** Windows Forms (`System.Windows.Forms`)
- **Datos:** archivos JSON locales (sin servidor, sin base de datos)
- **Project file:** `src/DiagnosticaTuMascota/DiagnosticaTuMascota.csproj`

### Funcionalidades

1. **Autenticación** — registro, inicio de sesión, recuperación de contraseña.
2. **Panel (Dashboard)** — resumen, alertas y próximas consultas.
3. **Mascotas** — alta, edición y ficha de cada mascota (`PetCard`).
4. **Análisis de síntomas** — selección de síntomas en pasos y motor de triaje.
5. **Resultado** — nivel de severidad, implicaciones médicas y recomendaciones.
6. **Historial** — consultas anteriores con detalle en modal.
7. **Ajustes** — preferencias, notificaciones y alarmas.
8. **Alarmas y avisos** — ventanas modales de alarma activa y notificaciones emergentes.

---

## 3. Ciclo de vida: cómo arranca la app

### 3.1 Punto de entrada — `Program.cs` (12 líneas)

```csharp
[STAThread]                              // obligatorio para WinForms
private static void Main()
{
    ApplicationConfiguration.Initialize(); // carga tema, fuentes y DPI del sistema
    var storage = Storage.AppStorage.CreateDefault(); // abre/crea el JSON
    Application.Run(new MainForm(storage));  // arranca el bucle de mensajes
}
```

- `[STAThread]`: WinForms exige un hilo de un solo apartment.
- `ApplicationConfiguration.Initialize()`: aplica la configuración generada
  (HighDPI, fuentes).
- `Application.Run(...)`: inicia el **bucle de mensajes**; la app vive hasta que
  se cierra la ventana.

### 3.2 Ventana principal — `MainForm.cs`

`MainForm` es un `Form` que:

- crea el **menú lateral** (`SidebarControl`) y la zona de contenido;
- implementa `INavigator`, por lo que **todas las páginas pueden navegar**;
- cambia de página con `Navigate(AppPage, NavState?)` y muestra toasts.

### 3.3 Navegación entre páginas

`Pages/PageBase.cs` define tres piezas:

```csharp
public enum AppPage { Login, Register, ForgotPassword, Dashboard, Pets,
                      SymptomAnalysis, ConsultationStep2, DiagnosticResult, History, Settings }

public sealed class NavState            // datos que viajan entre páginas
{
    public List<string>? SelectedSymptoms { get; set; }
    public string? PetName { get; set; }
    public string? AdditionalInfo { get; set; }
    public object? Extra { get; set; }
}

public interface INavigator             // lo que una página puede hacer
{
    AppStorage Storage { get; }
    void Navigate(AppPage page, NavState? state = null);
    void ShowToast(string message, ToastKind kind = ToastKind.Info);
    void Logout();
}
```

Todas las páginas heredan de `PageBase`, que ya aplica el fondo del tema y se
suscribe a `AppTheme.ThemeChanged`.

### 3.4 Flujo completo

```
Program.Main
   └─ Application.Run(MainForm)
         └─ LoginPage ──(credenciales)──► AuthService
               └─ Navigate(Dashboard, NavState)
                     └─ PetsPage ──► SymptomAnalysisPage
                                          └─ TriageEngine ─► DiagnosticResultPage
                                                                └─ HistoryPage / SettingsPage
MainForm.ShowToast ─► Toast (ventana flotante)
AlarmService       ─► ActiveAlarmModal (ventana modal)
Todo                ─► AppStorage ─► archivos JSON
```

---

## 4. Las funciones de la librería que usa la app

### 4.1 Jerarquía de clases base

Todo en WinForms hereda de `Control`. Los niveles usados aquí son:

```
Control                     (System.Windows.Forms.Control)
 ├── Label                  texto
 ├── Button                 botón estándar
 ├── Panel                  contenedor
 ├── UserControl            bloque reutilizable
 │    ├── PageBase ───────── todas las páginas
 │    ├── RoundedButton, RoundedTextBox, RoundedComboBox, NavItem, ToggleSwitch…
 │    └── SidebarControl, FeatureCard, PetCard…
 ├── Form                   ventana
 │    ├── MainForm
 │    └── ModalForm ── PetModalForm, AlertModal, ActiveAlarmModal, HistoryDetailModal
 └── SeverityBadge, HeroArt, ProgressBarSkin, ProgressIndicator (dibujo GDI+)
```

**Cómo se declara una clase propia:**

```csharp
public class RoundedButton : Button { }        // hereda de un control nativo
public class RoundedPanel : Panel { }          // panel con esquinas redondeadas
public class ThemeLabel : Label { }            // label con variantes de estilo
public class PetCard : RoundedPanel { }        // reutiliza otro control propio
public class PetModalForm : ModalForm { }     // reutiliza otro form propio
```

> Al heredar de un control de la librería, `PetCard` **hereda automáticamente** todo:
> `Text`, `BackColor`, `Dock`, `Click`, `Font`, etc.

### 4.2 Propiedades más usadas

| Propiedad | Qué hace | Ocurrencias en el proyecto |
|---|---|---|
| `Text` | Texto visible de un label o botón | 134 |
| `BackColor` | Color de fondo | 97 |
| `Location` | Posición (X, Y) desde la esquina superior izquierda | 83 |
| `Dock` | Pegar a un borde o rellenar el padre (`DockStyle.Fill`) | 78 |
| `Anchor` | Fijar distancias a los bordes al redimensionar | 69 |
| `Size` | Ancho y alto | 56 |
| `AutoSize` | Ajustar el tamaño al contenido | 56 |
| `Margin` | Espacio exterior respecto a otros controles | 48 |
| `Font` | Tipografía | 41 |
| `Padding` | Espacio **interno** del contenedor | 28 |
| `ForeColor` | Color del texto | 22 |
| `Enabled` / `Visible` | Activar o ocultar | habitual |
| `Cursor` | Cursor al pasar el mouse (`Cursors.Hand`) | habitual |

**`Dock` y `Anchor` son la clave del layout en WinForms:**

```csharp
// Panel que se pega a la izquierda y se estira a toda la altura
sidebar.Dock = DockStyle.Left;
sidebar.Width = 264;

// La zona de contenido ocupa todo el espacio restante
content.Dock = DockStyle.Fill;
```

> `DockStyle.Fill` es la razón por la que las páginas se ven bien a cualquier
> resolución: cada página empieza con `Dock = DockStyle.Fill;` en `PageBase`.

### 4.3 Contenedores y la propiedad `Controls`

Un contenedor es cualquier control que tiene otros controles dentro. La propiedad
`Controls` los guarda en orden de dibujo (el último se ve encima).

```csharp
var card = new RoundedPanel { Dock = DockStyle.Fill };
card.Controls.Add(new ThemeLabel { Text = "Nombre", TextKind = TextKind.Title });
card.Controls.Add(new RoundedTextBox { Location = new Point(20, 60) });
```

Contenedores usados: `Panel`, `UserControl`, `Form` y los propios
`RoundedPanel`, `PageBody`, `CenteredHost` y `OverlayForm`.

### 4.4 Eventos

Un evento es "código que se ejecuta cuando algo pasa". Se conecta con `+=`.

| Evento | Cuándo se dispara | Ejemplo en la app |
|---|---|---|
| `Click` | Clic en botón o control | 48 usos: navegar, guardar, activar alarma |
| `Resize` | Cambia el tamaño de la ventana o panel | 10 usos: recalcular anchos responsivos |
| `KeyDown` | Se pulsa una tecla | 3 usos: Enter para enviar formulario |
| `SelectedIndexChanged` | Cambia el elemento seleccionado | Combobox de especies o síntomas |
| `CheckedChanged` | Cambia el estado marcado | `ToggleSwitch` de notificaciones |
| `Paint` | Hay que dibujar algo a mano | Bordes redondeados de `RoundedPanel` |

```csharp
button.Click += (_, _) => _navigator.Navigate(AppPage.Dashboard);
window.Resize += (_, _) => RecalculateLayout();
```

> El `_` es el parámetro ignorado: en WinForms los manejadores siempre reciben un
> argumento (`ClickEventArgs`, `PaintEventArgs`…) aunque no lo necesites.

### 4.5 Dibujo personalizado con GDI+

GDI+ es el motor de dibujo 2D de Windows. WinForms lo expone sobrescribiendo
`OnPaint`, que es **la forma más común de crear efectos que la librería no trae**
(esquinas redondeadas, degradados, barras, circulares).

| API | Para qué | Ocurrencias |
|---|---|---|
| `OnPaint` | Método que se sobrescribe para dibujar | 24 |
| `Graphics` | Objeto de dibujo principal | 44 |
| `SolidBrush` | Relleno con color plano | 30 |
| `Pen` | Contorno (líneas, bordes) | 9 |
| `GraphicsPath` | Forma geométrica (rectángulo redondeado) | 4 |
| `FillPath` | Rellena una forma | 25 |
| `FillEllipse` | Rellena un círculo (barras, iconos) | 7 |
| `LinearGradientBrush` | Degradado de dos colores | 8 |
| `Region` | Recorta el control con una forma | 6 |
| `SmoothingMode` | Antialiasing (bordes suaves) | 38 |
| `DrawString` | Escribe texto dibujado a mano | 2 |
| `StringFormat` | Alineación del texto dibujado | 1 |

**Ejemplo real: un panel con esquinas redondeadas**

```csharp
protected override void OnPaint(PaintEventArgs e)
{
    base.OnPaint(e);
    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;   // bordes suaves

    var path = new GraphicsPath();
    var diameter = 18;
    path.AddArc(0, 0, diameter, diameter, 180, 90);                     // esquinas
    path.AddArc(Width - diameter, 0, diameter, diameter, 270, 90);
    path.AddArc(Width - diameter, Height - diameter, diameter, diameter, 0, 90);
    path.AddArc(0, Height - diameter, diameter, diameter, 90, 90);
    path.CloseFigure();

    using var brush = new SolidBrush(BackColor);
    e.Graphics.FillPath(brush, path);   // rellena la forma
    using var pen = new Pen(BorderColor, 1.2f);
    e.Graphics.DrawPath(pen, path);     // dibuja el contorno
    Region = new Region(path);          // recorta el control con esa forma
}
```

Ese es el corazón de `RoundedPanel`, `RoundedButton`, `SeverityBadge` y los demás
controles dibujados: **el `Region` es lo que hace que las esquinas realmente se
vean recortadas.**

### 4.6 Timers

`System.Windows.Forms.Timer` ejecuta código en el hilo de la interfaz cada
`Interval` milisegundos, y solo mientras está `Start()`.

| Uso | Componente |
|---|---|
| Animación de pulso en el icono de alarma | `PulsingIconBox` (9 timers en el proyecto) |
| Mostrar y ocultar notificaciones flotantes | `Toast` |
| Refrescar "última revisión" del dashboard | `DashboardPage` |
| Progress bar de análisis | `ProgressIndicator` |

```csharp
_timer = new System.Windows.Forms.Timer { Interval = 16 };  // ~60 fps
_timer.Tick += (_, _) => Invalidate();   // redibuja en cada tic
_timer.Start();
```

> `Invalidate()` pide repintar; el Windows Forms es quien decide el momento exacto,
> por eso la animación no bloquea la interfaz.

### 4.7 Formularios y modales

| API | Qué hace |
|---|---|
| `Application.Run(form)` | Muestra la ventana principal y arranca la app |
| `form.Show()` | Abre otra ventana no bloqueante |
| `form.ShowDialog()` | Abre una ventana **modal**: bloquea la de atrás hasta que se cierre |
| `StartPosition` | Dónde aparece: `FormStartPosition.CenterScreen` |
| `FormBorderStyle` | Borde: `FixedDialog` en modales, `Sizable` en la principal |
| `WindowState` | `Maximized`, `Minimized`, `Normal` |
| `Close()` / `Dispose()` | Cerrar y liberar recursos |

`ModalHost` + `OverlayForm` implementan los modales: una capa oscura a pantalla
completa detrás de la ventana (`ShowDialog()`).

### 4.8 Mensajes al usuario

- `MessageBox.Show(...)` — 4 usos: cuadros del sistema (advertencias, errores).
- **Toasts propios** (`Toast`, `ToastKind.Success/Error/Info`) — notificaciones
  flotantes con el estilo de la app, mostradas con `INavigator.ShowToast()`.

### 4.9 Detectar el Diseñador

`DesignTime.cs` comprueba con `LicenseManager.UsageMode` si el código se está
ejecutando dentro de Visual Studio:

```csharp
public static bool IsActive =>
    LicenseManager.UsageMode == LicenseUsageMode.Designtime;
```

Si es tiempo de diseño, la app usa `MemoryJsonStore` (datos en memoria, sin
tocar los JSON) y `DesignTimeNavigator`, y `PrimeLayout()` recorre los controles
para forzar su medición antes de mostrarlos en el Diseñador.

---

## 5. Mapa de archivos del proyecto

Raíz: `DiagnosticaTuMascota/src/DiagnosticaTuMascota/` — 33 archivos, ~6 600 líneas.

### Entrada y estructura

| Archivo | Líneas | Responsabilidad |
|---|---:|---|
| `Program.cs` | 12 | Punto de entrada: `Application.Run(new MainForm(...))` |
| `MainForm.cs` | 322 | Ventana principal, menú lateral, `INavigator` |
| `DesignTime.cs` | 103 | Support para el Diseñador de VS (storage y navigator falsos) |

### `Pages/` — las 10 pantallas

| Archivo | Líneas | Pantalla |
|---|---:|---|
| `AuthPages.cs` | 653 | `LoginPage`, `RegisterPage`, `ForgotPasswordPage` |
| `DashboardPage.cs` | 324 | Resumen, alertas, próximas consultas |
| `PetsPage.cs` | 476 | Lista y edición de mascotas |
| `SymptomAnalysisPage.cs` | 520 | Selección de síntomas |
| `ConsultationStep2Page.cs` | 278 | Datos complementarios del caso |
| `DiagnosticResultPage.cs` | 460 | Resultado del triaje |
| `HistoryPage.cs` | 684 | Historial de consultas |
| `SettingsPage.cs` | 310 | Preferencias y alarmas |
| `PageBase.cs` | 45 | `AppPage`, `NavState`, `INavigator`, clase base `PageBase` |

### `Controls/` — 13 controles propios

| Archivo | Líneas | Controles |
|---|---:|---|
| `RoundedButton.cs` | 152 | `RoundedButton : Button`, `ButtonVariant` |
| `RoundedTextBox.cs` | 220 | `RoundedTextBox` con placeholder y modo contraseña |
| `RoundedComboBox.cs` | 131 | `RoundedComboBox` con lista desplegable propia |
| `Panels.cs` | 125 | `RoundedPanel`, `GradientPanel`, `PageBody` |
| `CenteredHost.cs` | 56 | Centra contenido dentro de un panel |
| `ThemeLabel.cs` | 88 | `ThemeLabel`, `TextKind` (title/subtitle/caption…) |
| `BadgesAndCards.cs` | 189 | `SeverityBadge`, `FeatureCard`, `PetCard` |
| `NavItems.cs` | 139 | `NavItem`, `PillButton`, `CategoryChip`, `SymptomChip` |
| `SidebarControl.cs` | 145 | `SidebarControl`, `SidebarPage` |
| `Toast.cs` | 118 | `Toast`, `ToastKind` |
| `ModalHost.cs` | 186 | `ModalHost`, `OverlayForm`, `ModalForm` |
| `PulsingIconBox.cs` | 90 | Icono animado con `Timer` |
| `ToggleSwitch.cs` | 100 | Interruptor con `Paint` propio |

### `Core/`, `Theme/`, `Storage/`

| Archivo | Líneas | Responsabilidad |
|---|---:|---|
| `Core/Models.cs` | 81 | `SeverityLevel`, `SeverityColor`, `LoginFailure`, modelos de datos |
| `Core/TriageEngine.cs` | 90 | Motor de triaje: síntomas → severidad (`SymptomTriage`) |
| `Core/AuthService.cs` | 79 | Registro e inicio de sesión local |
| `Core/AlarmService.cs` | 24 | Programación de alarmas de medicación y consulta |
| `Theme/AppTheme.cs` | 66 | Paleta de colores, fuentes y `ThemeChanged` |
| `Theme/DrawingHelpers.cs` | 136 | Helpers GDI+ (gradientes, texto, esquinas) |
| `Storage/AppStorage.cs` | 145 | `IJsonStore`, `JsonFileStore`, `AppStorage.CreateDefault()` |
| `Storage/DemoData.cs` | 79 | Datos de ejemplo para la primera ejecución |

### Modales definidos junto a las páginas

`ModalForm` y sus derivados —`PetModalForm`, `AlertModal`, `ActiveAlarmModal`,
`HistoryDetailModal`, `UpcomingModal`— se declaran en los archivos de las páginas
que los usan. Además, `AlertCard`, `HistoryCard`, `MedicalDisclaimer`,
`UpcomingCard`, `SeverityHero` y `ProgressBarSkin` / `ProgressIndicator` son
controles de GDI+ reutilizables.

---

## 6. Controles personalizados

Resumen de la jerarquía y para qué sirve cada control propio:

```
Control / Panel / Label / Button          (librería WinForms)
 └── RoundedPanel      esquinas redondeadas → base de PetCard, AlertCard, HistoryCard…
      └── PetCard      ficha de mascota en la lista
 └── GradientPanel     fondo con degradado
 └── ModalForm         ventana modal base
      └── PetModalForm / AlertModal / ActiveAlarmModal / HistoryDetailModal / UpcomingModal
 └── UserControl
      ├── PageBase     base de las 10 páginas
      │    └── LoginPage, RegisterPage, …, SettingsPage
      ├── RoundedButton / RoundedTextBox / RoundedComboBox / ToggleSwitch
      ├── NavItem / PillButton / CategoryChip / SymptomChip
      ├── SidebarControl, CenteredHost, FeatureCard, SeverityHero
      └── Toast
 └── SeverityBadge, HeroArt, PulsingIconBox, ProgressBarSkin, ProgressIndicator
```

---

## 7. Abrir y usar Visual Studio 2026

1. **Clonar el repositorio**

   ```bash
   git clone https://github.com/EduarJIM/Diagnostical-Pets.git
   cd Diagnostical-Pets/DiagnosticaTuMascota
   ```

2. **Abrir la solución**

   ```
   DiagnosticaTuMascota.sln
   ```

   ⚠️ Abre el archivo `.sln` de esta carpeta, no la carpeta raíz (que contiene la
   versión web).

3. **Estructura esperada**

   ```
   DiagnosticaTuMascota/
   ├── DiagnosticaTuMascota.sln
   ├── COMO-FUNCIONA.md          ← este documento
   ├── README.md
   ├── src/DiagnosticaTuMascota/ ← proyecto WinForms
   │   ├── Program.cs
   │   ├── MainForm.cs
   │   ├── DesignTime.cs
   │   ├── Pages/
   │   ├── Controls/
   │   ├── Core/
   │   ├── Theme/
   │   └── Storage/
   └── tests/DiagnosticaTuMascota.Tests/
   ```

4. **Ejecutar:** `F5` (o *Depurar → Iniciar*). Requiere Windows con .NET 10 Desktop Runtime.

5. **Usar el Diseñador:** clic derecho sobre cualquier archivo de `Pages/` →
   **Ver → Diseñador**. Ahí se pueden seleccionar, arrastrar y redimensionar
   controles con el mouse.

   Si el Diseñador muestra una página vacía: clic derecho sobre el proyecto →
   **Recompilar**, y vuelve a abrirlo.

6. **Publicar un `.exe`**

   ```bash
   dotnet publish src/DiagnosticaTuMascota -c Release -r win-x64 --self-contained true
   ```

   El ejecutable queda en `src/DiagnosticaTuMascota/bin/Release/net10.0-windows/win-x64/publish/`.

### Notas de compilación

- El proyecto tiene `EnableWindowsTargeting=true`, por lo que **compila también desde
  Linux o macOS**; solo la ejecución y el Diseñador requieren Windows.
- `.csproj` define `AppTargetFramework` para poder cambiar la versión de .NET desde
  una sola línea.

---

## 8. Dónde aprender más

- **Tutorial oficial:** [Introducción a Windows Forms](https://learn.microsoft.com/dotnet/desktop/winforms/)
- **Referencia de la API:** [System.Windows.Forms](https://learn.microsoft.com/dotnet/api/system.windows.forms)
- **Controles:** [Windows Forms Controls](https://learn.microsoft.com/dotnet/desktop/winforms/controls/)
- **GDI+:** [Graphics API](https://learn.microsoft.com/dotnet/api/system.drawing.graphics)
- **Diseñador:** [Designing a Windows Forms Application with Visual Studio](https://learn.microsoft.com/visualstudio/winforms/)
