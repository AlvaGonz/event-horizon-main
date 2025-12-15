using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// ScriptableObject configuration for base station
    /// </summary>
    [CreateAssetMenu(fileName = "BaseConfig", menuName = "Tower Defense/Base Config")]
    public class BaseConfig : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("Starting health of the base")]
        public float maxHealth = 100f;
        
        [Header("Visual")]
        public Sprite baseSprite;
        public Color healthyColor = Color.green;
        public Color criticalColor = Color.red;
        
        [Tooltip("Health percentage at which base enters critical state")]
        [Range(0f, 1f)]
        public float criticalHealthPercent = 0.25f;
    }
}
