
using Inventory;
using UnityEngine;

namespace Crafting
{
    [CreateAssetMenu(menuName = "Crafting/Craft", fileName = "Craft")]
    public class Craft : ScriptableObject
    {
        public ItemSC[] items;
        public byte[] counts;
        public ItemSC ProductItem;
        public byte PruductItemCount;

        public byte[] ItemIDs()
        {
            var itemsIds = new byte[items.Length];
            
            for (int i = 0; i < items.Length; i++)
            {
                itemsIds[i] = items[i].Id;
            }

            return itemsIds;
        }
    }
}
