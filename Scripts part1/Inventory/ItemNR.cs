using Rock;
using System;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public abstract class ItemNR : IItem
    {
        public byte Id;
        public short InventoryId;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public ItemClass currentClass;
        [NonSerialized]
        public string description;
        public PoolObject poolName;
        public HandState handState;

        public Sprite icon;
        public short Price;
        public const bool DefaultTrade = false; // itemi satmaya yarar
               // hand
        [Space]
        public HandPosition HandPosition;
        public Mesh mesh;
        
        public LayerMask Damagables;

        // rock
        public RockPack rockPack;

        public bool IsHandTool => currentClass == ItemClass.HandTool;

        public void ChangeLanguage(ItemStrings strings)
        {
            Title = strings.Title;
            description = strings.description;
        }

        public static T CastItem<T>(object Item)
        {
            T x = (T)Item;
            return x;
        }

        public byte id()
        {
            return Id;
        }

        public RockPack RockPack()
        {
            return rockPack;
        }

        Sprite IItem.icon()
        {
            return icon;
        }
    }
}