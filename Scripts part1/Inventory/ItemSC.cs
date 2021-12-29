using Rock;
using UnityEngine;
using System;
public enum ItemClass
{
    trade, Eating, Armor, HandTool, potion, crafting,
    None,rock,bow
}

public enum HandPosition
{ 
    left,right,Double
}

namespace Inventory
{ 
    [CreateAssetMenu(menuName = "Items/Trade")]
    public class ItemSC : ScriptableObject , IItem , IEquatable<ItemNR>
    {
        public byte Id => (byte)poolName;
        [NonSerialized]
        public string Title;
        [NonSerialized]
        public string description;
        [NonSerialized] 
        public short InventoryId;
        public HandState handState;
        public ItemClass currentClass;
        public PoolObject poolName;
        
        public Sprite icon;
        public short Price;
        public bool DefaultTrade = true; // itemi satmaya yarar
        // hand
        [Space]
        public HandPosition HandPosition;

        public Mesh mesh;
        public LayerMask Damagables;
        // rock
        public RockPack rockPack;

        // inspector
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

        public bool Equals(ItemNR other)
        {
            return Id == other.Id;
        }
    }

    [Serializable]
    public struct ItemStrings
    {
        public ItemSC item;
        public string Title;
        [Multiline]
        public string description;

    }

}
