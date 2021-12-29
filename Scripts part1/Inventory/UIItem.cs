using AnilTools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{  
    public class UIItem : MonoBehaviour, IPointerDownHandler
    {
        public object item;
        public Type type => item.GetType();
        public UnityEngine.Object itemDebug;

        public Image ItemIcon;
        public TextMeshProUGUI CountTxt;

        [SerializeField] 
        private bool isInfo;
        public MerchantSlot merchantSlot;

        public bool HasItem;

        public byte id;

        public sbyte Count;

        private void Awake()
        {
            if (CountTxt){
                CountTxt.text = string.Empty;
                item = null;
                HasItem = false;
            }
        }

        public void Spawn(byte id)
        {
            this.id = id;
        }

        public void UpdateItem(IItem _item)
        {
            //Debug2.Log("update item");

            if (_item == null)
            {
                //Debug2.Log("item null");
                if (Count == 0)        Debug.LogError("envanterde bu item yok kaldırmaya çalışıyon");                
                else if (item is null) Debug.LogError("item null kaldırmaya çalışıyon");
                
                Count--;
                
                if (Count == 0){
                    item = null;
                    HasItem = false;
                    if (ItemIcon)
                        ItemIcon.color = Color.clear;
                }

                if (CountTxt) CountTxt.SetText(Count == 0 || Count == 1 ? string.Empty : Count.ToString());
                return;
            }

            if (item is null) { // item ilk eklenince
                item = _item;
            }

            if (item.GetType() != type){
                Debug.Log("item tipi yanlış");
                return;
            }

            Count++;

            if (CountTxt) CountTxt.SetText(Count == 0 || Count == 1 ? string.Empty : Count.ToString());

            ItemIcon.sprite = _item.icon();
            id = _item.id();
            HasItem = true;
            item = _item;

            new Timer(.5f,() => {
                ItemIcon.color = Color.white;
                item = _item;
                HasItem = true;
            });

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isInfo)
                if (merchantSlot.item)
                    ToolTip.instance.GenerateToolTip(merchantSlot.item,true);

            if (HasItem)
            {
                if (item is ItemSC SC)      ToolTip.instance.GenerateToolTip(SC);
                else if (item is ItemNR NR) ToolTip.instance.GenerateToolTip(NR);
            }
            
            //Debug2.Log("clicked");
        }

        public void DropItem()
        {
            //PoolManager.instance.ReuseObject(item.poolName, Player.position + new Vector3(0, 0, 2), Quaternion.identity);
            //CharacterInventory.instance.RemoveItem(itemSC.Id);
        }
    }
}
