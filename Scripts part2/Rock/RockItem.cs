
using Inventory;
using UnityEngine;

namespace Rock
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SwordRock", menuName = "Items/Rocks/RockItem")]
    public class RockItem : ItemSC , IItem
    {
        public RockType rockType;

        [Space]
        public float damageReducing;

        [Space]
        public int damage;
        public float AttackSpeed;
        public bool HasEffect;

        public bool DefaultOrNull()
        {
            return this == null || this == default;
        }

        public bool IsArmor => rockType == RockType.armor;
        public bool IsSword => rockType == RockType.armor;
    }

    public enum RockType
    {
        armor, sword
    }
}