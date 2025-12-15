# 🚀 Event Horizon Frontier - Quick Start Guide
## Primeros Pasos para Convertir a Tower Defense

**[STACK: C# | Unity | Game Development | Fast Iteration]**

---

## 📋 Requisitos Previos

- ✅ Unity 2022 LTS o superior
- ✅ Proyecto Event Horizon Develop branch
- ✅ Git con acceso a branches
- ✅ Cursor IDE (recomendado) o VS Code

---

## ⚡ 30 minutos: Setup Inicial

### Paso 1: Crear Rama Feature (5 min)

```bash
# En tu proyecto local
cd event-horizon-main
git fetch origin
git checkout Develop
git pull origin Develop

# Crear rama nueva
git checkout -b feature/tower-defense-base

# Verificar rama
git branch
```

### Paso 2: Crear Estructura de Carpetas (5 min)

```
Assets/Scripts/TowerDefense/
├── Enemy/
│   ├── EnemyUnit.cs
│   └── EnemyConfig.cs
├── Towers/
│   ├── Tower.cs (base)
│   ├── LaserTower.cs
│   ├── ProjectileTower.cs
│   └── TowerConfig.cs
├── WaveSystem/
│   ├── WaveManager.cs
│   ├── Wave.cs
│   └── DifficultyScaler.cs
├── TowerPlacement/
│   └── TowerPlacementSystem.cs
├── BaseDefense/
│   └── BaseStation.cs
├── PathFinding/
│   ├── Path.cs
│   └── PathVisualizer.cs
├── Combat/
│   ├── Projectile.cs
│   └── ProjectilePool.cs
└── Economy/
    └── RewardSystem.cs
```

**Crear carpetas en Unity:**
1. Abrir Assets/Scripts/
2. Click derecho → Create → Folder → "TowerDefense"
3. Dentro de TowerDefense, crear subcarpetas

### Paso 3: Crear Primera Clase (5 min)

Crear `Assets/Scripts/TowerDefense/PathFinding/Path.cs`:

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a path that enemies follow in tower defense
/// </summary>
[System.Serializable]
public class Path
{
    [SerializeField] private Vector3[] waypoints;
    
    private float totalLength;
    
    public float Length => totalLength;
    public Vector3[] Waypoints => waypoints;
    
    /// <summary>Initialize path and calculate total length</summary>
    public void Initialize()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("Path needs at least 2 waypoints");
            return;
        }
        
        totalLength = 0;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            totalLength += Vector3.Distance(waypoints[i], waypoints[i + 1]);
        }
    }
    
    /// <summary>Get position along path from 0 to 1 progress</summary>
    public Vector3 GetPositionAtProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        
        if (waypoints.Length < 2) return Vector3.zero;
        
        float distanceToTravel = totalLength * progress;
        float currentDistance = 0f;
        
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(waypoints[i], waypoints[i + 1]);
            
            if (currentDistance + segmentLength >= distanceToTravel)
            {
                float localProgress = segmentLength > 0 
                    ? (distanceToTravel - currentDistance) / segmentLength 
                    : 0;
                return Vector3.Lerp(waypoints[i], waypoints[i + 1], localProgress);
            }
            
            currentDistance += segmentLength;
        }
        
        return waypoints[waypoints.Length - 1];
    }
    
    /// <summary>Get direction along path</summary>
    public Vector3 GetDirectionAtProgress(float progress)
    {
        Vector3 pos1 = GetPositionAtProgress(progress - 0.01f);
        Vector3 pos2 = GetPositionAtProgress(progress + 0.01f);
        return (pos2 - pos1).normalized;
    }
}
```

### Paso 4: Crear SO para Config (5 min)

Crear `Assets/Scripts/TowerDefense/Enemy/EnemyConfig.cs`:

```csharp
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    public string enemyName = "Fighter";
    public float maxHealth = 20f;
    public float speed = 3f;
    public int rewardCredits = 10;
    public float baseDamage = 5f;
}
```

Crear SO en el editor:
1. Click derecho en Assets/ScriptableObjects/ → Create → Enemy Config
2. Asignar valores
3. Guardar como "FighterConfig"

---

## 🎮 2 Horas: Primera Escena Funcional

### Paso 5: Crear Escena de Prueba (30 min)

1. **Crear escena**: File → New Scene → "TowerDefenseTest"
2. **Agregar objetos base**:
   - Cámara (Main Camera)
   - Canvas para UI
   - Quad para fondo

3. **Setup de escena**:
```csharp
// TowerDefenseTest.unity hierarchy:
TowerDefenseTest (scene)
├── Main Camera
├── Game Scene (empty object for organization)
│   ├── Spawn Point
│   ├── Base Station (sprite + scripts)
│   ├── Path Visualizer (gizmos)
│   └── Enemy Container
└── Canvas (UI)
    ├── Wave Counter
    ├── Credits Display
    └── Health Bar
