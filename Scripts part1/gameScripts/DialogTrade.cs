using Inventory;
using UnityEngine;
using AnilTools;
using UnityEngine.UI;

namespace Dialog
{
    public partial class DialogControl 
    {
        // --- Ticaret ---
        public void EnterTrade()
        {
            GameMenu.instance.canvases.BaseCanvas.SetVisuality(false);

            if (npcController.npcType == NpcType.smith)         CurrentTradeMenu = silahcıMenu;
            else if (npcController.npcType == NpcType.merchant) CurrentTradeMenu = TüccarMenu;
            else if (npcController.npcType == NpcType.Healer)   CurrentTradeMenu = HealerMenu;

            StopAllCoroutines();
            EquipmentGroup.SetVisible(false);
            DialogGroup.SetVisible(false);
            InventoryCanvasGroup.SetVisible(true);
            GameMenu.instance.canvases.InventoryCanvas.SetActive(true);

            if (CurrentTradeMenu != null)
            {
                GameMenu.SetCanvasGroupFade(CurrentTradeMenu, true);
                CurrentTradeMenu.GetComponent<ScrollRect>().enabled = true;
            }
        }

        public void ExitTrade()
        {
            if (GetinEquipment()){
                GameMenu.instance.DisableInventory();
                return;
            }

            GameMenu.instance.canvases.BaseCanvas.SetVisuality(false == false);

            PlayerInventoryCamera.canLook = false;
            GameMenu.PlayerOnMenu = false;

            EquipmentGroup.SetVisible(false);

            if (!GetinEquipment()){
                DialogGroup.SetVisible(true);
                GameMenu.instance.canvases.InventoryCanvas.SetActive(false);
            }
            
            if (CurrentTradeMenu) {
                CurrentTradeMenu.SetVisible(false);
                CurrentTradeMenu.GetComponent<ScrollRect>().enabled = false;
            }

            GameMenu.instance.canvases.BaseCanvas.SetActive(true);
            PlayerUI.SetActive(true);
            OnDialogFinish();

            SetinEquipment(false);
        }

        // --- Inventory ---
        public void SellItem(byte id)
        {
            CharacterInventory.characterItems.ForEachBreak(x =>
            {
                if (x is ItemSC itemSC){
                    if (itemSC.Id == id){
                        CharacterInventory.instance.RemoveItem(itemSC);
                        Money += itemSC.Price;
                        uyarı.text = itemSC.Title + words.satıldı;
                        return true;
                    }
                }
                return false;
            });

            PopUpMenu.instance.PopUp("balta" + "selled", Color.white);
        }

        /// <summary> Npc'ye itemi vermek için </summary>
        public void GiveItem(byte id)
        {
            var item = CharacterInventory.characterItems.Find(x => ((ItemSC)x).Id == id) as ItemSC;

            CharacterInventory.instance.RemoveItem(item);
            PopUpMenu.instance.PopUp(Language.CurrentLanguage().GetItemName(id) + Language.CurrentLanguage().Gived, Color.white);
        }

        public void BuyItem(byte id)
        {
            var item = ItemDatabase.instance.GetItem(id);

            if (item is ItemSC itemSC)
            {
                if (Money <= itemSC.Price){
                    PopUpMenu.instance.PopUp(Language.MoneyNotEnough + Language.MoneyNotEnough, Color.white);
                    uyarı.text = words.yetersiz;
                    Debug2.Log("Yetersiz");
                }
                // envanter taşmasın
                else if (CharacterInventory.characterItems.Count < UIInventory.instance.SlotSayısı - 2){
                    CharacterInventory.instance.AddItem(itemSC.Id);
                    PopUpMenu.instance.PopUp(Language.CurrentLanguage().GetItemName(id) + Language.CurrentLanguage().Buyed, Color.white);
                    Money -= itemSC.Price;
                    uyarı.text = itemSC.Title + words.alındı;
                    Debug2.Log("Buy");
                }
            }
        }

        public void ReciveItem(short id)
        {
            var item = ItemDatabase.instance.GetItem(id);
            Debug2.Log("Buy Giriş");

            if (item is ItemSC itemSC)
            {
                CharacterInventory.instance.AddItem(itemSC.Id);
                Money -= itemSC.Price;
                uyarı.text = itemSC.Title + words.alındı;
                Debug2.Log("Gived");
            }
        }

        public void AddMoney(short amount)
        {
            Money += amount;
        }
    }
}