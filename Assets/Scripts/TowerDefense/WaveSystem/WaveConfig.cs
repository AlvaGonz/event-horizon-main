using UnityEngine;
using System.Collections.Generic;

namespace Game.TowerDefense
{
    /// <summary>
    /// ScriptableObject configuration for all waves in a tower defense session
    /// </summary>
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Tower Defense/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("All waves in this configuration")]
        public List<Wave> waves = new List<Wave>();
        
        [Header("Difficulty Scaling")]
        [Tooltip("Base multiplier increase per wave (1.15 = +15% per wave)")]
        public float difficultyMultiplierPerWave = 1.15f;
        
        [Tooltip("Base reward multiplier increase per wave")]
        public float rewardMultiplierPerWave = 1.1f;
        
        [Header("Timing")]
        [Tooltip("Delay between wave completion and next wave start")]
        public float delayBetweenWaves = 3f;
        
        [Tooltip("Grace period after kill target reached to clear remaining enemies")]
        public float gracePeriodAfterWaveComplete = 5f;
        
        /// <summary>
        /// Auto-generate default waves for testing
        /// </summary>
        [ContextMenu("Generate Default Waves")]
        public void GenerateDefaultWaves()
        {
            waves.Clear();
            
            for (int i = 0; i < 20; i++)
            {
                Wave wave = new Wave
                {
                    waveNumber = i + 1,
                    targetEnemyCount = 15 + (i * 3), // Increasing kill target
                    spawnRate = 1f + (i * 0.1f), // Increasing spawn rate
                    spawnDelay = 0f,
                    difficultyMultiplier = Mathf.Pow(difficultyMultiplierPerWave, i),
                    rewardMultiplier = Mathf.Pow(rewardMultiplierPerWave, i),
                    enemyTypes = new List<EnemySpawnData>()
                };
                
                waves.Add(wave);
            }
            
            Debug.Log($"Generated {waves.Count} default waves");
        }
    }
    
    /// <summary>
    /// Configuration for a single wave
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        public int waveNumber;
        
        [Header("Completion Criteria")]
        [Tooltip("Number of enemies that must be killed to complete this wave")]
        public int targetEnemyCount = 15;
        
        [Header("Spawn Configuration")]
        [Tooltip("Enemy types and their spawn frequencies")]
        public List<EnemySpawnData> enemyTypes = new List<EnemySpawnData>();
        
        [Tooltip("Enemies spawned per second")]
        public float spawnRate = 1f;
        
        [Tooltip("Delay before first enemy spawns")]
        public float spawnDelay = 0f;
        
        [Header("Difficulty")]
        [Tooltip("Multiplier for enemy health and speed")]
        public float difficultyMultiplier = 1f;
        
        [Tooltip("Multiplier for kill rewards")]
        public float rewardMultiplier = 1f;
    }
    
    /// <summary>
    /// Configuration for enemy spawning in a wave
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
        
        [Tooltip("Weight in spawn pool (higher = more likely)")]
        [Range(1, 100)]
        public int spawnWeight = 10;
    }
}
