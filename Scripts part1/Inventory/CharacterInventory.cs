using AnilTools;
using System.Collections.Generic;
using UnityEditor;

namespace Inventory
{
    public class CharacterInventory : Singleton<CharacterInventory>
    {
        private UIInventory inventoryuı => UIInventory.instance;

        public static List<object> characterItems = new List<object>();

        internal static List<ItemData> InventoryItems {
            get {
                var Itemdatas = new List<ItemData>();

                for (byte i = 0; i < ItemDatabase.instance.itemSCs.Count; i++)
                {
                    var item = ItemDatabase.instance.itemSCs[i];

                    Itemdatas.Add(new ItemData(item.Id, GetCount(item)));
                }

                return Itemdatas;
            }
        }

        internal static List<ItemNR> SpecialItems {
            get {
                var list = new List<ItemNR>();

                for (short i = 0; i < characterItems.Count; i++)
                    if (characterItems[i] is ItemNR itemNR)
                        list.Add(itemNR);
                return list;
            }
        }

        public int AllItemsCount {
            get => InventoryItems.Count;
        }

        public void RemoveAllItems() {
            characterItems.Clear();

            for (short i = 0; i < inventoryuı.itemSlots.Count; i++) {
                if (inventoryuı.itemSlots[i].item is ItemSC itemSC) {
                    inventoryuı.itemSlots[i].UpdateItem(itemSC);
                }
                else if (inventoryuı.itemSlots[i].item is ItemNR itemNR) {
                    inventoryuı.itemSlots[i].UpdateItem(itemNR);
                }
            }
        }

        public void AddItem(short id)
        {
            var item = ItemDatabase.instance.GetItem(id);

            characterItems.Add(item);
            inventoryuı.AddNewItem(item);
        }

        public void AddItem(IItem itemToAdd)
        {
            characterItems.Add(itemToAdd);
            inventoryuı.AddNewItem(itemToAdd);
        }

        /// <summary>
        /// sc için kullanılır
        /// </summary>
        public void RemoveItem(byte id)
        {
            IItem _item = CheckForItem(id);

            if (_item != null)
            {
                characterItems.Remove(_item);
                inventoryuı.RemoveItem(_item);
            }
        }

        /// <summary>
        /// nr için kullanılır
        /// </summary>
        public void RemoveItem(object itemNR)
        {
            characterItems.Remove(itemNR);
            inventoryuı.RemoveItem((IItem)itemNR);
        }

        public void RemoveItemInventory(ItemNR itemNR)
        {
            if (itemNR != null)
            {
                characterItems.Remove(itemNR);
                inventoryuı.RemoveItem(itemNR);
            }
        }

        public IItem CheckForItem(byte id)
        {
            for (short i = 0; i < characterItems.Count; i++)
                if (characterItems[i] is IItem t)
                    if (t.id() == id)
                        return t;

            return default;
        }

        public bool CheckForItemExist(byte id)
        {
            for (short i = 0; i < characterItems.Count; i++)
                if (characterItems[i] is ItemNR t)
                    if (t.id() == id)
                        return true;

            return false;
        }

        public bool CheckForItemExist(params byte[] ids)
        {
            byte ExistItemsCount = 0;

            for (int i = 0; i < ids.Length; i++)
            {
                for (short j = 0; j < characterItems.Count; j++)
                    if (characterItems[j] is ItemNR t)
                        if (t.id() == ids[j])
                            ExistItemsCount++;
            }

            return ExistItemsCount >= ids.Length;
        }

        public IEnumerable<byte> Counts(params IItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                yield return GetCount(items[i]); 
            }
        }

        public static byte GetCount(IItem item)
        {
            byte count = 0;
            for (byte i = 0; i < characterItems.Count; i++)
                if (characterItems[i].Equals(item))
                    count++;
            
            return count;
        }

    }
#if UNITY_EDITOR

    [CustomEditor(typeof(CharacterInventory))]
    public class CharacterInventoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var _target = (CharacterInventory)target;
            EditorGUILayout.LabelField("Character Items Count: " + CharacterInventory.characterItems.Count);
        }
    }
#endif
}
