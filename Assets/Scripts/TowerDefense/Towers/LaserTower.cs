using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// Laser tower - instant hit with LineRenderer beam
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LaserTower : Tower
    {
        [Header("Laser Visual")]
        [SerializeField] private float beamDuration = 0.1f;
        [SerializeField] private Color beamColor = Color.cyan;
        [SerializeField] private float beamWidth = 0.1f;
        
        private LineRenderer lineRenderer;
        private float beamEndTime;
        
        protected override void Awake()
        {
            base.Awake();
            lineRenderer = GetComponent<LineRenderer>();
            
            if (lineRenderer != null)
            {
                lineRenderer.startWidth = beamWidth;
                lineRenderer.endWidth = beamWidth;
                lineRenderer.startColor = beamColor;
                lineRenderer.endColor = beamColor;
                lineRenderer.enabled = false;
                lineRenderer.positionCount = 2;
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Hide beam after duration
            if (lineRenderer != null && lineRenderer.enabled && Time.time >= beamEndTime)
            {
                lineRenderer.enabled = false;
            }
        }
        
        protected override void Fire()
        {
            if (currentTarget == null) return;
            
            // Instant hit - deal damage immediately
            currentTarget.TakeDamage(currentDamage);
            
            // Visual effect: draw laser beam
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, currentTarget.Position);
                lineRenderer.enabled = true;
                beamEndTime = Time.time + beamDuration;
            }
            
            Debug.Log($"[LaserTower] Fired at {currentTarget.name}, dealt {currentDamage} damage");
        }
    }
}
