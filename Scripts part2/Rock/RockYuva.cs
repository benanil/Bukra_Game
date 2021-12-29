using Inventory;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Rock
{
    public class RockYuva : AnilButton, IPointerClickHandler , IPointerEnterHandler
    {
        private enum RockYuvaState { Item , Tas}
        public enum RockYuvaSayi { zero,one,two}

        [SerializeField]
        private RockYuvaState YuvaState;
        public RockYuvaSayi rockYuvaSayi;

        public object item;
        public RockItem rock;

        public Image icon;

        internal short InventoryId;

        public bool Tas => YuvaState == RockYuvaState.Tas;
        public bool Item => YuvaState == RockYuvaState.Item;

        public bool HasItem{
            get{ 
                return YuvaState == RockYuvaState.Item ? item != null : rock != null;
            }
        }

        public void Show()
        {
            if (YuvaState == RockYuvaState.Item)
            {
                if (item == null) return;

                if (item is ItemSC itemSC)
                {
                    icon.sprite = itemSC.icon;
                }
                else if(item is ItemNR items)
                {
                    icon.sprite = items.icon;
                }
            }
            else if(YuvaState == RockYuvaState.Tas)
            {
                if (rock == null) return;

                if (rock.rockType == RockType.armor)
                {
                    icon.sprite = rock.icon;
                }
                else
                {
                    icon.sprite = rock.icon;
                }
            }
        }

        public override void Sellect()
        {
            if (RockBox.SellectedItem == null)
            {
                //Debug2.Log("rockbox.item = null");
                return;
            }

            if (YuvaState == RockYuvaState.Item)
            {
                if (RockBox.SellectedItem is ItemSC itemSC)
                {
                    item = itemSC;
                    icon.sprite = itemSC.icon;
                    icon.color = Color.white;
                    InventoryId = RockBox.InventoryId;
                    RockHandeller.instance.RemoveItem();
                    RockHandeller.instance.NewItem();
                    RockDatabase.instance.UpdateUI();
                }
                else if (RockBox.SellectedItem is ItemNR itemNR)
                {
                    item = itemNR;
                    icon.sprite = itemNR.icon;
                    icon.color = Color.white;
                    InventoryId = RockBox.InventoryId;
                    RockHandeller.instance.RemoveItem();
                    RockHandeller.instance.NewItem();
                    RockDatabase.instance.UpdateUI();
                }
                RockHandeller.instance.EklemektenVazgec();
                return;
            }

            if (RockBox.SellectedItem is RockItem BoxItem)
            {
                if (RockHandeller.instance.ItemYuva.item is ItemSC itemSC1 && !HasItem && YuvaState == RockYuvaState.Tas)
                {
                    if (itemSC1.rockPack.RockType == BoxItem.rockType)
                    {
                        rock = BoxItem;
                        RockHandeller.instance.eklenecekler.Add(rock);
                        RockHandeller.instance.UpdateUI();
                        icon.sprite = rock.icon;
                        CharacterInventory.instance.RemoveItem(rock.Id);
                        RockDatabase.instance.UpdateUI();
                    }
                }
            }
        }

        public void ClearUI()
        {
            icon.sprite = null;
            item = null;
            icon.color = Color.clear;
        }

        public void UpdateUI()
        {
            if (RockBox.SellectedItem == null)
                return;

            if (RockBox.SellectedItem is ItemSC itemSC)
            {
                switch (rockYuvaSayi)
                {
                    case RockYuvaSayi.zero:
                        Check(itemSC.rockPack.rock);
                        break;
                    case RockYuvaSayi.one:
                        Check(itemSC.rockPack.rock1);
                        break;
                    case RockYuvaSayi.two:
                        Check(itemSC.rockPack.rock2);
                        break;
                }
            }
            else if (RockBox.SellectedItem is ItemNR itemNR)
            {
                switch (rockYuvaSayi)
                {
                    case RockYuvaSayi.zero:
                        Check(itemNR.rockPack.rock);
                        break;
                    case RockYuvaSayi.one:
                        Check(itemNR.rockPack.rock1);
                        break;
                    case RockYuvaSayi.two:
                        Check(itemNR.rockPack.rock2);
                        break;
                }
            }

            icon.color = Color.white;
        }

        private void Check(RockItem rock)
        {
            if (rock == default)
                return;
            
            icon.sprite = rock.icon;
            
        }

        public override void Desellect()
        {
            if (YuvaState == RockYuvaState.Item)
            {
                ClearUI();
                InventoryId = 0;
                RockHandeller.instance.RemoveItem();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Desellect();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Sellect();
            RockBox.instance.Desellect();
        }
    }
}