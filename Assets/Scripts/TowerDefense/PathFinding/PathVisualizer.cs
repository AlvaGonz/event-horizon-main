using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// Editor component to visualize path in scene view
    /// </summary>
    [RequireComponent(typeof(Path))]
    public class PathVisualizer : MonoBehaviour
    {
        [SerializeField] private Color pathColor = Color.yellow;
        [SerializeField] private Color waypointColor = Color.green;
        [SerializeField] private float waypointRadius = 0.3f;
        [SerializeField] private bool showDirectionArrows = true;
        
        private Path path;
        
        private void OnDrawGizmos()
        {
            if (path == null)
                path = GetComponent<Path>();
            
            if (path == null)
                return;
            
            // Path component already draws its own gizmos
            // This is a placeholder for additional visualization if needed
        }
        
        private void OnDrawGizmosSelected()
        {
            // Additional visual feedback when selected
            if (path == null)
                path = GetComponent<Path>();
            
            if (path == null || path.WaypointCount < 2)
                return;
            
            // Draw path progress visualization (sample points)
            Gizmos.color = Color.white;
            int samples = 20;
            for (int i = 0; i <= samples; i++)
            {
                float progress = (float)i / samples;
                Vector3 pos = path.GetPositionAtProgress(progress);
                Gizmos.DrawWireSphere(pos, 0.1f);
            }
        }
    }
}
