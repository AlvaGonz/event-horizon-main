using UnityEngine;

namespace Game.TowerDefense
{
    /// <summary>
    /// ScriptableObject configuration for enemy types
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Tower Defense/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Identity")]
        public EnemyType enemyType = EnemyType.Fighter;
        public string enemyName = "Fighter";
        
        [Header("Stats")]
        [Tooltip("Base health (scaled by wave difficulty)")]
        public float maxHealth = 20f;
        
        [Tooltip("Base movement speed (scaled by wave difficulty)")]
        public float speed = 3f;
        
        [Tooltip("Credits rewarded when killed")]
        public int killReward = 10;
        
        [Header("Visual")]
        [Tooltip("Sprite for this enemy type")]
        public Sprite sprite;
        
        [Tooltip("Scale of the sprite")]
        public float spriteScale = 1f;
        
        [Header("Behavior")]
        [Tooltip("Damage dealt to base when reaching end")]
        public float baseDamage = 5f;
    }
}
