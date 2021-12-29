using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rock
{
    public class RockSlot : AnilButton,IPointerDownHandler,IPointerClickHandler
    {
        // solda spawnlanacaklar
        // rock , armor , sword
        public object item;
        private Image ico => transform.GetChild(0).GetComponent<Image>();
        internal short InventoryID;

        public void Init(object item, short InventoryID)
        {
            if (item is ItemSC itemSC)
            {
                this.item = itemSC;
                ico.sprite = itemSC.icon;
                this.InventoryID = InventoryID;
                itemSC.InventoryId = InventoryID;
            }
            else if (item is ItemNR itemNR)
            {
                this.item = itemNR;
                //if(ico)
                ico.sprite = itemNR.icon;
                this.InventoryID = InventoryID;
                itemNR.InventoryId = InventoryID;
            }
        }

        private void Update()
        {
            if (Sellected)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Desellect();
                }
            }
        }

        public override void Desellect()
        {
            base.Desellect();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Sellect();
            RockBox.SellectedItem = item;
            RockBox.InventoryId = InventoryID;
            RockBox.instance.Sellect();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (item is ItemSC itemSC)
            {
                ToolTip.instance.GenerateToolTip(itemSC, true);
            }
            else if (item is ItemNR itemNR)
            {
                ToolTip.instance.GenerateToolTip(itemNR, true);
            }
        }

    }
}