# 🎮 Event Horizon → Event Horizon: Frontier
## Conversión de Juego Arcade a Tower Defense - Guía Completa

**[STACK: C# + Unity | Client-Side | Game Architecture | URP/Standard Rendering]**

---

## 📊 Análisis Comparativo: Juego Base vs Frontier

### Event Horizon (Actual)
- **Tipo**: Arcade Shooter / Space Dodger (estilo Flappy Bird cósmico)
- **Mecánica Principal**: Navegación del jugador evitando obstáculos
- **Control**: Input directo del jugador (movimiento)
- **Objetivo**: Sobrevivir, acumular puntos/dinero
- **Scope**: Juego de un solo escenario con progresión lineal

### Event Horizon: Frontier (Target)
- **Tipo**: Tower Defense / Base Defense
- **Mecánica Principal**: Construcción y gestión de torres defensivas
- **Control**: Pausable, estratégico (click to build/upgrade)
- **Objetivo**: Defender estación espacial contra oleadas de enemigos
- **Scope**: Múltiples misiones, progressión por niveles, sandbox elements

---

## 🏗️ Arquitectura de Transformación

### Fase 0: Preparación (Branches y Estructura)

```
Develop branch (actual)
├── Assets/Scripts/GameContent/    [MANTENER - módulos de juego]
├── Assets/Scripts/Combat/         [REFACTORIZAR - agregar tower defense]
├── Assets/Scripts/Domain/         [EXPANDIR - nuevos tipos de datos]
└── Assets/Scripts/GameServices/   [EXTENDER - sistemas de economía]

Nueva estructura Tower Defense:
├── Assets/Scripts/TowerDefense/
│   ├── Towers/                    [NEW] Torre base + tipos específicos
│   ├── WaveManager/               [NEW] Generador de oleadas
│   ├── BaseDefense/               [NEW] Sistema de defensa de base
│   ├── TowerPlacement/            [NEW] Grid placement system
│   └── PathFinding/               [NEW] Sistema de pathing enemigos
```

---

## 🔄 Transformación por Componentes

### 1️⃣ Refactorización: Sistema de Enemigos
**Contexto**: El juego ya tiene enemigos que se mueven. Necesitamos adaptarlos a un sistema de oleadas.

```csharp
// ANTES - Enemy individual con lógica propia
public class Enemy : MonoBehaviour
{
    public void Update() 
    {
        // Movimiento independiente estilo arcade
        transform.position += velocidad * Time.deltaTime;
    }
}

// DESPUÉS - Enemy con seguimiento de path
public class EnemyUnit : MonoBehaviour
{
    private Path path;
    private float pathProgress = 0f;
    private float speed = 2f;
    
    public void SetPath(Path newPath) => path = newPath;
    
    public void Update() 
    {
        if (path == null) return;
        
        pathProgress += (speed * Time.deltaTime) / path.Length;
        transform.position = path.GetPositionAtProgress(pathProgress);
        
        if (pathProgress >= 1f) 
        {
            OnReachedEnd();
            Destroy(gameObject);
        }
    }
}
```

**Acción 1**: Crear `Assets/Scripts/TowerDefense/Enemy/EnemyUnit.cs`
**Acción 2**: Refactorizar enemigos existentes para heredar de EnemyUnit
**Acción 3**: Agregar sistema de path (ver siguiente sección)

---

### 2️⃣ Nuevo Sistema: Pathfinding & Rutas Predefinidas
**Contexto**: En Tower Defense, los enemigos siguen un camino fijo, no se mueven aleatoriamente.

```csharp
// Assets/Scripts/TowerDefense/PathFinding/Path.cs
[System.Serializable]
public class Path
{
    public Vector3[] waypoints;
    public float totalLength;
    
    public Vector3 GetPositionAtProgress(float progress)
    {
        // progress: 0 a 1
        progress = Mathf.Clamp01(progress);
        
        float distanceToTravel = totalLength * progress;
        float currentDistance = 0f;
        
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(waypoints[i], waypoints[i + 1]);
            
            if (currentDistance + segmentLength >= distanceToTravel)
            {
                float localProgress = (distanceToTravel - currentDistance) / segmentLength;
                return Vector3.Lerp(waypoints[i], waypoints[i + 1], localProgress);
            }
            
            currentDistance += segmentLength;
        }
        
        return waypoints[waypoints.Length - 1];
    }
}

// Assets/Scripts/TowerDefense/PathFinding/PathVisualizer.cs
[ExecuteInEditMode]
public class PathVisualizer : MonoBehaviour
{
    public Path path;
    
    private void OnDrawGizmos()
    {
        if (path?.waypoints == null) return;
        
        Gizmos.color = Color.green;
        for (int i = 0; i < path.waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(path.waypoints[i], path.waypoints[i + 1]);
            Gizmos.DrawSphere(path.waypoints[i], 0.3f);
        }
    }
}
```

**Acción 4**: Crear sistema de paths con SO (ScriptableObject)
**Acción 5**: Editar escenas para incluir waypoints visuales
**Acción 6**: Validar pathfinding en editor

---

### 3️⃣ Nuevo Sistema: Towers (Torres)
**Contexto**: La pieza central del gameplay. Necesitamos sistema base de torres y tipos específicos.

```csharp
// Assets/Scripts/TowerDefense/Towers/Tower.cs
public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected float range = 5f;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected int cost = 100;
    
    protected float lastFireTime = 0f;
    protected Collider2D detectionArea;
    
    protected virtual void Start()
    {
        // Crear área de detección circular
        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = range;
        col.isTrigger = true;
    }
    
    protected virtual void Update()
    {
        if (Time.time - lastFireTime >= 1f / fireRate)
        {
            TryFire();
        }
    }
    
    protected virtual void TryFire()
    {
        // Búsqueda de enemigos en rango
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position, 
            range,
            LayerMask.GetMask("Enemy")
        );
        
        if (enemies.Length > 0)
        {
            EnemyUnit target = enemies[0].GetComponent<EnemyUnit>();
            if (target != null)
            {
                Fire(target);
                lastFireTime = Time.time;
            }
        }
    }
    
    protected abstract void Fire(EnemyUnit target);
    
    public virtual void Upgrade()
    {
        damage *= 1.25f;
        fireRate *= 1.1f;
    }
}

// Assets/Scripts/TowerDefense/Towers/LaserTower.cs
public class LaserTower : Tower
{
    private LineRenderer laser;
    
    protected override void Start()
    {
        base.Start();
        laser = gameObject.AddComponent<LineRenderer>();
        laser.material = new Material(Shader.Find("Sprites/Default"));
        laser.startColor = Color.cyan;
        laser.endColor = Color.cyan;
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
    }
    
    protected override void Fire(EnemyUnit target)
    {
        // Daño por rayo láser
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, target.transform.position);
        
        target.TakeDamage(damage);
        StartCoroutine(DisableLaserAfterDelay(0.1f));
    }
    
    private IEnumerator DisableLaserAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        laser.positionCount = 0;
    }
}

// Assets/Scripts/TowerDefense/Towers/ProjectileTower.cs
public class ProjectileTower : Tower
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    
    protected override void Fire(EnemyUnit target)
    {
        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );
        
        Projectile p = proj.GetComponent<Projectile>();
        p.Initialize(target, damage, speed: 10f);
    }
}

// Assets/Scripts/TowerDefense/Projectile.cs
public class Projectile : MonoBehaviour
{
    private EnemyUnit target;
    private float damage;
    private float speed = 10f;
    
    public void Initialize(EnemyUnit newTarget, float newDamage, float speed = 10f)
    {
        target = newTarget;
        damage = newDamage;
        this.speed = speed;
    }
    
    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < 0.5f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
```

**Acción 7**: Crear jerarquía de torres (Tower → LaserTower, ProjectileTower, etc)
**Acción 8**: Implementar sistema de targeting (nearest, weakest, strongest)
**Acción 9**: Agregar SO para configuración de torres

---

### 4️⃣ Nuevo Sistema: Torre Defensa de Base
**Contexto**: La estación espacial que el jugador defiende, con HP y recompensas si sobrevive.

```csharp
// Assets/Scripts/TowerDefense/BaseDefense/BaseStation.cs
public class BaseStation : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDestroyed;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        
        if (currentHealth <= 0)
        {
            OnDestroyed?.Invoke();
            WaveManager.Instance.OnGameOver();
        }
    }
    
    public float GetHealthPercentage() => currentHealth / maxHealth;
}
```

**Acción 10**: Crear GameObject de base con visualización
**Acción 11**: Conectar a sistema de UI (barra de HP)

---

### 5️⃣ Nuevo Sistema: Wave Manager (Oleadas)
**Contexto**: Controla la dificultad progresiva generando oleadas de enemigos.

```csharp
// Assets/Scripts/TowerDefense/WaveSystem/Wave.cs
[System.Serializable]
public class Wave
{
    public int waveNumber;
    public float delayBetweenEnemies = 1f;
    public List<EnemySpawnData> enemySpawns = new();
}

[System.Serializable]
public class EnemySpawnData
{
    public EnemyType type;
    public int quantity = 5;
}

public enum EnemyType { Fighter, Corvette, Destroyer }

// Assets/Scripts/TowerDefense/WaveSystem/WaveManager.cs
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    [SerializeField] private List<Wave> waves = new();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Dictionary<EnemyType, GameObject> enemyPrefabs = new();
    
    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private bool isWaveActive = false;
    
    public event System.Action<int> OnWaveStarted;
    public event System.Action OnWaveCompleted;
    public event System.Action OnGameOver;
    
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
    
    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            OnGameOver?.Invoke();
            return;
        }
        
        Wave wave = waves[currentWaveIndex];
        OnWaveStarted?.Invoke(currentWaveIndex + 1);
        
        StartCoroutine(SpawnWave(wave));
        currentWaveIndex++;
    }
    
    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;
        
        foreach (var spawnData in wave.enemySpawns)
        {
            for (int i = 0; i < spawnData.quantity; i++)
            {
                SpawnEnemy(spawnData.type);
                yield return new WaitForSeconds(wave.delayBetweenEnemies);
            }
        }
        
        // Esperar a que todos los enemigos mueran
        yield return new WaitUntil(() => FindObjectsOfType<EnemyUnit>().Length == 0);
        
        isWaveActive = false;
        OnWaveCompleted?.Invoke();
    }
    
    private void SpawnEnemy(EnemyType type)
    {
        GameObject prefab = enemyPrefabs[type];
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        
        EnemyUnit unit = enemy.GetComponent<EnemyUnit>();
        unit.SetPath(GetWavePath());
    }
    
    private Path GetWavePath()
    {
        // Retorna la ruta principal de invasión
        return GetComponent<Path>();
    }
}
```

**Acción 12**: Crear SO para configuración de oleadas
**Acción 13**: Implementar curva de dificultad
**Acción 14**: Agregar eventos para UI

---

### 6️⃣ Nuevo Sistema: Torre Placement (Colocación)
**Contexto**: Sistema de click para colocar torres en la grilla.

```csharp
// Assets/Scripts/TowerDefense/TowerPlacement/TowerPlacementSystem.cs
public class TowerPlacementSystem : MonoBehaviour
{
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private Material validPlacementMaterial;
    [SerializeField] private Material invalidPlacementMaterial;
    
    private Camera mainCamera;
    private GameObject previewTower;
    private Tower selectedTowerPrefab;
    private int currentResources = 500;
    
    public event System.Action<int> OnResourcesChanged;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (selectedTowerPrefab == null) return;
        
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPos = SnapToGrid(mousePos);
        
        UpdatePreview(gridPos);
        
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower(gridPos);
        }
    }
    
    public void SelectTowerType(Tower prefab)
    {
        selectedTowerPrefab = prefab;
        
        if (previewTower != null) Destroy(previewTower);
        previewTower = Instantiate(prefab.gameObject);
        previewTower.GetComponent<Collider2D>().enabled = false;
        
        // Hacer semi-transparente
        SpriteRenderer sr = previewTower.GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.5f);
    }
    
    private Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            0
        );
    }
    
    private void UpdatePreview(Vector3 position)
    {
        if (previewTower == null) return;
        
        previewTower.transform.position = position;
        
        bool canPlace = CanPlaceAt(position);
        previewTower.GetComponent<SpriteRenderer>().material = 
            canPlace ? validPlacementMaterial : invalidPlacementMaterial;
    }
    
    private bool CanPlaceAt(Vector3 position)
    {
        // Verificar colisiones
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, gridSize * 0.4f);
        
        foreach (var col in colliders)
        {
            if (col.CompareTag("Tower") || col.CompareTag("Obstacle"))
                return false;
        }
        
        // Verificar recursos
        if (currentResources < selectedTowerPrefab.GetComponent<Tower>().cost)
            return false;
        
        return true;
    }
    
    private void TryPlaceTower(Vector3 position)
    {
        if (!CanPlaceAt(position)) return;
        
        Tower towerPrefab = selectedTowerPrefab.GetComponent<Tower>();
        
        // Consumir recursos
        currentResources -= towerPrefab.cost;
        OnResourcesChanged?.Invoke(currentResources);
        
        // Instanciar torre
        GameObject tower = Instantiate(selectedTowerPrefab.gameObject, position, Quaternion.identity);
        tower.tag = "Tower";
        
        // Feedback visual/audio
        AudioManager.Play("tower_placed");
    }
    
    public void AddResources(int amount)
    {
        currentResources += amount;
        OnResourcesChanged?.Invoke(currentResources);
    }
}
```

**Acción 15**: Crear sistema de preview de colocación
**Acción 16**: Agregar validación de collisiones
**Acción 17**: Integrar con sistema de economía

---

### 7️⃣ Refactorización: Sistema de Economía
**Contexto**: El juego ya tiene dinero. Adaptarlo para Tower Defense.

```csharp
// Assets/Scripts/Domain/Economy/Resources.cs (EXTENDER)
public class Resources : MonoBehaviour
{
    public int credits { get; private set; } = 500;
    
    public event System.Action<int> OnCreditsChanged;
    
    public bool TrySpend(int amount)
    {
        if (credits < amount) return false;
        
        credits -= amount;
        OnCreditsChanged?.Invoke(credits);
        return true;
    }
    
    public void Earn(int amount)
    {
        credits += amount;
        OnCreditsChanged?.Invoke(credits);
    }
}

// Assets/Scripts/TowerDefense/Economy/RewardSystem.cs (NEW)
public class RewardSystem : MonoBehaviour
{
    [SerializeField] private int killReward = 10;
    [SerializeField] private int waveCompleteBonus = 50;
    
    private Resources resourceManager;
    
    private void Start()
    {
        resourceManager = GetComponent<Resources>();
        WaveManager.Instance.OnWaveCompleted += GrantWaveBonus;
    }
    
    private void GrantWaveBonus()
    {
        resourceManager.Earn(waveCompleteBonus);
    }
    
    public void OnEnemyKilled(EnemyUnit enemy)
    {
        resourceManager.Earn(killReward);
    }
}
```

**Acción 18**: Extender sistema de economía existente
**Acción 19**: Conectar recompensas de kills a base de datos

---

### 8️⃣ Refactorización: UI/HUD
**Contexto**: Adaptar UI actual para tower defense (créditos, oleadas, salud base).

```csharp
// Assets/Scripts/Gui/TowerDefenseHUD.cs (NEW)
public class TowerDefenseHUD : MonoBehaviour
{
    [SerializeField] private Text waveText;
    [SerializeField] private Text creditsText;
    [SerializeField] private Slider baseHealthSlider;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button pauseButton;
    
    [SerializeField] private TowerSelectionPanel towerPanel;
    
    private BaseStation baseStation;
    private WaveManager waveManager;
    private Resources resources;
    
    private void Start()
    {
        baseStation = FindObjectOfType<BaseStation>();
        waveManager = WaveManager.Instance;
        resources = FindObjectOfType<Resources>();
        
        // Suscripciones
        baseStation.OnHealthChanged += UpdateBaseHealth;
        waveManager.OnWaveStarted += UpdateWaveDisplay;
        resources.OnCreditsChanged += UpdateCreditsDisplay;
        
        pauseButton.onClick.AddListener(TogglePause);
    }
    
    private void UpdateWaveDisplay(int waveNumber)
    {
        waveText.text = $"Wave {waveNumber}";
    }
    
    private void UpdateCreditsDisplay(int credits)
    {
        creditsText.text = $"Credits: {credits}";
    }
    
    private void UpdateBaseHealth(float healthPercent)
    {
        baseHealthSlider.value = healthPercent;
    }
    
    private void TogglePause()
    {
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
    }
}

// Assets/Scripts/Gui/TowerSelectionPanel.cs (NEW)
public class TowerSelectionPanel : MonoBehaviour
{
    [SerializeField] private GameObject towerButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    
    [SerializeField] private List<Tower> availableTowers = new();
    
    private TowerPlacementSystem placementSystem;
    
    private void Start()
    {
        placementSystem = FindObjectOfType<TowerPlacementSystem>();
        
        foreach (var tower in availableTowers)
        {
            CreateTowerButton(tower);
        }
    }
    
    private void CreateTowerButton(Tower towerPrefab)
    {
        GameObject btn = Instantiate(towerButtonPrefab, buttonContainer);
        Button buttonComponent = btn.GetComponent<Button>();
        Image icon = btn.GetComponent<Image>();
        Text costText = btn.GetComponentInChildren<Text>();
        
        icon.sprite = towerPrefab.GetComponent<SpriteRenderer>().sprite;
        costText.text = $"${towerPrefab.cost}";
        
        buttonComponent.onClick.AddListener(() => 
        {
            placementSystem.SelectTowerType(towerPrefab);
        });
    }
}
```

**Acción 20**: Rediseñar HUD para tower defense
**Acción 21**: Agregar panel de selección de torres
**Acción 22**: Implementar sistema de pausa

---

## 📋 Checklist de Implementación Ordenado por Dependencias

### Sprint 1: Fundación (Semana 1)
- [ ] Crear rama `feature/tower-defense-base`
- [ ] Crear estructura de carpetas TowerDefense
- [ ] Implementar sistema de Path con visualización
- [ ] Crear EnemyUnit base refactorizando enemigos actuales
- [ ] Implementar PathVisualizer en editor
- [ ] Crear escena de prueba con path simple

### Sprint 2: Lógica de Torres (Semana 2)
- [ ] Crear clase Tower abstracta
- [ ] Implementar LaserTower
- [ ] Implementar ProjectileTower
- [ ] Crear sistema de Targeting (Nearest, Weakest, etc)
- [ ] Agregar upgrade system
- [ ] Pruebas de disparo y daño

### Sprint 3: Sistema de Oleadas (Semana 3)
- [ ] Crear WaveManager y Wave SO
- [ ] Implementar SpawnSystem
- [ ] Agregar eventos de oleadas
- [ ] Crear BaseStation con HP
- [ ] Integrar game over condition
- [ ] Pruebas de oleadas completas

### Sprint 4: Placement & Economía (Semana 4)
- [ ] Implementar TowerPlacementSystem
- [ ] Grid snapping y validación
- [ ] Preview de colocación
- [ ] Refactorizar sistema de recursos
- [ ] Implementar RewardSystem
- [ ] Pruebas de colocación y economía

### Sprint 5: UI & Polish (Semana 5)
- [ ] Crear TowerDefenseHUD
- [ ] Panel de selección de torres
- [ ] Sistema de pausa
- [ ] Feedback visual de colocación
- [ ] Sfx/vfx de eventos
- [ ] Balanceo de dificultad

---

## 🎯 Ejemplo Completo: Crear la Primera Torre

### Paso 1: Crear ScriptableObject de Configuración

```csharp
// Assets/Scripts/TowerDefense/Towers/TowerConfig.cs
[CreateAssetMenu(menuName = "Tower Defense/Tower Config")]
public class TowerConfig : ScriptableObject
{
    public string towerName = "Laser Tower";
    public Sprite icon;
    public float range = 5f;
    public float fireRate = 2f;
    public float damage = 10f;
    public int cost = 100;
    public Material laserMaterial;
}

// Assets/Scripts/TowerDefense/Towers/ConfigurableTower.cs
public class ConfigurableTower : Tower
{
    [SerializeField] private TowerConfig config;
    
    protected override void Start()
    {
        base.Start();
        range = config.range;
        fireRate = config.fireRate;
        damage = config.damage;
    }
    
    protected override void Fire(EnemyUnit target)
    {
        // Implementación específica
        Debug.Log($"{config.towerName} fires at {target.name}");
    }
}
```

### Paso 2: Crear Prefab en Unity
1. Nueva escena temporal: `Assets/Scenes/TowerTest.unity`
2. Crear GameObject vacío: "LaserTower"
3. Agregar componentes:
   - SpriteRenderer (asignar sprite cósmico)
   - Circle Collider 2D
   - ConfigurableTower (script)
4. Crear SO de config: `Assets/ScriptableObjects/Towers/LaserTowerConfig.asset`
5. Asignar valores en inspector
6. Convertir a prefab: `Assets/Prefabs/Towers/LaserTower.prefab`

### Paso 3: Integrar con TowerPlacementSystem
```csharp
// En TowerDefenseHUD o escena
public class GameSetup : MonoBehaviour
{
    [SerializeField] private Tower laserTowerPrefab;
    private TowerPlacementSystem placementSystem;
    
    private void Start()
    {
        placementSystem = FindObjectOfType<TowerPlacementSystem>();
        placementSystem.SelectTowerType(laserTowerPrefab);
    }
}
```

---

## 🛠️ Prompts Optimizados para Cursor IDE

### Prompt 1: Refactorizar Enemy Existente
```
Task: Convert existing Enemy class to follow tower defense patterns
- Add Path tracking and progress (0-1)
- Remove free-moving velocity system
- Add pathProgress variable
- Implement OnReachedEnd() callback for base damage
- Keep existing health/damage systems
- Add serialization for speed parameter

Constraints:
- Must be backward compatible with existing enemy prefabs
- Don't break current sprite/animation systems
- Maintain death/explosion vfx

Acceptance Criteria:
- Enemy moves smoothly along predefined path
- Reaches end of path and triggers event
- Can be placed in scene and validated
```

### Prompt 2: Crear Sistema Completo de Torres
```
Task: Implement complete Tower system with inheritance hierarchy
- Create abstract Tower base class with fire logic
- Implement 3 tower types: Laser, Projectile, Explosive
- Add targeting priority system (nearest, lowest hp, highest threat)
- Implement upgrade mechanism (1.25x damage per level, max 5 levels)
- Tower configuration via ScriptableObject

Requirements:
- Object pooling for projectiles
- Range visualization (circle renderer)
- Tower rotation towards target
- Audio clips for fire sound
- Particle effects on hit

Acceptance Criteria:
- All 3 tower types deal damage to enemies
- Projectiles spawn and travel smoothly
- Upgrade increases visible stats
- No performance issues with 20+ towers
```

### Prompt 3: Implementar Wave Manager
```
Task: Complete wave spawning system with progression
- Spawn enemies in waves with configurable delays
- Support multiple enemy types per wave
- Increase difficulty each wave (+15% health, +10% speed)
- Detect when all enemies are defeated
- Award bonuses for wave completion

Features:
- SO-based wave configuration (5+ waves)
- Spawn point randomization (±1 unit)
- Pause between waves (countdown timer)
- Events: OnWaveStart, OnWaveComplete, OnGameOver
- Save/load wave progress

Acceptance Criteria:
- Waves spawn and complete correctly
- Difficulty progresses smoothly
- No memory leaks from destroyed enemies
- Can load wave data from SO
```

---

## 🔍 Testing Strategy

```csharp
// Assets/Tests/EditMode/TowerDefenseTests.cs
[TestFixture]
public class TowerDefenseTests
{
    [Test]
    public void Tower_ShouldDamageEnemyInRange()
    {
        // Arrange
        var tower = CreateLaserTower();
        var enemy = CreateEnemy(range: 3f);
        
        // Act
        tower.GetComponent<LaserTower>().Update();
        
        // Assert
        Assert.AreEqual(90, enemy.Health); // 100 - 10 damage
    }
    
    [Test]
    public void Path_ShouldReturnCorrectPositions()
    {
        // Arrange
        var path = new Path 
        { 
            waypoints = new[] 
            { 
                Vector3.zero, 
                Vector3.right * 10,
                Vector3.up * 10 
            }
        };
        
        // Act
        var midpoint = path.GetPositionAtProgress(0.5f);
        
        // Assert
        Assert.IsTrue(Vector3.Distance(midpoint, Vector3.right * 5) < 0.1f);
    }
    
    [Test]
    public void WaveManager_ShouldCompleteWaveWhenAllEnemiesDead()
    {
        // Arrange
        var waveManager = CreateWaveManager();
        var wave = CreateTestWave(enemyCount: 3);
        
        // Act
        waveManager.StartWave(wave);
        KillAllEnemies();
        
        // Assert
        Assert.IsTrue(waveManager.IsWaveComplete);
    }
}
```

---

## 📊 Curva de Dificultad Recomendada

```csharp
public class DifficultyScaler : MonoBehaviour
{
    public float GetHealthMultiplier(int waveNumber)
    {
        return 1f + (waveNumber - 1) * 0.15f; // +15% por onda
    }
    
    public float GetSpeedMultiplier(int waveNumber)
    {
        return 1f + (waveNumber - 1) * 0.1f; // +10% por onda
    }
    
    public float GetDamageMultiplier(int waveNumber)
    {
        return 1f + (waveNumber - 1) * 0.08f; // +8% por onda
    }
    
    public int GetEnemyCountIncrease(int waveNumber)
    {
        return Mathf.RoundToInt(waveNumber * 0.5f); // +50% enemigos cada 2 ondas
    }
}
```

---

## ⚠️ Errores Comunes a Evitar

1. **No poolear projectiles** → Creará GC stutters con muchas torres
   - **Solución**: ObjectPool reutilizable
   
2. **Enemigseos se superponen en spawn** → Colisiones físicas lentas
   - **Solución**: Spawn con offset o distancia mínima

3. **Towers no usan GridLayout** → Colocación inconsistente
   - **Solución**: Siempre snapear a grid

4. **Estados de juego confusos** → Pausar sin parar spawns
   - **Solución**: State machine centralizado

5. **No limpiar corrutinas** → Memory leaks en wave manager
   - **Solución**: StopAllCoroutines() en Destroy

---

## 📚 Referencias de Arquitectura

### Pattern: Object Pool
```csharp
public class ProjectilePool : MonoBehaviour
{
    private Queue<Projectile> pool = new();
    [SerializeField] private Projectile prefab;
    [SerializeField] private int poolSize = 50;
    
    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Projectile p = Instantiate(prefab);
            p.gameObject.SetActive(false);
            pool.Enqueue(p);
        }
    }
    
    public Projectile Get() => 
        pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
    
    public void Return(Projectile p)
    {
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}
```

### Pattern: Event Bus
```csharp
public class GameEvents : MonoBehaviour
{
    public static event System.Action<int> OnWaveStarted;
    public static event System.Action<EnemyUnit> OnEnemyKilled;
    public static event System.Action OnGameOver;
    
    public static void TriggerWaveStarted(int waveNumber) =>
        OnWaveStarted?.Invoke(waveNumber);
}
```

### Pattern: Service Locator
```csharp
public class GameServices : MonoBehaviour
{
    public static T Get<T>() where T : MonoBehaviour =>
        FindObjectOfType<T>();
}

// Uso: var waveManager = GameServices.Get<WaveManager>();
```

---

## 🎓 Objetivos de Aprendizaje

Completar esta conversión te enseñará:

✅ **Architecture**: De juego arcade a strategic
✅ **State Management**: GameStateMachine más complejo
✅ **Event System**: Desacoplamiento mediante eventos
✅ **Resource Management**: Object pooling y GC optimization
✅ **Data Design**: ScriptableObjects para configuración
✅ **Testing**: Juegos tower defense son testables
✅ **UX/UI**: HUD dinámico y feedback visual
✅ **Game Balance**: Curvas de dificultad

---

## 📞 Próximos Pasos

1. **Crear PR inicial**: Rama `feature/tower-defense-base` con estructura
2. **First tower implementation**: LaserTower funcional
3. **PathFinding validation**: Enemies siguiendo rutas
4. **Wave testing**: Sistema completo de oleadas
5. **Integration**: Todas las piezas conectadas
6. **Polish**: Balanceo, UI, sfx/vfx

---

**Generado por AI DEV**  
**Tech Stack**: C# | Unity | Game Architecture  
**Estimated effort**: 60-80 horas (5-6 sprints de 2 semanas)  
**Complexity**: Medium-High (Tower Defense fundamentals)
