# Documentación Técnica del Proyecto
## Fiesta Fantasmal en Casa Embrujada

### Descripción General
Proyecto desarrollado en Unity basado en exploración, interacción con objetos, combate contra fantasmas, gestión de inventario, enfrentamiento contra un jefe final DJ Fantasma y un minijuego DDR.

---

# Arquitectura General

El proyecto está dividido en los siguientes módulos:

1. Sistema de Jugador
2. Sistema de Interacción
3. Sistema de Inventario
4. Sistema de Combate
5. Sistema de Jefe Final
6. Sistema DDR
7. Interfaz de Usuario
8. Gestión Global del Juego
9. Nivel 2
10. Nivel 3 y Secuencia Final

---

# Sistema de Jugador

## PlayerController.cs

Controla:

- Movimiento del personaje.
- Rotación.
- Interacción con el entorno.
- Navegación entre niveles.

Responsabilidades:

- Capturar entradas del usuario.
- Actualizar posición.
- Comunicar acciones al resto de sistemas.

---

# Sistema de Interacción

## IInteractable.cs

Interfaz común para todos los objetos interactuables.

Permite que distintos objetos respondan a una misma acción de interacción.

Método principal:

```csharp
Interact(InventorySystem inventory)
```

---

## InteractionProximityDetector.cs

Detecta objetos cercanos al jugador mediante colliders tipo Trigger.

Funciones:

- Detectar entrada y salida de objetos interactuables.
- Permitir interacción con tecla E.
- Comunicarse con objetos que implementen IInteractable.

---

## InteractableExample.cs

Ejemplo base para crear nuevos objetos interactuables.

---

# Sistema de Inventario

## ItemDefinition.cs

ScriptableObject utilizado para definir objetos.

Información almacenada:

- ID del objeto.
- Nombre.
- Stackable.
- Cantidad máxima.

---

## InventorySystem.cs

Gestiona el inventario completo.

Funciones:

- Agregar objetos.
- Consumir objetos.
- Consultar cantidades.
- Crear snapshots.
- Restaurar snapshots.

Métodos principales:

```csharp
AddItem()
ConsumeItem()
HasItem()
LoadSnapshot()
CreateSnapshot()
```

---

## PickupItem.cs

Permite recoger objetos del escenario.

Flujo:

1. Jugador interactúa.
2. Se agrega al inventario.
3. El objeto se destruye.

---

## BottleThrower.cs

Permite lanzar botellas.

Proceso:

1. Verifica inventario.
2. Consume una botella.
3. Instancia un proyectil.
4. Aplica fuerza física.

---

## ThrowAimIndicator.cs

Muestra visualmente el punto de impacto.

Funciones:

- Apuntado.
- Raycasting.
- Visualización del objetivo.

---

## BottleUI.cs

Representación visual de las botellas disponibles.

---

# Sistema de Combate

## Health.cs

Sistema genérico de salud.

Funciones:

- Aplicar daño.
- Curar.
- Muerte.
- Eventos de actualización.

Variables principales:

```csharp
maxHealth
currentHealth
```

Métodos:

```csharp
ApplyDamage()
Heal()
Deplete()
```

---

## BottleProjectile.cs

Define las propiedades ofensivas de una botella.

Incluye:

- Daño.
- Tiempo de vida.
- Destrucción al impactar.

---

## GhostBottleDamageReceiver.cs

Permite que enemigos reciban daño de botellas.

Detecta:

```csharp
OnTriggerEnter()
OnCollisionEnter()
```

---

## PlayerWaveDamageReceiver.cs

Gestiona el daño recibido por el jugador proveniente de ondas musicales del jefe.

---

# Sistema del Jefe Final

## DJGhostBossAttack.cs

Controla el patrón ofensivo del jefe.

Ataques:

### Regular Wave

Ataque estándar.

Características:

- Daño directo.
- Frecuencia alta.

### Special Wave

Ataque especial.

Características:

- Activa el minijuego DDR.
- Mayor velocidad.
- Mayor dificultad.

---

## MusicWaveProjectile.cs

Controla el comportamiento de las ondas musicales.

Funciones:

- Movimiento.
- Seguimiento del jugador.
- Destrucción automática.

---

## DJGhostFloatMotion.cs

Genera movimiento flotante del DJ Fantasma.

Objetivo:

- Simular levitación.
- Dar sensación sobrenatural.

---

## NV3FinaleSequence.cs

Gestiona la secuencia final del nivel.

Incluye:

