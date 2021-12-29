using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class EquipmentInfo : MonoBehaviour
    {
        public object savedItem = null;
        private Image itemSprite;

        [SerializeField] private Sprite defaultIcon;
        private Color StartColor;

        private void Start()
        {
            itemSprite = GetComponent<Image>();

            StartColor = itemSprite.color;
        }

        public void AddItem(ItemSC ıtem)
        {
            if (savedItem != null) return;

            itemSprite.sprite = ıtem.icon;
            itemSprite.color = Color.white;
            savedItem = ıtem;

            UpdateProperties(false);

            GameMenu.instance.UpdateStats();
        }

        public void AddItem(ItemNR ıtem)
        {
            if (savedItem != null) return;

            itemSprite.sprite = ıtem.icon;
            itemSprite.color = Color.white;
            savedItem = ıtem;

            UpdateProperties(false);

            GameMenu.instance.UpdateStats();
        }

        public void RemoveItem()
        {
            if (savedItem is IItem itemSC){
                CharacterInventory.instance.AddItem(itemSC.id());
                PlayerInfo.instance.RemoveItem(ItemDatabase.instance.itemSCs.Find(x => x.id() == itemSC.id()));
            }
            
            itemSprite.color = StartColor;
            itemSprite.sprite = defaultIcon;

            UpdateProperties(true);

            savedItem = null;

            GameMenu.instance.UpdateStats();
        }

        private void UpdateProperties(bool isRemove)
        {
            if (savedItem is ItemSC itemSC)
            {
                switch (itemSC.currentClass)
                {
                    case ItemClass.HandTool:
                        if (isRemove)
                        {
                            //characterMove.animatior.SetLayerWeight(0, 1);
                            CombatControl.instance.UpdateHandState(0);
                            switch (itemSC.handState)
                            {
                                case HandState.Sword:
                                    var swordItem = (SwordItem)itemSC;
                                    PlayerInfo.AttackDamage -= swordItem.Damage;
                                    PlayerInfo.AttackSpeed -= swordItem.AttackSpeed;
                                    break;
                                case HandState.Pickaxe:
                                    var pickAxeItem = (PickaxeItem)itemSC;
                                    PlayerInfo.AttackSpeed -= pickAxeItem.AttackSpeed;
                                    PlayerInfo.Digging -= pickAxeItem.Digging;
                                    break;
                                case HandState.Axe:
                                    var axeItem = (AxeItem)itemSC;
                                    PlayerInfo.AttackSpeed -= axeItem.AttackSpeed;
                                    PlayerInfo.WoodCutting -= axeItem.WoodCutting;
                                    break;
                            }
                        }
                        else
                        {
                            //characterMove.animatior.SetLayerWeight(1, 0); // karakterin kılıç tuttuğu elini kapalı hale getirir
                            switch (itemSC.handState)
                            {
                                case HandState.Sword:
                                    var swordItem = (SwordItem)itemSC;
                                    PlayerInfo.AttackDamage += swordItem.Damage;
                                    PlayerInfo.AttackSpeed += swordItem.AttackSpeed;
                                    break;
                                case HandState.Pickaxe:
                                    var pickAxeItem = (PickaxeItem)itemSC;
                                    PlayerInfo.AttackSpeed += pickAxeItem.AttackSpeed;
                                    PlayerInfo.Digging += pickAxeItem.Digging;
                                    break;
                                case HandState.Axe:
                                    var axeItem = (AxeItem)itemSC;
                                    PlayerInfo.AttackSpeed += axeItem.AttackSpeed;
                                    PlayerInfo.WoodCutting += axeItem.WoodCutting;
                                    break;
                            }
                            if (itemSC.currentClass == ItemClass.HandTool)
                            {
                                CombatControl.instance.UpdateHandState(itemSC.handState);
                            }
                        }
                        break;
                    case ItemClass.Armor:
                        var ArmorItem = (ArmorItem)savedItem;
                        PlayerInfo.DamageReducing += isRemove ? -ArmorItem.DamageReducing : ArmorItem.DamageReducing;
                        break;
                }
            }
            else if (savedItem is ItemNR itemNR)
            {
                switch (itemNR.currentClass)
                {
                    case ItemClass.HandTool:
                        if (isRemove)
                        {
                            //characterMove.animatior.SetLayerWeight(0, 1);
                            CombatControl.instance.UpdateHandState(0);
                            switch (itemNR.handState)
                            {
                                case HandState.Sword:
                                    var swordItem = (SwordNew)itemNR;
                                    PlayerInfo.AttackDamage -= swordItem.Damage;
                                    PlayerInfo.AttackSpeed -= swordItem.AttackSpeed;
                                    break;
                            }
                        }
                        else
                        {
                            //characterMove.animatior.SetLayerWeight(1, 0); // karakterin kılıç tuttuğu elini kapalı hale getirir
                            switch (itemNR.handState){
                                case HandState.Sword:
                                    var swordItem = (SwordNew)savedItem;
                                    PlayerInfo.AttackDamage += swordItem.Damage;
                                    PlayerInfo.AttackSpeed += swordItem.AttackSpeed;
                                    break;
                            }
                            if (itemNR.currentClass == ItemClass.HandTool){
                                CombatControl.instance.UpdateHandState(itemNR.handState);
                            }
                        }
                        break;
                    case ItemClass.Armor:
                        var ArmorItem = (ArmorNew)savedItem;
                        PlayerInfo.DamageReducing += isRemove ? -ArmorItem.DamageReducing : ArmorItem.DamageReducing;
                        break;
                }
            }
        }
    }
}