```

### Paso 6: Implementar EnemyUnit Refactorizada (45 min)

Crear `Assets/Scripts/TowerDefense/Enemy/EnemyUnit.cs`:

```csharp
using UnityEngine;
using System;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private EnemyConfig config;
    
    private Path path;
    private float pathProgress = 0f;
    private float currentHealth;
    private bool isDead = false;
    
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnReachedEnd;
    
    public float Health => currentHealth;
    public float HealthPercent => currentHealth / maxHealth;
    public bool IsDead => isDead;
    
    private void Start()
    {
        if (config != null)
        {
            maxHealth = config.maxHealth;
            speed = config.speed;
        }
        
        currentHealth = maxHealth;
    }
    
    private void Update()
    {
        if (path == null || isDead) return;
        
        // Avanzar en el camino
        pathProgress += (speed * Time.deltaTime) / path.Length;
        
        // Actualizar posición
        transform.position = path.GetPositionAtProgress(pathProgress);
        
        // Rotar hacia dirección
        Vector3 direction = path.GetDirectionAtProgress(pathProgress);
        if (direction.magnitude > 0)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        // Verificar si llegó al final
        if (pathProgress >= 1f)
        {
            OnReachedEnd?.Invoke();
            Die();
        }
    }
    
    public void SetPath(Path newPath)
    {
        path = newPath;
        pathProgress = 0f;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        OnHealthChanged?.Invoke(HealthPercent);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
        
        // Destruir después de animation delay
        Destroy(gameObject, 0.5f);
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // Dibujar health bar
        Gizmos.color = Color.Lerp(Color.green, Color.red, 1f - HealthPercent);
        Vector3 healthBarStart = transform.position + Vector3.up * 1f;
        Vector3 healthBarEnd = healthBarStart + Vector3.right * (HealthPercent * 0.5f);
        Gizmos.DrawLine(healthBarStart, healthBarEnd);
    }
}
```

### Paso 7: Crear Sistema Básico de Torres (45 min)

Crear `Assets/Scripts/TowerDefense/Towers/Tower.cs`:

```csharp
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected TowerConfig config;
    [SerializeField] protected Transform firePoint;
    
    protected float range = 5f;
    protected float fireRate = 1f;
    protected float damage = 10f;
    public int cost { get; protected set; }
    
    protected float lastFireTime = 0f;
    private CircleCollider2D rangeCollider;
    
    protected virtual void Start()
    {
        if (config != null)
        {
            range = config.range;
            fireRate = config.fireRate;
            damage = config.damage;
            cost = config.cost;
        }
        
        // Crear área de detección
        rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        rangeCollider.radius = range;
        rangeCollider.isTrigger = true;
        
        gameObject.tag = "Tower";
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
        // Buscar enemigos en rango
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            range,
            LayerMask.GetMask("Enemy")
        );
        
        if (colliders.Length > 0)
        {
            EnemyUnit target = colliders[0].GetComponent<EnemyUnit>();
            if (target != null && !target.IsDead)
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
```

Crear `Assets/Scripts/TowerDefense/Towers/LaserTower.cs`:

```csharp
using UnityEngine;
using System.Collections;

public class LaserTower : Tower
{
    private LineRenderer laser;
    
    protected override void Start()
    {
        base.Start();
        
        // Crear laser
        laser = gameObject.AddComponent<LineRenderer>();
        laser.material = new Material(Shader.Find("Sprites/Default"));
        laser.startColor = Color.cyan;
        laser.endColor = Color.cyan;
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        laser.positionCount = 0;
    }
    
    protected override void Fire(EnemyUnit target)
    {
        // Mostrar laser
        laser.positionCount = 2;
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, target.transform.position);
        
        // Aplicar daño
        target.TakeDamage(damage);
        
        // Esconder laser después de delay
        StartCoroutine(DisableLaserAfterDelay(0.1f));
    }
    
    private IEnumerator DisableLaserAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        laser.positionCount = 0;
    }
}
```

Crear `Assets/Scripts/TowerDefense/Towers/TowerConfig.cs`:

```csharp
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Tower Config")]
public class TowerConfig : ScriptableObject
{
    public string towerName = "Laser Tower";
    public Sprite icon;
    public float range = 5f;
    public float fireRate = 2f;
    public float damage = 10f;
    public int cost = 100;
}
```

---

## 🧪 30 minutos: Testing en Editor

### Paso 8: Setup de Tags y Layers

En Edit → Project Settings → Tags and Layers:

```
Tags:
- Tower
- Enemy
- Base

