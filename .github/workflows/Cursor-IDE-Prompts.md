# 🎯 Cursor IDE Prompts - Event Horizon Tower Defense Conversion
## Optimized Prompts para Generación Rápida de Código

**[STACK: C# | Unity | Cursor IDE | Fast Development]**

---

## 📌 Cómo Usar Estos Prompts

1. Abre Cursor IDE en tu proyecto Event Horizon
2. Copia el prompt completo (incluyendo código de ejemplo)
3. Pega en la ventana de Cursor Chat
4. Cursor generará el código completo y listo para usar
5. Pega el resultado en el archivo correspondiente

---

## Prompt 1️⃣: Refactorizar Enemy a Tower Defense

**Archivo Destino**: `Assets/Scripts/TowerDefense/Enemy/EnemyUnit.cs`

```
I need to refactor an existing Enemy class to work with a Tower Defense pathfinding system.

Current situation:
- Project: Unity game with existing arcade enemies
- Enemies currently move freely with velocity
- Need to: Make enemies follow a predefined path
- Path follows waypoints with progress 0-1

Requirements:
1. Create abstract EnemyUnit base class
2. Add Path tracking system (SetPath method)
3. Convert velocity-based movement to path-based
4. Add pathProgress variable (0 to 1)
5. Call OnReachedEnd() when path completes
6. Preserve existing health/damage systems
7. Keep sprite rendering and animations

Code structure:
- Should inherit existing Enemy damage system
- Must work with Path class that has waypoints array
- Speed should be adjustable in inspector
- Add gizmo visualization for debug

Constraints:
- Must NOT break existing enemy prefabs
- Must be compatible with Wave spawning system
- Should handle null path gracefully

Please generate the complete EnemyUnit class with:
- Constructor and initialization
- Path following logic in Update()
- Method to calculate position from progress
- Event when reaching path end
- Debug visualization
```

---

## Prompt 2️⃣: Sistema Completo de Torres

**Archivo Destino**: `Assets/Scripts/TowerDefense/Towers/`

```
Create a complete tower defense tower system with inheritance.

Requirements:
1. Abstract Tower base class with core functionality
2. Three concrete tower types:
   - LaserTower (instant hit, line renderer effect)
   - ProjectileTower (spawn projectiles, travel time)
   - ExplosiveTower (AoE damage, splash effect)
3. Targeting system (nearest, weakest hp, highest threat)
4. Upgrade mechanism (5 levels max, 1.25x damage multiplier)
5. ScriptableObject for configuration

Tower base class needs:
- Range, fireRate, damage, cost properties
- Fire() and TryFire() methods
- Collider-based enemy detection
- Range visualization (circle gizmo)
- Upgrade functionality
- Material/color change on upgrade

Specific tower features:
- LaserTower: LineRenderer beam, instant damage
- ProjectileTower: Spawn projectile prefab, add velocity
- ExplosiveTower: Damage all enemies in radius, particle effect

Configuration:
- Use ScriptableObject for tower stats
- Allow customization of targeting priority
- Include cost, damage, range, fire rate

Architecture:
- Use event system for tower placement feedback
- Support tower destruction/selling
- Implement tower highlight on selection

Constraints:
- Must work with Enemy unit detection system
- Compatible with grid placement system
- Support tower pooling for performance

Please generate complete C# code for:
- Tower abstract base class
- Three concrete tower implementations
- TargetingSystem enum/class
- TowerConfig ScriptableObject
- ITargetingStrategy pattern for flexible targeting

Include:
- Full method implementations
- Proper error handling
- Comments explaining logic
- Example usage in Start/Update
```

---

## Prompt 3️⃣: Wave Manager Completo

**Archivo Destino**: `Assets/Scripts/TowerDefense/WaveSystem/WaveManager.cs`

```
Create a complete Wave management system for tower defense.

System requirements:
1. Manage sequential waves of enemy spawning
2. Configurable waves via ScriptableObject
3. Difficulty progression (+15% health, +10% speed per wave)
4. Spawn delay between enemies
5. Wave completion detection (all enemies dead)
6. Game over condition (base health = 0)

Wave data structure:
- Wave number
- Enemy spawn configurations (type, quantity, spacing)
- Delay between enemy spawns
- Difficulty multipliers

WaveManager functionality:
1. StartNextWave() - Initialize wave spawning
2. IsWaveComplete check (all enemies dead)
3. Auto-progression to next wave
4. Pause/resume support
5. Event system:
   - OnWaveStarted(waveNumber)
   - OnWaveCompleted()
   - OnGameOver()
   - OnEnemySpawned()

Features:
- Difficulty scales with wave number
- Support for multiple enemy types
- Spawn point variation
- Countdown between waves
- Progress tracking (current wave, total enemies)

Architecture:
- Singleton pattern for easy access
- Use Coroutines for spawning timing
- Event-based communication
- Compatible with RewardSystem

Constraints:
- Must handle wave configuration via SO
- Support 20+ waves
- No memory leaks on enemy destruction
- Clean up coroutines properly

Please generate:
- WaveManager class (Singleton)
- Wave and EnemySpawnData classes
- DifficultyScaler helper class
- Complete spawn logic with coroutines
- Event system integration
- Save/load wave progress (optional)

Include:
- Full implementation with error checks
- Comments for complex logic
- Debug logging for tracking
- Inspector-friendly serialization
```

---

## Prompt 4️⃣: Tower Placement System

**Archivo Destino**: `Assets/Scripts/TowerDefense/TowerPlacement/TowerPlacementSystem.cs`

```
Create a tower placement system with grid snapping and validation.

Requirements:
1. Grid-based tower placement (snap to grid)
2. Preview tower with valid/invalid states
3. Collision detection to prevent overlaps
4. Resource consumption validation
5. Click-to-place interaction

Placement workflow:
1. Player selects tower type from UI
2. Preview appears following mouse (semi-transparent)
3. Preview color changes:
   - Green = valid placement (green material)
   - Red = invalid placement (red material)
4. Click to place if valid
5. Consume resources and spawn tower

Validation checks:
- No tower already at position
- Resources available
- Not overlapping obstacles
- Within map bounds
- Proper grid alignment

Features:
1. SelectTowerType(Tower prefab) method
2. Grid snapping with configurable size
3. Real-time preview following mouse
4. Visual feedback (valid/invalid colors)
5. Sound feedback on placement
6. Resource deduction
7. Tower storage/tracking

Preview system:
- Semi-transparent tower model
- Range circle visualization
- Cost display near cursor
- Placement prediction

Architecture:
- Use Input.GetMouseButtonDown(0) for placement
- Camera.ScreenToWorldPoint for mouse position
- Collider-based overlap detection
- Material swapping for preview states
- Event callbacks for UI updates

Resource integration:
- Check Resources.credits
- OnTowerPlaced event with cost
- Update UI resource display

Constraints:
- Preview must not have colliders
- Must support multiple tower types
- Grid size configurable
- Support tower drag-to-preview

Please generate:
- TowerPlacementSystem class
- SnapToGrid helper method
- CanPlaceAt validation method
- UpdatePreview with visual feedback
- Mouse tracking logic
- Integration with Resources system

Include:
- Complete implementation
- Grid visualization (gizmos)
- Null checking and error handling
- Comments explaining validation
- Public API for UI integration
```

---

## Prompt 5️⃣: Base Station (Defensa de Base)

**Archivo Destino**: `Assets/Scripts/TowerDefense/BaseDefense/BaseStation.cs`

```
Create a base station system that can take damage and trigger game over.

BaseStation requirements:
1. Health system (maxHealth configurable)
2. Take damage when enemies reach it
3. Visual health representation
4. Game over trigger at 0 health
5. Event system for UI updates

Health system:
- maxHealth property (default 100)
- currentHealth tracking
- TakeDamage(amount) method
- Heal(amount) method
- GetHealthPercentage() return 0-1

Events:
- OnHealthChanged(float percentage)
- OnHealthCritical(below 25%)
- OnDestroyed()
- OnGameOver()

Features:
1. Damage animation (shake, color flash)
2. Health bar visualization
3. Critical health warning (SFX/VFX)
4. Invulnerability frames option
5. Damage reduction armor system

Visualization:
- Sprite renderer with health visualization
- Particle effect on damage
- Screen shake on heavy damage
- Color lerp from green → red based on health

Integration:
- Work with WaveManager for game over
- Connect to HUD for health display
- Support damage numbers floating
- Reset health on new level

Architecture:
- Standalone component
- Pure data (no UI coupling)
- Event-driven for other systems
- Support for multiple bases (future)

Constraints:
- Must prevent damage > 0 on 0 health
- Should log damage taken
- Support serialization
- Inspector-friendly

Please generate:
- BaseStation MonoBehaviour class
- Health property with change detection
- TakeDamage with optional damage reduction
- Damage animation system
- Event invocation at key thresholds
- Debug visualization

Include:
- Full implementation with safety checks
- Comments explaining state changes
- Inspector properties for tuning
- Example usage in other systems
```

---

## Prompt 6️⃣: HUD & UI System

**Archivo Destino**: `Assets/Scripts/Gui/TowerDefenseHUD.cs`

```
Create a tower defense HUD that displays game information.

HUD elements needed:
1. Wave counter (current/total waves)
2. Credits/resources display
3. Base health bar + percentage text
4. Tower selection panel
5. Game over/win screen
6. Pause button
7. Speed controls (1x, 2x, 4x)

Layout structure:
- Top left: Wave info
- Top right: Credits counter
- Center: Main game view
- Left side: Tower selection buttons
- Center top: Base health bar
- Right side: Game over panel (hidden normally)

Features:
1. Real-time stat updates
2. Tower button shows cost and icon
3. Highlight selected tower
4. Disable unavailable towers (insufficient credits)
5. Pause functionality (hide game, show menu)
6. Game over/win states with restart button
7. Speed multiplier buttons

Events to subscribe:
- WaveManager.OnWaveStarted
- WaveManager.OnGameOver
- Resources.OnCreditsChanged
- BaseStation.OnHealthChanged
- TowerPlacementSystem placements

Tower selection panel:
- Show all available towers
- Display tower icon
- Show cost
- Disable if not enough credits
- On click: SelectTowerType in placement system

Health display:
- Slider/bar visualization
- Percentage text
- Color change (green → yellow → red)
- Damage popup numbers (optional)

Game over panel:
- Victory or defeat message
- Wave reached count
- Total enemies defeated
- Resources accumulated
- Restart button
- Main menu button

Pause system:
- Pause button in top-left
- Time.timeScale = 0
- Show pause menu overlay
- Resume button
- Return to menu

Architecture:
- Single HUD canvas
- Separate panels for sections
- Event subscriptions in OnEnable
- Unsubscribe in OnDisable
- Clean MVC pattern

Constraints:
- Must NOT crash if references null
- Support different screen resolutions
- Responsive UI layout
- Performance: minimize updates

Please generate:
- TowerDefenseHUD class
- TowerSelectionPanel class
- HealthDisplay helper
- GameOverPanel helper
- Method to update each element
- Event subscription system
- Pause/resume logic

Include:
- Complete UI controller code
- Comments for each section
- Null safety checks
- Clear public API
- Example serialization setup
```

---

## Prompt 7️⃣: Reward & Economy System

**Archivo Destino**: `Assets/Scripts/TowerDefense/Economy/RewardSystem.cs`

```
Create reward system for tower defense with kill bonuses and wave completion.

Reward mechanics:
1. Kill reward per enemy (configurable per type)
2. Wave completion bonus
3. Difficulty bonus (extra credits for harder waves)
4. Combo system (bonus for consecutive kills)
5. Base survival reward (wave end bonus)

Features:
1. Kill rewards:
   - Fighter = 10 credits
   - Corvette = 25 credits
   - Destroyer = 50 credits
   - Boss = 200 credits

2. Wave bonuses:
   - Base wave complete = 50 credits
   - Difficulty multiplier (1.1x per wave)
   - Speed bonus (rewards for fast completion)
   - No damage taken bonus

3. Combo system:
   - Track consecutive kills
   - Multiplier: 1 + (combo_count * 0.1x) up to 3x
   - Reset on wave end

4. UI feedback:
   - Floating text for kill rewards
   - Achievement notifications
   - Total bonus summary

Integration:
- Connect to Enemy death events
- Listen to WaveManager.OnWaveCompleted
- Add credits via Resources.Earn()
- Event callbacks for UI

Architecture:
- RewardSystem component (singleton)
- RewardConfig ScriptableObject
- Floating text prefab
- Event system for UI

Data tracking:
- Credits earned this wave
- Kills this combo
- Wave completion bonus pool
- Total session credits

Please generate:
- RewardSystem class
- RewardConfig ScriptableObject
- Methods for OnEnemyKilled
- Methods for OnWaveCompleted
- Combo tracking system
- Bonus calculation methods
- Floating text spawning

Include:
- Full implementation
- Kill reward lookup (by enemy type)
- Difficulty scaling calculations
- Combo system with reset
- Comments explaining bonuses
- Debug logging for rewards
```

---

## Prompt 8️⃣: Projectile & Object Pooling

**Archivo Destino**: `Assets/Scripts/TowerDefense/Combat/ProjectilePool.cs`

```
Create projectile system with object pooling to prevent GC stutters.

Pooling requirements:
1. Reusable projectile pool
2. Get() returns projectile (create if needed)
3. Return() puts back in pool
4. Grow dynamically if pool exhausted
5. Optional: Separate pools per tower type

Projectile requirements:
1. Initialize with target and damage
2. Travel toward target
3. Detect collision
4. Deal damage on hit
5. Return to pool instead of destroy

Features:
1. Configurable pool size (50 projectiles)
2. Grow by N more if exhausted
3. Visual update (position, rotation)
4. Smooth movement interpolation
5. Damage on collision only
6. Optional: trail renderer effect
7. Optional: particle on hit

Projectile logic:
1. Enabled = false when pooled
2. Initialize(target, damage) to activate
3. Update: move toward target
4. CheckCollision: if distance < 0.5
5. OnHit: damage target, return to pool
6. Timeout: return if target dies

Optimization:
- No Instantiate/Destroy calls per shot
- Minimal Update() work
- Reuse Rigidbody/Collider
- Optional: Multiple pools per tower type

Architecture:
- ProjectilePool Singleton
- Projectile interface/base
- Specific projectile types inherit
- Pool manager handles recycling
- Event on projectile hit

Integration:
- ProjectileTower calls pool.Get()
- Passes target and damage
- Projectile handles movement
- Pool returns on hit

Please generate:
- ProjectilePool singleton
- Projectile base class
- Specific projectile types
- Get() and Return() methods
- Movement and collision logic
- Pool growth mechanism
- Debug utilities

Include:
- Full implementation
- Safe null checking
- Comments for pooling logic
- Configurable pool parameters
- Performance optimizations
- Example tower integration
```

---

## 🚀 Recomendación de Orden de Implementación

1. **Prompt 1** → Enemy refactoring (foundation)
2. **Prompt 3** → Path system (foundation)
3. **Prompt 2** → Towers (core mechanic)
4. **Prompt 4** → Placement (player interaction)
5. **Prompt 5** → Base station (win condition)
6. **Prompt 3** → Wave manager (progression)
7. **Prompt 6** → HUD (feedback)
8. **Prompt 7** → Rewards (fun)
9. **Prompt 8** → Pooling (optimization)

---

## 💡 Tips para Usar Cursor Efectivamente

1. **Sé específico**: Incluye constraints y ejemplos
2. **Código de referencia**: Incluye interfaces esperadas
3. **Error handling**: Pide checks de null y validación
4. **Documentación**: Pide comentarios en código
5. **Testing**: Pide métodos Debug/Gizmos
6. **Integración**: Especifica qué otros sistemas van a usar este

---

## ✅ Post-Generación Checklist

Después que Cursor genere código:

- [ ] Copiar a archivo correcto en proyecto
- [ ] Revisar imports (using statements)
- [ ] Verificar sintaxis C#
- [ ] Crear SO para configuración (si aplica)
- [ ] Agregar a scene/prefab
- [ ] Asignar referencias en inspector
- [ ] Prueba rápida en play mode
- [ ] Commit a git con mensaje descriptivo

---

**Ready to start?** Pick a prompt and paste into Cursor IDE! 🚀
