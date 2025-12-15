using UnityEngine;

namespace Game.TowerDefense.Interaction
{
    /// <summary>
    /// Utility for grid-based coordinate system.
    /// Converts between World Positions and Grid Coordinates.
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; }
        
        [Header("Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector2 gridOrigin = Vector2.zero;
        [SerializeField] private bool showDebugGrid = true;
        [SerializeField] private int debugGridSize = 20;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Snaps a world position to the nearest grid cell center
        /// </summary>
        public Vector3 SnapToGrid(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            return GridToWorld(gridPos);
        }

        /// <summary>
        /// Converts World Position to Grid Coordinates
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize);
            int y = Mathf.RoundToInt((worldPosition.y - gridOrigin.y) / cellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Converts Grid Coordinates to World Position (Center of cell)
        /// </summary>
        public Vector3 GridToWorld(Vector2Int gridPosition)
        {
            float x = gridPosition.x * cellSize + gridOrigin.x;
            float y = gridPosition.y * cellSize + gridOrigin.y;
            return new Vector3(x, y, 0f);
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGrid) return;

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            
            // Draw vertical lines
            for (int x = -debugGridSize; x <= debugGridSize; x++)
            {
                float xPos = x * cellSize + gridOrigin.x;
                Vector3 start = new Vector3(xPos, -debugGridSize * cellSize + gridOrigin.y, 0f);
                Vector3 end = new Vector3(xPos, debugGridSize * cellSize + gridOrigin.y, 0f);
                Gizmos.DrawLine(start, end);
            }

            // Draw horizontal lines
            for (int y = -debugGridSize; y <= debugGridSize; y++)
            {
                float yPos = y * cellSize + gridOrigin.y;
                Vector3 start = new Vector3(-debugGridSize * cellSize + gridOrigin.x, yPos, 0f);
                Vector3 end = new Vector3(debugGridSize * cellSize + gridOrigin.x, yPos, 0f);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
