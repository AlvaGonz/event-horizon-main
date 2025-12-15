using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.TowerDefense
{
    /// <summary>
    /// Manages tower defense waves with kill-count based completion.
    /// Waves complete when X enemies have been killed (not when all enemies are dead).
    /// Enemies spawn continuously until the wave kill target is reached.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }
        
        [Header("Configuration")]
        [SerializeField] private WaveConfig waveConfig;
        [SerializeField] private Path enemyPath;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private BaseStation baseStation;
        
        [Header("Runtime Info")]
        [SerializeField] private int currentWaveIndex = 0;
        [SerializeField] private int currentKillCount = 0;
        [SerializeField] private int targetKillCount = 0;
        [SerializeField] private bool isWaveActive = false;
        
        private Coroutine spawnCoroutine;
        private List<EnemyUnit> activeEnemies = new List<EnemyUnit>();
        
        // ===== EVENTS =====
        public event Action<int> OnWaveStarted;              // waveNumber
        public event Action<int, int> OnKillCountChanged;    // (current, target)
        public event Action<int> OnWaveCompleted;            // waveNumber
        public event Action OnAllWavesCompleted;             // Victory!
        public event Action OnGameOver;                      // Base destroyed
        
        public int CurrentWave => currentWaveIndex;
        public int TotalWaves => waveConfig != null ? waveConfig.waves.Count : 0;
        public int CurrentKills => currentKillCount;
        public int TargetKills => targetKillCount;
        public float WaveProgress => targetKillCount > 0 ? (float)currentKillCount / targetKillCount : 0f;
        public bool IsWaveActive => isWaveActive;
        
        // ===== INITIALIZATION =====
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            if (baseStation != null)
            {
                baseStation.OnDestroyed += HandleBaseDestroyed;
            }
            
            // Auto-start first wave after brief delay
            StartCoroutine(DelayedStart());
        }
        
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(2f);
            StartNextWave();
        }
        
        public void Initialize(WaveConfig config)
        {
            waveConfig = config;
            currentWaveIndex = 0;
            currentKillCount = 0;
        }
        
        // ===== WAVE CONTROL =====
        /// <summary>Start the next wave in sequence</summary>
        public void StartNextWave()
        {
            if (waveConfig == null)
            {
                Debug.LogError("[WaveManager] No wave config assigned!");
                return;
            }
            
            if (currentWaveIndex >= waveConfig.waves.Count)
            {
                Debug.Log("🎉 ALL WAVES COMPLETED! VICTORY!");
                OnAllWavesCompleted?.Invoke();
                return;
            }
            
            Wave wave = waveConfig.waves[currentWaveIndex];
            StartWave(wave);
        }
        
        private void StartWave(Wave wave)
        {
            Debug.Log($"\n▶️ WAVE {wave.waveNumber} START");
            Debug.Log($"   Target Kills: {wave.targetEnemyCount}");
            Debug.Log($"   Difficulty: {wave.difficultyMultiplier:F2}x");
            Debug.Log($"   Spawn Rate: {wave.spawnRate} enemies/sec");
            
            isWaveActive = true;
            currentKillCount = 0;
            targetKillCount = wave.targetEnemyCount;
            
            OnWaveStarted?.Invoke(wave.waveNumber);
            OnKillCountChanged?.Invoke(0, targetKillCount);
            
            // Start continuous enemy spawning
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            spawnCoroutine = StartCoroutine(SpawnWaveEnemies(wave));
        }
        
        // ===== SPAWNING SYSTEM =====
        private IEnumerator SpawnWaveEnemies(Wave wave)
        {
            // Initial delay before first spawn
            if (wave.spawnDelay > 0)
                yield return new WaitForSeconds(wave.spawnDelay);
            
            // Calculate delay between spawns
            float delayBetweenSpawns = wave.spawnRate > 0 ? 1f / wave.spawnRate : 1f;
            int spawnedCount = 0;
            
            while (isWaveActive)
            {
                // Stop spawning if kill target reached
                if (currentKillCount >= targetKillCount)
                {
                    Debug.Log($"✓ Wave kill target reached! ({currentKillCount}/{targetKillCount})");
                    yield return StartCoroutine(CompleteWave(wave));
                    yield break;
                }
                
                // Spawn next enemy
                SpawnEnemy(wave, spawnedCount);
                spawnedCount++;
                
                // Wait for next spawn
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        
        private void SpawnEnemy(Wave wave, int spawnIndex)
        {
            // Validate configuration
            if (wave.enemyTypes == null || wave.enemyTypes.Count == 0)
            {
                Debug.LogError("[WaveManager] Wave has no enemy types configured!");
                return;
            }
            
            // Select random enemy type based on weights
            EnemySpawnData spawnData = SelectRandomEnemyType(wave.enemyTypes);
            
            if (spawnData.enemyPrefab == null)
            {
                Debug.LogError("[WaveManager] Enemy prefab is null!");
                return;
            }
            
            // Calculate spawn position with slight randomization
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            spawnPos += (Vector3)UnityEngine.Random.insideUnitCircle * 0.5f;
            
            // Instantiate enemy
            GameObject enemyGo = Instantiate(spawnData.enemyPrefab, spawnPos, Quaternion.identity);
            EnemyUnit enemy = enemyGo.GetComponent<EnemyUnit>();
            
            if (enemy != null)
            {
                // Apply difficulty scaling
                float healthMultiplier = wave.difficultyMultiplier;
                float speedMultiplier = 1f + ((wave.difficultyMultiplier - 1f) * 0.5f); // Speed scales slower
                
                // Set enemy configuration
                if (enemy.Config != null)
                {
                    enemy.SetConfig(enemy.Config, healthMultiplier, speedMultiplier);
                }
                
                // Subscribe to events
                enemy.OnDeath += HandleEnemyKilled;
                enemy.OnReachedEnd += HandleEnemyReachedEnd;
                
                // Set path
                if (enemyPath != null)
                {
                    enemy.SetPath(enemyPath);
                }
                else
                {
                    Debug.LogWarning("[WaveManager] No enemy path assigned!");
                }
                
                // Track enemy
                activeEnemies.Add(enemy);
                
                Debug.Log($"   Spawned: {spawnData.enemyType} (#{spawnIndex + 1})");
            }
            else
            {
                Debug.LogError("[WaveManager] Spawned prefab has no EnemyUnit component!");
                Destroy(enemyGo);
            }
        }
        
        private EnemySpawnData SelectRandomEnemyType(List<EnemySpawnData> enemyTypes)
        {
            // Calculate total weight
            int totalWeight = enemyTypes.Sum(e => e.spawnWeight);
            
            // Select random value
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            
            // Find corresponding enemy type
            int currentWeight = 0;
            foreach (var enemy in enemyTypes)
            {
                currentWeight += enemy.spawnWeight;
                if (randomValue < currentWeight)
                    return enemy;
            }
            
            // Fallback to first
            return enemyTypes[0];
        }
        
        // ===== EVENT HANDLERS =====
        private void HandleEnemyKilled(EnemyUnit enemy)
        {
            currentKillCount++;
            OnKillCountChanged?.Invoke(currentKillCount, targetKillCount);
            
            // Remove from active list
            activeEnemies.Remove(enemy);
            
            Debug.Log($"   💀 Enemy killed. Kills: {currentKillCount}/{targetKillCount}");
        }
        
        private void HandleEnemyReachedEnd(EnemyUnit enemy, float baseDamage)
        {
            // Deal damage to base
            if (baseStation != null)
            {
                baseStation.TakeDamage(baseDamage);
            }
            
            // Remove from active list
            activeEnemies.Remove(enemy);
            
            Debug.LogWarning($"[WaveManager] Enemy reached base! Dealt {baseDamage} damage.");
        }
        
        private void HandleBaseDestroyed()
        {
            Debug.LogError("[WaveManager] BASE DESTROYED! GAME OVER!");
            
            // Stop spawning
            isWaveActive = false;
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            OnGameOver?.Invoke();
        }
        
        // ===== WAVE COMPLETION =====
        private IEnumerator CompleteWave(Wave wave)
        {
            isWaveActive = false;
            
            // Stop spawning
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            Debug.Log($"✓ Wave {wave.waveNumber} kill target reached!");
            
            // Grace period: allow remaining enemies to be cleared
            float gracePeriod = waveConfig != null ? waveConfig.gracePeriodAfterWaveComplete : 5f;
            Debug.Log($"⏱️ Grace period: {gracePeriod} seconds for cleanup...");
            
            float gracePeriodTimer = gracePeriod;
            while (gracePeriodTimer > 0)
            {
                // Clean up null references
                activeEnemies.RemoveAll(e => e == null || e.IsDead);
                
                // Check if all enemies are cleared
                if (activeEnemies.Count == 0)
                {
                    Debug.Log("✓ All enemies eliminated!");
                    break;
                }
                
                gracePeriodTimer -= Time.deltaTime;
                yield return null;
            }
            
            // Wave is complete
            Debug.Log($"✓ WAVE {wave.waveNumber} COMPLETE\n");
            OnWaveCompleted?.Invoke(wave.waveNumber);
            
            // Increment wave index
            currentWaveIndex++;
            
            // Delay before next wave
            float delayBetween = waveConfig != null ? waveConfig.delayBetweenWaves : 3f;
            yield return new WaitForSeconds(delayBetween);
            
            // Start next wave
            StartNextWave();
        }
        
        // ===== UTILITY =====
        public Wave GetCurrentWave()
        {
            if (waveConfig == null || currentWaveIndex >= waveConfig.waves.Count)
                return null;
            
            return waveConfig.waves[currentWaveIndex];
        }
        
        // ===== CLEANUP =====
        private void OnDestroy()
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            
            if (baseStation != null)
            {
                baseStation.OnDestroyed -= HandleBaseDestroyed;
            }
        }
        
        // ===== DEBUG =====
        private void OnDrawGizmos()
        {
            // Draw spawn point
            if (spawnPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                Gizmos.DrawRay(spawnPoint.position, Vector3.up * 2f);
            }
        }
    }
}