- Eventos finales.
- Transiciones.
- Condiciones de victoria.

---

# Sistema DDR

## Objetivo

Cuando el jugador es golpeado por una onda especial se carga la escena DDR.

Debe completar correctamente una secuencia de pasos musicales.

---

## DDRManager.cs

Controlador principal.

Responsabilidades:

- Cargar canciones.
- Seleccionar canción aleatoria.
- Reproducir audio.
- Gestionar pasos.
- Calcular aciertos.
- Calcular errores.
- Retornar a la escena principal.

Variables importantes:

```csharp
stepDurationSeconds
previewDurationSeconds
inputDurationSeconds
```

---

## DDRSongData.cs

Estructura de datos para canciones.

Contiene:

- BPM.
- Nombre.
- Archivo de audio.
- Secuencia de pasos.

---

## DDRArrowCanvasUI.cs

Interfaz visual del DDR.

Funciones:

- Mostrar flechas.
- Animar desplazamiento.
- Sincronizar con DDRManager.

---

# Gestión Global

## GameManager.cs

Singleton persistente.

Funciones:

### Persistencia

Conserva entre escenas:

- Vida del jugador.
- Vida del jefe.
- Inventario.
- Party-O-Meter.

### DDR

Gestiona:

- Entrada al DDR.
- Regreso desde DDR.
- Penalizaciones.

### Estado global

Mantiene información compartida entre escenas.

---

## MusicManager.cs

Administrador global de música.

Responsabilidades:

- Mantener audio entre escenas.
- Controlar volumen.
- Evitar duplicados.

---

# Interfaz de Usuario

## PlayerHealthBar.cs

Barra de salud del jugador.

---

## PartyometerController.cs

Control visual del Party-O-Meter.

---

## PermanentHUDManager.cs

HUD persistente.

Muestra:

- Vida.
- Inventario.
- Party-O-Meter.

---

## SceneFader.cs

Transiciones visuales entre escenas.

---

# Nivel 2

## Level2Manager.cs

Controlador principal del segundo nivel.

Gestiona:

- Objetivos.
- Estado de progreso.
- Eventos.

---

## Sistema de Fantasmas

### GhostPatrol.cs

Patrullaje de enemigos.

### GhostDamage.cs

Daño provocado por fantasmas.

---

## Sistema de Luces

### lamp.cs

Representación individual de lámparas.

### LightControl.cs

Gestión global de iluminación.

---

## Sistema de Parlantes

### speaker.cs

Representación de parlantes.

### SpeakerControl.cs

Control de activación y desactivación.

Permite afectar la música del nivel.

---

## Sistema de Mensajes

### ghostMessages.cs

Mensajes contextuales de fantasmas.

### InteractiveUI.cs

Interfaz de interacción.

---

# Puzzles

## MesitaContraseña.cs

Puzzle basado en contraseña.

---

## pistaCuadro.cs

Obtención de pista mediante cuadro.

---

## pistaLibro.cs

Obtención de pista mediante libro.

---

## pistaMaceta.cs

Obtención de pista mediante maceta.

---

## PuertaBloqueada.cs

Control de acceso mediante llave o contraseña.

---

# Menús

## principalMenu.cs

Menú principal.

Funciones:

- Iniciar partida.
- Salir del juego.
- Navegación básica.

---

## IntroController.cs

Gestiona introducciones y secuencias iniciales.

---

## CreditsRoll.cs

Controla la pantalla de créditos.

---

# Flujo General del Juego

1. El jugador explora la casa.
2. Interactúa con objetos y puzzles.
3. Obtiene recursos.
4. Recoge botellas.
5. Combate enemigos.
6. Reduce el Party-O-Meter.
7. Llega al DJ Fantasma.
8. Esquiva ondas normales.
9. Completa desafíos DDR cuando recibe ondas especiales.
10. Derrota al jefe.
11. Ejecuta la secuencia final.
12. Muestra créditos.

---

# Tecnologías Utilizadas

- Unity Engine
- C#
- Rigidbody Physics
- Colliders y Triggers
- ScriptableObjects
- AudioSource
- UI Canvas
- SceneManager
- Coroutines
- PlayerPrefs

---

# Conclusión

El proyecto implementa una arquitectura modular basada en componentes de Unity. Los sistemas de interacción, combate, inventario, persistencia y minijuegos están desacoplados mediante interfaces y gestores globales, permitiendo mantener el estado del jugador entre escenas y facilitar la expansión futura del videojuego.
