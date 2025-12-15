using UnityEngine;
using System;

namespace Game.TowerDefense
{
    /// <summary>
    /// Tower Defense enemy unit that follows predefined paths.
    /// Refactored from original arcade enemy system for TD gameplay.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyUnit : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        
        [Header("Runtime Stats")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float currentHealth;
        
        private Path path;
        private float pathProgress = 0f;
        private float maxHealth;
        private bool isDead = false;
        private SpriteRenderer spriteRenderer;
        
        // Events for wave manager and reward system
        public event Action<float> OnHealthChanged;  // healthPercent (0-1)
        public event Action<EnemyUnit> OnDeath;      // When health reaches 0
        public event Action<EnemyUnit, float> OnReachedEnd; // When reaches end of path (unit, baseDamage)
        
        public float Health => currentHealth;
        public float HealthPercent => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        public EnemyConfig Config => config;
        public Vector3 Position => transform.position;
        
        // ===== INITIALIZATION =====
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Start()
        {
            InitializeFromConfig();
        }
        
        public void InitializeFromConfig()
        {
            if (config != null)
            {
                maxHealth = config.maxHealth;
                speed = config.speed;
                
                if (config.sprite != null && spriteRenderer != null)
                {
                    spriteRenderer.sprite = config.sprite;
                    transform.localScale = Vector3.one * config.spriteScale;
                }
            }
            else
            {
                // Fallback defaults
                maxHealth = 20f;
                speed = 3f;
            }
            
            currentHealth = maxHealth;
        }
        
        /// <summary>
        /// Set configuration at runtime (for pooling)
        /// </summary>
        public void SetConfig(EnemyConfig newConfig, float healthMultiplier = 1f, float speedMultiplier = 1f)
        {
            config = newConfig;
            maxHealth = config.maxHealth * healthMultiplier;
            speed = config.speed * speedMultiplier;
            currentHealth = maxHealth;
            
            if (config.sprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = config.sprite;
                transform.localScale = Vector3.one * config.spriteScale;
            }
        }
        
        // ===== MOVEMENT & PATHFINDING =====
        private void Update()
        {
            if (path == null || isDead)
                return;
            
            // Calculate progress along path (0 to 1)
            float pathDistance = path.Length;
            if (pathDistance > 0)
            {
                pathProgress += (speed * Time.deltaTime) / pathDistance;
            }
            
            // Update position
            transform.position = path.GetPositionAtProgress(pathProgress);
            
            // Update rotation to face direction
            Vector3 direction = path.GetDirectionAtProgress(pathProgress);
            if (direction.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward); // -90 to face forward
            }
            
            // Check if reached end
            if (pathProgress >= 1f)
            {
                ReachEnd();
            }
        }
        
        public void SetPath(Path newPath)
        {
            path = newPath;
            pathProgress = 0f;
            
            if (path != null && !path.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("[EnemyUnit] Path is not active in hierarchy!");
            }
        }
        
        // ===== HEALTH & DAMAGE =====
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            OnHealthChanged?.Invoke(HealthPercent);
            
            // Visual feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, HealthPercent);
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(float amount)
        {
            if (isDead) return;
            
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(HealthPercent);
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, HealthPercent);
            }
        }
        
        // ===== DEATH & END =====
        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            OnDeath?.Invoke(this);
            
            // Visual feedback (optional)
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.gray;
            }
            
            // Destroy after brief delay
            Destroy(gameObject, 0.2f);
        }
        
        private void ReachEnd()
        {
            if (isDead) return;
            
            isDead = true;
            
            // Calculate damage to base
            float baseDamage = config != null ? config.baseDamage : 5f;
            OnReachedEnd?.Invoke(this, baseDamage);
            
            Debug.Log($"[EnemyUnit] {gameObject.name} reached base! Dealing {baseDamage} damage.");
            
            // Destroy immediately
            Destroy(gameObject, 0.1f);
        }
        
        // ===== DEBUG VISUALIZATION =====
        private void OnDrawGizmos()
        {
            if (isDead) return;
            
            // Draw health bar above enemy
            Vector3 healthBarPos = transform.position + Vector3.up * 1f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 0.25f, healthBarPos + Vector3.right * 0.25f);
            
            Gizmos.color = Color.green;
            float healthWidth = HealthPercent * 0.5f;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 0.25f, 
                           healthBarPos - Vector3.right * 0.25f + Vector3.right * healthWidth);
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw path connection
            if (path != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, path.GetPositionAtProgress(pathProgress + 0.1f));
            }
        }
    }
}
