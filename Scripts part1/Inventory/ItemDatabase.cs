using AnilTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory
{
    public class ItemDatabase : Singleton<ItemDatabase>
    {

        private List<ItemSC> _itemSCs;

        public List<ItemSC> itemSCs
        {
            get
            {
                if (_itemSCs == null)
                {
                    _itemSCs = Resources.FindObjectsOfTypeAll<ItemSC>().ToList();
                    _itemSCs.OrderBy(x => x.Id);
                }

                return _itemSCs;
            }
        }

        private List<ItemNR> _itemNRs;

        public List<ItemNR> itemNRs
        {
            get
            {
                if (_itemNRs == null)
                {
                    _itemNRs = new List<ItemNR>(); 

                    for (short i = 0; i < CharacterInventory.characterItems.Count; i++)
                    {
                        if (CharacterInventory.characterItems[i] is ItemNR itemNR)
                        {
                            _itemNRs.Add(itemNR);
                        }
                    }
                }

                return _itemNRs;
            }
        }

        public List<Object> ItemDeneme;

        public ItemSC GetItem(short id)
        {
            ItemSC item = itemSCs.Find(x => x.Id == id);

            if (item == null){
                Debug2.Log(id,Color.red);
            }

            return item;
        }

        public ItemNR GetItem(ItemNR itemNR)
        {
            return itemNRs.Find(x => x == itemNR);
        }

    }
}
