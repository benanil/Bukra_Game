using Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace Rock
{
    public class RockDatabase : MonoBehaviour
    {
        private static RockDatabase _instance;
        public static RockDatabase instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RockDatabase>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// İNT: inventoryID , İTEM: armor, rock, sword
        /// </summary>
        internal List<RockData> RockDatas = new List<RockData>();

        internal List<GameObject> rockSlots = new List<GameObject>();
        public GameObject prefab;

        private object SavedSword => PlayerInfo.instance.HandInfo.savedItem;
        private object SavedArmor => PlayerInfo.instance.ArmorInfo.savedItem;

        // EXİCUTİON ORDER ARTTIR

        public void Start()
        {
            AddItemsToScrol();
        }

        public void UpdateUI()
        {
            AddItemsToScrol();
        }

        private void RemoveItems()
        {
            for (int i = 0; i < rockSlots.Count; i++)
            {
                Destroy(rockSlots[i]);
            }

            rockSlots.Clear();
            RockDatas.Clear();
        }

        private void AddItemsToScrol()
        {
            RemoveItems();

            for (short i = 0; i < CharacterInventory.characterItems.Count; i++)
            {
                var item = CharacterInventory.characterItems[i];

                if (item is ItemSC itemSC)
                {
                    if (itemSC.currentClass == ItemClass.Armor
                     || itemSC.currentClass == ItemClass.rock
                     || itemSC.handState == HandState.Sword)
                    {
                        RockDatas.Add(new RockData(i, itemSC));
                    }
                }
            }

            for (short i = 0; i < CharacterInventory.SpecialItems.Count; i++)
            {
                RockDatas.Add(new RockData(i, CharacterInventory.SpecialItems[i]));
            }

            if (SavedArmor != null) // en bastaki 2 seyin giydiğimiz zırh ve kılıç olması için
            {
                //var id = CharacterInventory.instance.characterItems.Count;
                RockDatas.Add(new RockData(61, SavedArmor));
            }

            if (SavedSword != null)
            {
                //var id = CharacterInventory.instance.characterItems.Count + 1;
                RockDatas.Add(new RockData(62, SavedSword));
            }

            for (short i = 0; i < RockDatas.Count; i++) // envanterdeki diğer klc tas ve zrh lar
            {
                var pref = Instantiate(prefab, transform);
                var prefComp = pref.GetComponent<RockSlot>();
                pref.transform.SetParent(transform);
                prefComp.Init(RockDatas[i].Item,i);
                rockSlots.Add(prefComp.gameObject);
            }
        }
    }

    [System.Serializable]
    public struct RockData
    {
        public short Inventoryid;
        public object Item;

        public RockData(short inventoryid, object item)
        {
            Inventoryid = inventoryid;
            Item = item;
        }
    }

}