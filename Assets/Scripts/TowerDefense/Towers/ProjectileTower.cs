using UnityEngine;

namespace Game.TowerDefense.Towers
{
    /// <summary>
    /// Projectile tower - fires projectiles that travel to target
    /// </summary>
    public class ProjectileTower : Tower
    {
        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float projectileSpeed = 10f;
        
        protected override void Fire()
        {
            if (currentTarget == null) return;
            
            // Spawn projectile
            GameObject projectileGo;
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            
            if (projectilePrefab != null)
            {
                projectileGo = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                // Create simple sphere as fallback
                projectileGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projectileGo.transform.position = spawnPos;
                projectileGo.transform.localScale = Vector3.one * 0.2f;
                projectileGo.GetComponent<Renderer>().material.color = Color.yellow;
            }
            
            // Add projectile component
            Projectile projectile = projectileGo.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = projectileGo.AddComponent<Projectile>();
            }
            
            projectile.Initialize(currentTarget, currentDamage, projectileSpeed);
            
            Debug.Log($"[ProjectileTower] Fired projectile at {currentTarget.name}");
        }
    }
}
