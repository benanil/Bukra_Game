using AnilTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Inventory
{
    public class UIInventory : Singleton<UIInventory>
    {
        public List<UIItem> itemSlots = new List<UIItem>();
        public GameObject slotprefab;
        public Transform slotPanel;

        public int FullSlots{
            get{
                var fullSlots = 0;

                for (int i = 0; i < itemSlots.Count; i++)
                    if (itemSlots[i].item != null) fullSlots++;

                return fullSlots;
            }
        }

        public byte SlotSayısı
        {
            get{
                if ((byte)itemSlots.Count == 0){
                    return 10;
                }
                return (byte)itemSlots.Count;
            }
        }

        [SerializeField]
        private bool isCrafting;

        public void AddSlot(int SlotSayısı)
        {
            for (byte i = 0; i < SlotSayısı; i++){
                var instance = Instantiate(slotprefab, transform);
                var inscode = instance.GetComponentInChildren<UIItem>();
                inscode.Spawn(i);
                instance.transform.SetParent(transform);
                instance.transform.localScale = Vector3.one;
                itemSlots.Add(inscode);
            }
        }

        public void RemoveAllSlots()
        {
            foreach (var item in itemSlots){
                Destroy(item);
            }
        }


        public void AddNewItem(IItem item)
        {
            Debug2.Log("item type: " + item.GetType());

            // item slotlarından uygun olan varmı kontrol
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].item != null)
                    if (itemSlots[i].Count < 16 && itemSlots[i].type == item.GetType())
                    {
                        UpdateSlot(i, item);
                        return;
                    }
            }

            for (int i = 0; i < itemSlots.Count; i++){
                if (itemSlots[i].HasItem == false){
                    UpdateSlot(i, item);
                    return;
                }
            }

        }

        private void UpdateSlot(int slot, IItem item)
        {
            try
            {
                itemSlots[slot].UpdateItem(item);
            }
            catch {
                throw new System.Exception("slot upadtemedi " +  item + "slot :" + slot);
            }
        }       
        public void RemoveItem(IItem item)
        {
            itemSlots.ForEachBreak(x =>
            {
                if (x.item is ItemSC xitem){
                    if (xitem.id() == item.id()){
                        x.UpdateItem(null);
                        return true;
                    }
                }
                return false;
            });
        }

        public void RefreshInventory()
        {
            var ActiveItems = new List<object>(itemSlots.Count);
            byte addedItems = 0;

            itemSlots.ForEach(item =>
            {
                RemoveItem((IItem)ActiveItems[addedItems]);
                ActiveItems.Add(itemSlots[addedItems].item);
                addedItems++;
            });

            ActiveItems.ForEach(item =>
            {
                 AddNewItem((IItem)item);
            });
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIInventory))]
    public class UıInventoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var _target = (UIInventory)target;
            EditorGUILayout.LabelField("Slot Sayısı" + _target.SlotSayısı);
        }
    }
#endif

}