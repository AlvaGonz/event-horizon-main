using UnityEngine;
using System.Collections.Generic;

namespace Game.TowerDefense
{
    /// <summary>
    /// Explosive tower - area of effect damage
    /// </summary>
    public class ExplosiveTower : Tower
    {
        [Header("Explosive")]
        [SerializeField] private float explosionRadius = 2f;
        [SerializeField] private GameObject explosionEffectPrefab;
        [SerializeField] private float damageFalloffPercent = 0.5f; // 50% damage at edge
        
        protected override void Fire()
        {
            if (currentTarget == null) return;
            
            Vector3 explosionPos = currentTarget.Position;
            
            // Find all enemies in explosion radius
            var allEnemies = FindObjectsOfType<EnemyUnit>();
            List<EnemyUnit> affectedEnemies = new List<EnemyUnit>();
            
            foreach (var enemy in allEnemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector3.Distance(explosionPos, enemy.Position);
                if (distance <= explosionRadius)
                {
                    affectedEnemies.Add(enemy);
                    
                    // Calculate damage based on distance (falloff)
                    float distancePercent = distance / explosionRadius;
                    float damageMultiplier = Mathf.Lerp(1f, damageFalloffPercent, distancePercent);
                    float damage = currentDamage * damageMultiplier;
                    
                    enemy.TakeDamage(damage);
                }
            }
            
            // Spawn explosion effect
            if (explosionEffectPrefab != null)
            {
                GameObject effect = Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
                Destroy(effect, 2f);
            }
            else
            {
                // Draw debug visualization
                Debug.DrawRay(explosionPos, Vector3.up * 2f, Color.red, 0.5f);
            }
            
            Debug.Log($"[ExplosiveTower] Explosion hit {affectedEnemies.Count} enemies");
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw explosion radius when targeting
            if (currentTarget != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                Gizmos.DrawWireSphere(currentTarget.Position, explosionRadius);
            }
        }
    }
}
