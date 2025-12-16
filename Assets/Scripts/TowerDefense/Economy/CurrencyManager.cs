using UnityEngine;
using System;

namespace Game.TowerDefense.Economy
{
    /// <summary>
    /// Manages the player's currency (Credits).
    /// Handles earning, spending, and persistence (session only for now).
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [SerializeField] private int startingCredits = 500;
        
        private int currentCredits;

        // Events
        public event Action<int> OnCreditsChanged; // newAmount

        public int Credits => currentCredits;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            ResetCredits();
        }

        public void ResetCredits()
        {
            currentCredits = startingCredits;
            OnCreditsChanged?.Invoke(currentCredits);
        }

        public void Add(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("[CurrencyManager] Cannot add negative credits. Use Spend() instead.");
                return;
            }

            currentCredits += amount;
            OnCreditsChanged?.Invoke(currentCredits);
            // Debug.Log($"[Economy] Earned {amount} credits. Total: {currentCredits}");
        }

        public bool Spend(int amount)
        {
            if (amount < 0) return false;

            if (currentCredits >= amount)
            {
                currentCredits -= amount;
                OnCreditsChanged?.Invoke(currentCredits);
                // Debug.Log($"[Economy] Spent {amount} credits. Remaining: {currentCredits}");
                return true;
            }
            
            Debug.Log("[Economy] Not enough credits!");
            return false;
        }

        public bool HasEnough(int amount)
        {
            return currentCredits >= amount;
        }
    }
}
