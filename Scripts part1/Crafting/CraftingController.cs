using AnilTools;
using Inventory;
using UnityEngine;
using UnityEngine.UI;

using Sstring = StringOperationUtil.OptimizedStringOperation;

namespace Crafting
{
    public class CraftingController : Singleton<CraftingController> , UILanguage
    {
        public static CraftSlot CurrentSlot;

        [Space] // menu
        [SerializeField] private Text Uretim;
        [SerializeField] private Text Yap;
        [SerializeField] private Text Özellikler;
        [Space] // item
        [SerializeField] private Text Description;
        [SerializeField] private Text Name;
        [SerializeField] private Text Stat;
        [SerializeField] private Text Stat1;

        private CharacterInventory CharacterInventory;

        private TooltipWords TooltipWords => ToolTip.instance.TooltipWords;
        private ItemSC ProductItem => CurrentSlot.craft.ProductItem;

        private void Start()
        {
            CharacterInventory = CharacterInventory.instance;
        }

        public void UpdateYapBtn()
        {
            Stat.text = string.Empty;
            Stat1.text = string.Empty;

            if (!CurrentSlot)
                return;

            CraftBox.instance.Show(ProductItem);

            Yap.GetComponentInParent<Image>().color = CharacterInventory.CheckForItemExist(CurrentSlot.craft.ItemIDs()) ? Color.green : Color.grey;

            Name.text = ProductItem.Title;
            Description.text = ProductItem.description;
            switch (ProductItem.currentClass)
            {
                case ItemClass.trade:
                    Stat.text =  Sstring.tiny + $"{TooltipWords.fiyat}: {ProductItem.Price}";
                    break;
                case ItemClass.Eating:
                    Stat.text = Sstring.tiny + $"{TooltipWords.healing}: {CastItem<EatingItem>().doyuruculuk}";
                    break;
                case ItemClass.Armor:
                    Stat.text = Sstring.tiny + $"{TooltipWords.DamageReducing}: {CastItem<ArmorItem>().DamageReducing}";
                    break;
                case ItemClass.HandTool:
                    switch (ProductItem.handState)
                    {
                        case HandState.Punch:
                            break;
                        case HandState.Sword:
                            Stat.text = Sstring.tiny  + $"{TooltipWords.güç}         : {CastItem<SwordItem>().Damage}";
                            Stat1.text = Sstring.tiny + $"{TooltipWords.AttackSpeed}:  {CastItem<SwordItem>().AttackSpeed}";
                            break;
                        case HandState.Pickaxe:
                            Stat.text = Sstring.tiny + $"{TooltipWords.digging}     :  {CastItem<PickaxeItem>().Digging}";
                            break;
                        case HandState.Axe:
                            Stat.text = Sstring.tiny + $"{TooltipWords.woodCutting} :  {CastItem<AxeItem>().WoodCutting}";
                            break;
                    }
                    break;
                case ItemClass.potion:
                    Stat.text = Sstring.tiny + $"{TooltipWords.healing}: {ItemSC.CastItem<PotionItem>(ProductItem).Healing}";
                    break;
                case ItemClass.crafting:
                    ///
                    break;
            }
        }

        private T CastItem<T>()
        {
            return ItemSC.CastItem<T>(ProductItem);
        }

        public void CraftItem()
        {
            if (!CurrentSlot)
                return;

            if (CharacterInventory.CheckForItemExist(CurrentSlot.craft.ItemIDs())) 
            {
                CharacterInventory.AddItem(ProductItem.Id);
                for (int i = 0; i < CurrentSlot.craft.items.Length; i++){
                    CharacterInventory.RemoveItem(CurrentSlot.craft.items[i].Id);
                }
                CraftDatabase.instance.UpdateCounts();
            }
            else{
                PopUpMenu.instance.PopUp(Language.InventoryWarning, Color.white);
            }
        }

        public void UpdateLanguage(int id)
        {
            var pack = GameMenu.instance.languages.languages[id].craftingLanguage;
            Yap.text = pack.Yap;
            Uretim.text = pack.Uretim;
            Özellikler.text = pack.Özellikler;
        }

    }
}