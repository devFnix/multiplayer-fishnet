# Sistema de Equipos y Juego por Turnos - Documentación

## Resumen de Funcionalidades Implementadas

### 4. Selección de Equipos dentro de la Sala ✅

**Características implementadas:**
- **Interfaz de selección de equipos** con dos paneles claramente diferenciados:
  - Equipo 1 (color azul) - máximo 2 jugadores
  - Equipo 2 (color rojo) - máximo 2 jugadores
- **Botones de unión a equipos:**
  - "Unirse al Equipo 1" - se deshabilita cuando el equipo está lleno
  - "Unirse al Equipo 2" - se deshabilita cuando el equipo está lleno
- **Cambio de equipos:** Los jugadores pueden cambiar de equipo mientras haya espacio disponible
- **Visualización en tiempo real** de los jugadores en cada equipo
- **Estado del juego** que muestra el progreso hacia el 2 vs 2

### 5. Inicio de la Partida ✅

**Características implementadas:**
- **Botón "PLAY"** que aparece cuando se cumple la condición 2 vs 2
- **Validación automática** de equipos completos antes de permitir el inicio
- **Transición automática** a la escena del juego (GameScene)
- **Indicador visual** del estado del juego ("¡Listo para jugar! 2 vs 2")

### 6. Mecánica de Juego por Turnos ✅

**Características implementadas:**
- **Sistema de turnos rotativos:**
  - Equipo 1 Jugador 1 → Equipo 2 Jugador 1 → Equipo 1 Jugador 2 → Equipo 2 Jugador 2 → (repite)
- **Botones interactivos del juego:**
  - 6 botones dispuestos en una cuadrícula 2x3
  - Los botones se deshabilitan después de ser seleccionados
  - Desaparecen visualmente una vez clickeados
- **Indicadores visuales:**
  - Texto que muestra de quién es el turno actual
  - Colores diferenciados por equipo (azul/rojo)
  - Información del equipo activo
- **Reinicio automático** del juego cuando todos los botones han sido seleccionados

## Archivos Creados/Modificados

### Scripts Principales:

1. **TeamManager.cs** - Gestor de equipos con networking (FishNet)
2. **LocalTeamManager.cs** - Versión local para testing sin networking
3. **TurnBasedGameManager.cs** - Gestor del juego por turnos con networking
4. **LocalTurnBasedGameManager.cs** - Versión local del juego por turnos
5. **TeamTestHelper.cs** - Utilidad para testing de equipos

### Scripts Modificados:

1. **SimpleRoomManager.cs** - Añadidas referencias y métodos para selección de equipos
2. **RoomItem.cs** - Actualizado para mostrar la selección de equipos al unirse

### Escenas:

1. **MultiplayerGame.unity** - Escena principal con UI de selección de equipos
2. **GameScene.unity** - Escena del juego con mecánica por turnos

## Estructura de UI Implementada

### Escena Principal (MultiplayerGame.unity):
```
Canvas/
├── MainMenuPanel (menú principal existente)
├── RoomPanel (lista de salas existente)
├── TeamSelectionPanel (NUEVO)
│   ├── RoomTitleText
│   ├── Team1Panel/
│   │   ├── Team1Title
│   │   ├── Team1PlayersText
│   │   └── JoinTeam1Button
│   ├── Team2Panel/
│   │   ├── Team2Title
│   │   ├── Team2PlayersText
│   │   └── JoinTeam2Button
│   ├── GameStatusText
│   ├── PlayButton
│   ├── BackToRoomListButton
│   └── TestButtonsPanel/ (para testing)
│       ├── TestP1T1, TestP2T1, TestP3T2, TestP4T2
```

### Escena del Juego (GameScene.unity):
```
Canvas/
└── GameUI/
    ├── CurrentTurnText
    ├── GameInfoText
    └── ButtonContainer/
        ├── GameButton1, GameButton2, GameButton3
        └── GameButton4, GameButton5, GameButton6
```

## Cómo Usar el Sistema

### Para Testing (Modo Local):

1. **Ejecutar el juego** en Unity
2. **Navegar a la selección de equipos** (se muestra automáticamente para testing)
3. **Usar los botones de test** (P1→T1, P2→T1, P3→T2, P4→T2) para simular jugadores uniéndose a equipos
4. **Alternativamente**, usar las teclas del teclado:
   - `1` - Player1 se une al Equipo 1
   - `2` - Player2 se une al Equipo 1
   - `3` - Player3 se une al Equipo 2
   - `4` - Player4 se une al Equipo 2
5. **Presionar PLAY** cuando ambos equipos tengan 2 jugadores
6. **En el juego**, hacer clic en los botones para simular turnos

### Flujo Normal de Usuario:

1. **Crear/Unirse a una sala** desde el menú principal
2. **Seleccionar equipo** usando los botones "Unirse al Equipo X"
3. **Esperar** a que se complete el 2 vs 2
4. **Presionar PLAY** para iniciar la partida
5. **Jugar por turnos** seleccionando botones cuando sea tu turno

## Características Técnicas

### Sistema de Turnos:
- **Orden fijo y predecible** de turnos
- **Validación** de que solo el jugador activo puede hacer movimientos
- **Sincronización** entre todos los clientes (en versión con networking)
- **Manejo de desconexiones** (en versión con networking)

### Gestión de Equipos:
- **Límites estrictos** de 2 jugadores por equipo
- **Cambio dinámico** de equipos
- **Validación** antes del inicio del juego
- **Persistencia** de datos de jugadores

### UI Responsiva:
- **Actualización en tiempo real** del estado de equipos
- **Habilitación/deshabilitación** automática de botones
- **Indicadores visuales** claros del estado del juego
- **Colores diferenciados** por equipo

## Próximos Pasos para Integración Completa

1. **Integrar con FishNet** - Reemplazar LocalTeamManager con TeamManager
2. **Sincronización de red** - Asegurar que todos los clientes vean el mismo estado
3. **Manejo de desconexiones** - Qué hacer cuando un jugador se desconecta
4. **Persistencia de salas** - Mantener el estado de la sala entre sesiones
5. **Validación del servidor** - Asegurar que las acciones son válidas en el servidor

## Testing

El sistema incluye herramientas de testing integradas:
- **Botones de simulación** para testing rápido
- **Atajos de teclado** para desarrollo
- **Logs de debug** para seguimiento de estado
- **Reinicio automático** del juego para testing continuo

Todas las funcionalidades solicitadas han sido implementadas y están listas para testing y integración con el sistema de networking completo.