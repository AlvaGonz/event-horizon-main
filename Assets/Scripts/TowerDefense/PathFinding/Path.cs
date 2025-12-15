using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game.TowerDefense
{
    /// <summary>
    /// Defines a path for enemies to follow using waypoints.
    /// Provides smooth movement along the path with progress tracking (0-1).
    /// </summary>
    public class Path : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private bool loopPath = false;
        
        private float totalLength;
        private float[] segmentLengths;
        private bool isInitialized;
        
        public float Length => totalLength;
        public int WaypointCount => waypoints?.Length ?? 0;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// Calculate total path length and segment lengths for accurate progress tracking
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;
            
            if (waypoints == null || waypoints.Length < 2)
            {
                Debug.LogError("[Path] Path must have at least 2 waypoints!");
                return;
            }
            
            // Calculate segment lengths
            segmentLengths = new float[waypoints.Length - 1];
            totalLength = 0f;
            
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] == null || waypoints[i + 1] == null)
                {
                    Debug.LogError($"[Path] Waypoint {i} or {i + 1} is null!");
                    continue;
                }
                
                float segmentLength = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
                segmentLengths[i] = segmentLength;
                totalLength += segmentLength;
            }
            
            if (loopPath && waypoints.Length > 1)
            {
                float loopSegment = Vector3.Distance(waypoints[waypoints.Length - 1].position, waypoints[0].position);
                totalLength += loopSegment;
            }
            
            isInitialized = true;
            Debug.Log($"[Path] Initialized with {waypoints.Length} waypoints, total length: {totalLength:F2}");
        }
        
        /// <summary>
        /// Get position along path at given progress (0-1)
        /// </summary>
        public Vector3 GetPositionAtProgress(float progress)
        {
            if (!isInitialized) Initialize();
            
            progress = Mathf.Clamp01(progress);
            
            if (progress >= 1f)
                return waypoints[waypoints.Length - 1].position;
            
            if (progress <= 0f)
                return waypoints[0].position;
            
            // Find which segment we're on
            float targetDistance = progress * totalLength;
            float accumulatedDistance = 0f;
            
            for (int i = 0; i < segmentLengths.Length; i++)
            {
                if (accumulatedDistance + segmentLengths[i] >= targetDistance)
                {
                    // We're on this segment
                    float segmentProgress = (targetDistance - accumulatedDistance) / segmentLengths[i];
                    return Vector3.Lerp(waypoints[i].position, waypoints[i + 1].position, segmentProgress);
                }
                
                accumulatedDistance += segmentLengths[i];
            }
            
            // Fallback to end
            return waypoints[waypoints.Length - 1].position;
        }
        
        /// <summary>
        /// Get direction (normalized) along path at given progress
        /// </summary>
        public Vector3 GetDirectionAtProgress(float progress)
        {
            if (!isInitialized) Initialize();
            
            progress = Mathf.Clamp01(progress);
            
            // Find which segment we're on
            float targetDistance = progress * totalLength;
            float accumulatedDistance = 0f;
            
            for (int i = 0; i < segmentLengths.Length; i++)
            {
                if (accumulatedDistance + segmentLengths[i] >= targetDistance)
                {
                    // Direction is from waypoint[i] to waypoint[i+1]
                    Vector3 direction = (waypoints[i + 1].position - waypoints[i].position).normalized;
                    return direction;
                }
                
                accumulatedDistance += segmentLengths[i];
            }
            
            // Fallback: direction of last segment
            if (waypoints.Length >= 2)
            {
                return (waypoints[waypoints.Length - 1].position - waypoints[waypoints.Length - 2].position).normalized;
            }
            
            return Vector3.right;
        }
        
        /// <summary>
        /// Add a waypoint at runtime (useful for procedural paths)
        /// </summary>
        public void AddWaypoint(Transform waypoint)
        {
            var list = new List<Transform>(waypoints ?? new Transform[0]);
            list.Add(waypoint);
            waypoints = list.ToArray();
            isInitialized = false;
            Initialize();
        }
        
        /// <summary>
        /// Get waypoint by index
        /// </summary>
        public Vector3 GetWaypoint(int index)
        {
            if (index < 0 || index >= waypoints.Length)
                return Vector3.zero;
            
            return waypoints[index].position;
        }
        
        // ===== EDITOR VISUALIZATION =====
        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length < 2)
                return;
            
            // Draw waypoints as spheres
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                    
                    // Draw index label (approximation)
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 0.5f, $"WP {i}");
                    #endif
                }
            }
            
            // Draw path segments
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    
                    // Draw direction arrow
                    Vector3 direction = (waypoints[i + 1].position - waypoints[i].position).normalized;
                    Vector3 midpoint = (waypoints[i].position + waypoints[i + 1].position) * 0.5f;
                    DrawArrow(midpoint, direction, 0.5f);
                }
            }
            
            // Draw loop connection
            if (loopPath && waypoints.Length > 1)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            }
        }
        
        private void DrawArrow(Vector3 position, Vector3 direction, float size)
        {
            Vector3 right = Vector3.Cross(direction, Vector3.forward).normalized * size * 0.3f;
            Vector3 arrowTip = position + direction * size * 0.5f;
            
            Gizmos.DrawLine(position, arrowTip);
            Gizmos.DrawLine(arrowTip, arrowTip - direction * size * 0.3f + right);
            Gizmos.DrawLine(arrowTip, arrowTip - direction * size * 0.3f - right);
        }
    }
}
