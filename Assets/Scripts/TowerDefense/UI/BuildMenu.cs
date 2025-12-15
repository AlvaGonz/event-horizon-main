using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.TowerDefense.Towers;
using Game.TowerDefense.Interaction;
using Game.TowerDefense.Economy;

namespace Game.TowerDefense.UI
{
    /// <summary>
    /// Simple menu with buttons to select towers to build.
    /// </summary>
    public class BuildMenu : MonoBehaviour
    {
        [System.Serializable]
        public struct BuildButton
        {
            public string name;
            public TowerConfig config;
            public Button button;
            public Text costText;
        }

        [SerializeField] private List<BuildButton> buildButtons;

        private void Start()
        {
            // Initialize buttons
            foreach (var item in buildButtons)
            {
                if (item.button != null && item.config != null)
                {
                    item.button.onClick.AddListener(() => OnBuildButtonClicked(item.config));
                    if (item.costText != null)
                        item.costText.text = item.config.buildCost.ToString();
                }
            }

            // Subscribe to updates for interactivity
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCreditsChanged += UpdateButtonStates;
                UpdateButtonStates(CurrencyManager.Instance.Credits); // Initial check
            }
        }

        private void OnBuildButtonClicked(TowerConfig config)
        {
            if (TowerPlacementManager.Instance != null)
            {
                TowerPlacementManager.Instance.StartPlacing(config);
            }
        }

        private void UpdateButtonStates(int currentCredits)
        {
            foreach (var item in buildButtons)
            {
                if (item.button != null && item.config != null)
                {
                    bool canAfford = currentCredits >= item.config.buildCost;
                    item.button.interactable = canAfford;
                    
                    // Optional: Change text color
                    if (item.costText != null)
                        item.costText.color = canAfford ? Color.white : Color.red;
                }
            }
        }

        private void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCreditsChanged -= UpdateButtonStates;
        }
    }
}