Layers:
- Enemy (layer 6)
- Obstacle (layer 7)
```

### Paso 9: Preparar Prefabs

1. **Enemy Prefab**:
   - Crear un Quad visual
   - Agregar SpriteRenderer (color azul para enemy)
   - Agregar Collider 2D
   - Agregar EnemyUnit script
   - Asignar enemyconfig
   - Guardar como `Assets/Prefabs/Enemy/Fighter.prefab`

2. **Tower Prefab**:
   - Crear un Quad visual
   - Agregar SpriteRenderer (color rojo para tower)
   - Agregar LaserTower script
   - Asignar TowerConfig
   - Guardar como `Assets/Prefabs/Towers/LaserTower.prefab`

### Paso 10: Setup de Escena de Prueba

En TowerDefenseTest.unity:

```
1. Path setup:
   - Crear GameObject "Path"
   - Agregar Path.cs como SO con waypoints
   - Waypoints: (0,0,0) → (5,0,0) → (5,5,0) → (10,5,0)
   - Agregar PathVisualizer para ver en gizmos

2. Tower placement:
   - Instanciar LaserTower prefab en (0,3,0)
   - Verificar range circle en gizmos

3. Enemy spawn:
   - Instanciar Fighter prefab en path start
   - SetPath(path)
   - Play scene

4. Base station:
   - Crear Quad en (10,5,0)
   - Agregar BaseStation script
   - Configurar health 100
```

### Paso 11: Test Flow

```csharp
// Debug script para testing
public class QuickTest : MonoBehaviour
{
    [SerializeField] private EnemyUnit enemy;
    [SerializeField] private Tower tower;
    
    private void Update()
    {
        // Presionar T para damage test
        if (Input.GetKeyDown(KeyCode.T))
        {
            enemy.TakeDamage(10);
            Debug.Log($"Enemy health: {enemy.Health}");
        }
        
        // Presionar F para fire test
        if (Input.GetKeyDown(KeyCode.F))
        {
            tower.GetComponent<LaserTower>().Shoot(enemy);
            Debug.Log("Tower fired!");
        }
    }
}
```

---

## ✅ Verificación de Funcionalidad

```
☐ Path visualization visible en gizmos
☐ Enemy se mueve suavemente por el path
☐ Enemy completa path sin errores
☐ Tower detecta enemy en rango
☐ Tower dispara laser cada frame válido
☐ Enemy recibe daño al ser golpeado
☐ Enemy muere cuando health ≤ 0
☐ Laser desaparece después del disparo
☐ No hay errores en console
```

---

## 📊 Commit Progress

```bash
# Después de terminar paso 4
git add Assets/Scripts/TowerDefense/
git commit -m "feat: Initial tower defense folder structure and Path system"
git push origin feature/tower-defense-base

# Después de terminar paso 6
git add Assets/Scripts/TowerDefense/Enemy/
git commit -m "feat: EnemyUnit implementation with pathfinding"
git push

# Después de terminar paso 7
git add Assets/Scripts/TowerDefense/Towers/
git commit -m "feat: Tower base class and LaserTower implementation"
git push

# Después de terminar paso 11
git add Assets/Scenes/ Assets/Prefabs/
git commit -m "test: Tower Defense test scene with functional enemy and tower"
git push
```

---

## 🎯 Próximos Pasos Después del Quick Start

1. **Completar Sprint 1**: Path system + enemy refactoring
2. **Implementar Sprint 2**: Múltiples tipos de torres
3. **Agregar Sprint 3**: Wave manager y progresión
4. **Continuar con Sprints 4-5**: Placement system y UI

---

## 🆘 Troubleshooting

| Problema | Solución |
|----------|----------|
| Enemy no se mueve | Verificar Path.Initialize() fue llamado |
| Tower no detecta enemy | Verificar layer "Enemy" en prefab |
| Laser no aparece | Verificar LineRenderer material |
| Errores de null | Verificar asignaciones en inspector |
| Performance lento | Usar Play mode profiler (Window → Analysis → Profiler) |

---

**¡Listo! Tienes tu primera funcionalidad de Tower Defense en 2.5 horas.** 🎉

Ahora puedes continuar con los prompts de Cursor IDE para expandir cada sistema.

