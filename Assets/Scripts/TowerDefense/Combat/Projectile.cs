using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// Projectile script for towers that fire traveling projectiles
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private EnemyUnit target;
        private float damage;
        private float speed;
        private float lifetime = 5f;
        private float spawnTime;
        
        public void Initialize(EnemyUnit targetEnemy, float projectileDamage, float projectileSpeed)
        {
            target = targetEnemy;
            damage = projectileDamage;
            speed = projectileSpeed;
            spawnTime = Time.time;
        }
        
        private void Update()
        {
            // Destroy if too old
            if (Time.time > spawnTime + lifetime)
            {
                Destroy(gameObject);
                return;
            }
            
            // Check if target is still valid
            if (target == null || target.IsDead)
            {
                Destroy(gameObject);
                return;
            }
            
            // Move towards target
            Vector3 direction = (target.Position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            
            // Rotate to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Check if close enough to hit
            float distance = Vector3.Distance(transform.position, target.Position);
            if (distance < 0.2f)
            {
                Hit();
            }
        }
        
        private void Hit()
        {
            if (target != null && !target.IsDead)
            {
                target.TakeDamage(damage);
            }
            
            // Could spawn impact effect here
            
            Destroy(gameObject);
        }
        
        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, target.Position);
            }
        }
    }
}
