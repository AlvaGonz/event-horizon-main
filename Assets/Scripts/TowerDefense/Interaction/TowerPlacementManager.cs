using UnityEngine;
using Game.TowerDefense.Economy;
using Game.TowerDefense.Towers;

namespace Game.TowerDefense.Interaction
{
    /// <summary>
    /// Handles tower placement logic: Raycasting, Ghost Preview, Validity Checks, and Spawning.
    /// </summary>
    public class TowerPlacementManager : MonoBehaviour
    {
        public static TowerPlacementManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private LayerMask invalidPlacementLayers; // e.g. Path, Obstacles, Other Towers
        
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer ghostRenderer;
        [SerializeField] private Color uniqueValidColor = new Color(0, 1, 0, 0.5f);
        [SerializeField] private Color uniqueInvalidColor = new Color(1, 0, 0, 0.5f);

        private TowerConfig currentTowerToBuild;
        private bool isPlacing = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (ghostRenderer != null)
                ghostRenderer.gameObject.SetActive(false);
        }
        
        private void Start()
        {
            if (gridSystem == null) 
                gridSystem = GridSystem.Instance;
        }

        /// <summary>
        /// Enter placement mode for a specific tower
        /// </summary>
        public void StartPlacing(TowerConfig towerConfig)
        {
            // Check affordability first
            if (CurrencyManager.Instance != null && !CurrencyManager.Instance.HasEnough(towerConfig.buildCost))
            {
                Debug.Log("Not enough credits!");
                return;
            }

            currentTowerToBuild = towerConfig;
            isPlacing = true;
            
            if (ghostRenderer != null)
            {
                ghostRenderer.gameObject.SetActive(true);
                // Try to set ghost sprite from tower config if available (assuming tower has a sprite or we pick one)
                // Since TowerConfig is scriptable object for stats, we might not have visual there directly unless we add it. 
                // For now, let's assume we use a generic placeholder or the tower prefab's sprite if accessible.
                // Simpler: Just use a generic circle sprite for ghost or keep whatever is assigned.
            }
        }
        
        public void CancelPlacing()
        {
            isPlacing = false;
            currentTowerToBuild = null;
            if (ghostRenderer != null) 
                ghostRenderer.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isPlacing || currentTowerToBuild == null) return;

            // Handle Input
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacing();
                return;
            }

            HandlePlacement();
        }

        private void HandlePlacement()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Snap to grid
            Vector3 buildPos = mousePos;
            if (gridSystem != null)
            {
                buildPos = gridSystem.SnapToGrid(mousePos);
            }
            
            // Update Ghost Position
            if (ghostRenderer != null)
            {
                ghostRenderer.transform.position = buildPos;
            }

            // Check Validity
            bool isValid = IsPositionValid(buildPos);
            UpdateGhostColor(isValid);

            // Handle Click
            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                {
                    PlaceTower(buildPos);
                }
                else
                {
                    Debug.Log("Invalid placement position!");
                }
            }
        }

        private bool IsPositionValid(Vector3 position)
        {
            // 1. Overlap Check (Physics 2D)
            Collider2D col = Physics2D.OverlapCircle(position, 0.4f, invalidPlacementLayers);
            if (col != null)
            {
                // Debug.Log($"Blocked by {col.gameObject.name}");
                return false;
            }

            // 2. Map Bounds Check (Optional, could add later)
            
            return true;
        }
        
        private void UpdateGhostColor(bool isValid)
        {
            if (ghostRenderer != null)
            {
                ghostRenderer.color = isValid ? uniqueValidColor : uniqueInvalidColor;
            }
        }

        private void PlaceTower(Vector3 position)
        {
            if (currentTowerToBuild == null) return;

            // Double check credits
            if (CurrencyManager.Instance != null && !CurrencyManager.Instance.Spend(currentTowerToBuild.buildCost))
            {
                Debug.Log("Insufficient credits at moment of placement!");
                CancelPlacing();
                return;
            }

            // Instantiate
            if (currentTowerToBuild.prefab != null)
            {
                GameObject towerObj = Instantiate(currentTowerToBuild.prefab, position, Quaternion.identity);
                Tower tower = towerObj.GetComponent<Tower>();
                if (tower != null)
                {
                    tower.Initialize(currentTowerToBuild);
                }
            }
            else
            {
                Debug.LogError("No prefab in TowerConfig!");
            }

            // Done placement (or continue placing if SHIFT held? Basic for now)
            CancelPlacing();
        }
    }
}
