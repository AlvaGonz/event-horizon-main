using UnityEngine;
using System;

namespace Game.TowerDefense
{
    /// <summary>
    /// The base station that must be defended.
    /// Enemies deal damage when they reach the end of their path.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BaseStation : MonoBehaviour
    {
        [SerializeField] private BaseConfig config;
        [SerializeField] private float currentHealth;
        
        private float maxHealth;
        private SpriteRenderer spriteRenderer;
        private bool isDestroyed = false;
        
        // Events
        public event Action<float> OnHealthChanged;     // healthPercent (0-1)
        public event Action<float, float> OnDamaged;    // damage, newHealth
        public event Action OnDestroyed;
        public event Action OnCriticalHealth;           // Triggered when health drops below threshold
        
        public float Health => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercent => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        public bool IsDestroyed => isDestroyed;
        public bool IsCritical => config != null && HealthPercent <= config.criticalHealthPercent;
        
        // ===== INITIALIZATION =====
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Start()
        {
            InitializeFromConfig();
        }
        
        private void InitializeFromConfig()
        {
            if (config != null)
            {
                maxHealth = config.maxHealth;
                currentHealth = maxHealth;
                
                if (config.baseSprite != null && spriteRenderer != null)
                {
                    spriteRenderer.sprite = config.baseSprite;
                }
            }
            else
            {
                maxHealth = 100f;
                currentHealth = maxHealth;
            }
            
            UpdateVisuals();
        }
        
        public void SetConfig(BaseConfig newConfig)
        {
            config = newConfig;
            InitializeFromConfig();
        }
        
        // ===== DAMAGE SYSTEM =====
        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;
            
            bool wasCritical = IsCritical;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            Debug.Log($"[BaseStation] Took {damage} damage. Health: {currentHealth}/{maxHealth}");
            
            // Fire events
            OnHealthChanged?.Invoke(HealthPercent);
            OnDamaged?.Invoke(damage, currentHealth);
            
            // Check for critical state transition
            if (!wasCritical && IsCritical)
            {
                OnCriticalHealth?.Invoke();
                Debug.LogWarning("[BaseStation] Entered critical health state!");
            }
            
            UpdateVisuals();
            
            // Check for destruction
            if (currentHealth <= 0)
            {
                Destroy();
            }
        }
        
        public void Heal(float amount)
        {
            if (isDestroyed) return;
            
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(HealthPercent);
            
            UpdateVisuals();
        }
        
        private void Destroy()
        {
            if (isDestroyed) return;
            
            isDestroyed = true;
            Debug.LogError("[BaseStation] BASE DESTROYED! GAME OVER!");
            
            OnDestroyed?.Invoke();
            
            // Visual feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.black;
            }
        }
        
        // ===== VISUAL FEEDBACK =====
        private void UpdateVisuals()
        {
            if (spriteRenderer == null || config == null) return;
            
            // Color feedback based on health
            Color targetColor = Color.Lerp(config.criticalColor, config.healthyColor, HealthPercent);
            spriteRenderer.color = targetColor;
            
            // Optional: add pulsing effect when critical
            if (IsCritical)
            {
                float pulse = Mathf.PingPong(Time.time * 2f, 1f);
                spriteRenderer.color = Color.Lerp(config.criticalColor, targetColor, pulse);
            }
        }
        
        private void Update()
        {
            // Continuous visual update for pulsing effect
            if (IsCritical && !isDestroyed)
            {
                UpdateVisuals();
            }
        }
        
        // ===== DEBUG VISUALIZATION =====
        private void OnDrawGizmos()
        {
            // Draw health bar above base
            Vector3 healthBarPos = transform.position + Vector3.up * 2f;
            
            // Background
            Gizmos.color = Color.red;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 1f, healthBarPos + Vector3.right * 1f);
            
            // Health
            Gizmos.color = Color.green;
            float healthWidth = HealthPercent * 2f;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 1f, 
                           healthBarPos - Vector3.right * 1f + Vector3.right * healthWidth);
            
            // Critical indicator
            if (IsCritical)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
