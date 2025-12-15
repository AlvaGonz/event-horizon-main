using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.TowerDefense
{
    /// <summary>
    /// Abstract base class for all tower types
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Tower : MonoBehaviour
    {
        [SerializeField] protected TowerConfig config;
        [SerializeField] protected int currentLevel = 0;
        
        protected float currentRange;
        protected float currentDamage;
        protected float currentFireRate;
        protected float lastFireTime;
        protected EnemyUnit currentTarget;
        protected SpriteRenderer spriteRenderer;
        protected List<EnemyUnit> enemiesInRange = new List<EnemyUnit>();
        
        public TowerConfig Config => config;
        public int Level => currentLevel;
        public float Range => currentRange;
        public bool CanUpgrade => currentLevel < config.maxUpgradeLevel;
        public int UpgradeCost => CanUpgrade ? config.upgradeCosts[currentLevel] : 0;
        public int SellValue => CalculateSellValue();
        
        // ===== INITIALIZATION =====
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        protected virtual void Start()
        {
            InitializeFromConfig();
        }
        
        protected virtual void InitializeFromConfig()
        {
            if (config == null)
            {
                Debug.LogError("[Tower] No config assigned!");
                return;
            }
            
            currentRange = config.range;
            currentDamage = config.damage;
            currentFireRate = config.fireRate;
            
            if (config.towerSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = config.towerSprite;
            }
            
            ApplyUpgradeStats();
        }
        
        public void SetConfig(TowerConfig newConfig)
        {
            config = newConfig;
            InitializeFromConfig();
        }
        
        // ===== UPDATE LOOP =====
        protected virtual void Update()
        {
            RefreshEnemiesInRange();
            AcquireTarget();
            
            if (currentTarget != null && CanFire())
            {
                Fire();
                lastFireTime = Time.time;
            }
        }
        
        // ===== TARGETING SYSTEM =====
        protected virtual void RefreshEnemiesInRange()
        {
            // Remove dead or out-of-range enemies
            enemiesInRange.RemoveAll(e => e == null || e.IsDead || 
                Vector3.Distance(transform.position, e.Position) > currentRange);
            
            // Find all enemies in range
            var allEnemies = FindObjectsOfType<EnemyUnit>();
            foreach (var enemy in allEnemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector3.Distance(transform.position, enemy.Position);
                if (distance <= currentRange && !enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }
        
        protected virtual void AcquireTarget()
        {
            // Clear target if invalid
            if (currentTarget == null || currentTarget.IsDead || 
                Vector3.Distance(transform.position, currentTarget.Position) > currentRange)
            {
                currentTarget = null;
            }
            
            // Find new target if needed
            if (currentTarget == null && enemiesInRange.Count > 0)
            {
                // Default: target nearest enemy
                currentTarget = enemiesInRange
                    .OrderBy(e => Vector3.Distance(transform.position, e.Position))
                    .FirstOrDefault();
            }
        }
        
        protected bool CanFire()
        {
            return Time.time >= lastFireTime + (1f / currentFireRate);
        }
        
        // ===== COMBAT (Abstract - implemented by subclasses) =====
        protected abstract void Fire();
        
        // ===== UPGRADE SYSTEM =====
        public bool Upgrade()
        {
            if (!CanUpgrade)
            {
                Debug.LogWarning("[Tower] Already at max level!");
                return false;
            }
            
            currentLevel++;
            ApplyUpgradeStats();
            
            Debug.Log($"[Tower] Upgraded to level {currentLevel}");
            return true;
        }
        
        protected virtual void ApplyUpgradeStats()
        {
            if (config == null) return;
            
            // Calculate multipliers based on level
            float damageMultiplier = Mathf.Pow(config.damageMultiplierPerLevel, currentLevel);
            float fireRateMultiplier = Mathf.Pow(config.fireRateMultiplierPerLevel, currentLevel);
            
            currentDamage = config.damage * damageMultiplier;
            currentRange = config.range + (config.rangeBonusPerLevel * currentLevel);
            currentFireRate = config.fireRate * fireRateMultiplier;
        }
        
        protected int CalculateSellValue()
        {
            if (config == null) return 0;
            
            int totalInvestment = config.buildCost;
            for (int i = 0; i < currentLevel; i++)
            {
                if (i < config.upgradeCosts.Length)
                    totalInvestment += config.upgradeCosts[i];
            }
            
            return Mathf.RoundToInt(totalInvestment * config.sellValuePercent);
        }
        
        // ===== UTILITY =====
        protected Vector3 GetDirectionToTarget()
        {
            if (currentTarget == null) return Vector3.right;
            return (currentTarget.Position - transform.position).normalized;
        }
        
        protected void RotateTowardsTarget()
        {
            if (currentTarget == null) return;
            
            Vector3 direction = GetDirectionToTarget();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }
        
        // ===== VISUALIZATION =====
        protected virtual void OnDrawGizmos()
        {
            // Draw range circle
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            DrawCircle(transform.position, currentRange > 0 ? currentRange : (config != null ? config.range : 5f));
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw range circle (highlighted)
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            DrawCircle(transform.position, currentRange > 0 ? currentRange : (config != null ? config.range : 5f));
            
            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.Position);
            }
        }
        
        private void DrawCircle(Vector3 center, float radius, int segments = 32)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
