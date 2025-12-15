using UnityEngine;
using UnityEngine.UI;
using Game.TowerDefense.Economy;
using Game.TowerDefense.BaseDefense;
using Game.TowerDefense.WaveSystem;

namespace Game.TowerDefense.UI
{
    /// <summary>
    /// Manages the top-level HUD, displaying health, currency, and wave info.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private Text creditsText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text waveText;
        [SerializeField] private Text killProgressText;

        [Header("References")]
        [SerializeField] private BaseStation baseStation;
        [SerializeField] private WaveManager waveManager;
        
        private void Start()
        {
            // Subscribe to Economy
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCreditsChanged += UpdateCredits;
                UpdateCredits(CurrencyManager.Instance.Credits);
            }

            // Subscribe to Base
            if (baseStation != null)
            {
                baseStation.OnHealthChanged += UpdateHealth;
                UpdateHealth(baseStation.HealthPercent);
            }

            // Subscribe to Wave
            if (waveManager != null)
            {
                waveManager.OnWaveStarted += UpdateWaveInfo;
                waveManager.OnKillCountChanged += UpdateKillProgress;
            }
        }

        private void UpdateCredits(int amount)
        {
            if (creditsText != null)
                creditsText.text = $"Credits: {amount}";
        }

        private void UpdateHealth(float percent)
        {
            if (healthText != null)
            {
                // Can display percentage or absolute if base reference available
                float current = baseStation != null ? baseStation.Health : 0;
                float max = baseStation != null ? baseStation.MaxHealth : 100;
                healthText.text = $"Base Integrity: {Mathf.CeilToInt(current)}/{max}";
                healthText.color = Color.Lerp(Color.red, Color.green, percent);
            }
        }

        private void UpdateWaveInfo(int waveIndex)
        {
            if (waveText != null)
                waveText.text = $"Wave: {waveIndex}";
        }

        private void UpdateKillProgress(int current, int target)
        {
            if (killProgressText != null)
                killProgressText.text = $"Progress: {current}/{target}";
        }

        private void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCreditsChanged -= UpdateCredits;
                
            if (baseStation != null)
                baseStation.OnHealthChanged -= UpdateHealth;
                
            if (waveManager != null)
            {
                waveManager.OnWaveStarted -= UpdateWaveInfo;
                waveManager.OnKillCountChanged -= UpdateKillProgress;
            }
        }
    }
}
