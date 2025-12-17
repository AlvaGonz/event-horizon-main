using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// ScriptableObject configuration for tower types
    /// </summary>
    [CreateAssetMenu(fileName = "TowerConfig", menuName = "Tower Defense/Tower Config")]
    public class TowerConfig : ScriptableObject
    {
        [Header("Identity")]
        public GameObject prefab;
        public string towerName = "Basic Tower";
        public string description = "A basic defensive tower";
        
        [Header("Combat Stats")]
        [Tooltip("Detection and attack range")]
        public float range = 5f;
        
        [Tooltip("Base damage per shot")]
        public float damage = 10f;
        
        [Tooltip("Attacks per second")]
        public float fireRate = 1f;
        
        [Header("Economy")]
        [Tooltip("Cost to build this tower")]
        public int buildCost = 100;
        
        [Tooltip("Cost for each upgrade level")]
        public int[] upgradeCosts = new int[] { 50, 100, 200, 400, 800 };
        
        [Tooltip("Sell value (percentage of total investment)")]
        [Range(0f, 1f)]
        public float sellValuePercent = 0.7f;
        
        [Header("Upgrades")]
        [Tooltip("Damage multiplier per upgrade level")]
        public float damageMultiplierPerLevel = 1.25f;
        
        [Tooltip("Range bonus per upgrade level")]
        public float rangeBonusPerLevel = 0.5f;
        
        [Tooltip("Fire rate multiplier per upgrade level")]
        public float fireRateMultiplierPerLevel = 1.1f;
        
        [Tooltip("Maximum upgrade level")]
        public int maxUpgradeLevel = 5;
        
        [Header("Visual")]
        public Sprite towerSprite;
        public Color rangeCircleColor = new Color(1f, 1f, 0f, 0.3f);
        
        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip upgradeSound;
    }
}
