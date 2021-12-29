using UnityEngine;
using UnityEngine.UI;
using Inventory;
using Rock;
using StrOpe = StringOperationUtil.OptimizedStringOperation;
using AnilTools;
using Player;

public class ToolTip : Singleton<ToolTip>
{
    #region structs
    [System.Serializable]
    private struct Texts
    {
        [Header("Menu")]
        public Text isim;
        public Text fiyat;
        public Text detay;
        public Text stat;
        public Text stat1;

        public Texts(Text isim, Text fiyat, Text detay, Text stat, Text stat1)
        {
            this.isim = isim;
            this.fiyat = fiyat;
            this.detay = detay;
            this.stat = stat;
            this.stat1 = stat1;
        }
    }

    [System.Serializable]
    private struct Buttons
    {
        public GameObject at;
        public GameObject giy;
        public GameObject ye;
    }
    #endregion

    [SerializeField] Texts texts;
    [SerializeField] TooltipWords tooltipWords;
    public TooltipWords TooltipWords => tooltipWords;
    [SerializeField] Buttons buttonsInMemory;
    
    [Space]
    [SerializeField] private Image ImageSprite;
    
    [HideInInspector] public object currentItem;

    [SerializeField] EquipmentInfo armor;
    [SerializeField] EquipmentInfo handTool;

    [SerializeField] private Image rockImg;
    [SerializeField] private Image rockImg1;
    [SerializeField] private Image rockImg2;

    private AudioClip SwordEquip;
    private AudioClip ArmorEquip;
   
    private void Start()
    {
        ZeroTooltip();

        SwordEquip = Resources.Load<AudioClip>("Sounds/other/sword ding");
        ArmorEquip = Resources.Load<AudioClip>("Sounds/other/Wear Armor");
    }

    public void GenerateToolTip(ItemSC item,bool isInfo = false)
    {
        ZeroTooltip();

        HideRocks();

        texts.isim.text = item.Title;
        texts.detay.text = item.description;
        ImageSprite.sprite = item.icon;
        texts.fiyat.text = $"{tooltipWords.fiyat} {item.Price} C";

        currentItem = item;

        switch (item.currentClass)
        {
            case ItemClass.HandTool:
                StatActivate(true, true);

                switch (item.handState)
                {
                    case HandState.Sword:
                        texts.stat.text = StrOpe.tiny + tooltipWords.güç + StrOpe.space + CastItem<SwordItem>().Damage;
                        texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + CastItem<SwordItem>().AttackSpeed;
                        break;
                    case HandState.Pickaxe:
                        texts.stat.text = StrOpe.tiny + tooltipWords.digging + StrOpe.space + CastItem<PickaxeItem>().Digging;
                        texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + CastItem<PickaxeItem>().AttackSpeed;
                        break;
                    case HandState.Axe:
                        texts.stat.text = StrOpe.tiny + tooltipWords.woodCutting + StrOpe.space + CastItem<AxeItem>().WoodCutting;
                        texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + CastItem<AxeItem>().AttackSpeed;
                        break;
                }
                if (!isInfo)
                {
                    CreateButtons(buttonsInMemory.at, buttonsInMemory.giy);
                }
                break;
            case ItemClass.Armor:
                StatActivate(true, true);
                texts.stat.text = StrOpe.tiny + tooltipWords.DamageReducing + StrOpe.space + CastItem<ArmorItem>().DamageReducing;
                if (!isInfo)
                {
                    CreateButtons(buttonsInMemory.at, buttonsInMemory.giy);
                }
                break;
            case ItemClass.potion:
                StatActivate(true, true);
                texts.stat.text = StrOpe.tiny + tooltipWords.healing + StrOpe.space + CastItem<PotionItem>().Healing;
                if (!isInfo){
                    CreateButtons(buttonsInMemory.ye);
                }
                break;
            case ItemClass.crafting:
                // simdilik bos
                break;
            case ItemClass.trade:
                CreateButtons(buttonsInMemory.at);
                break;
            case ItemClass.rock:
                if (currentItem is RockItem rockItem)
                {
                    if (rockItem.rockType == RockType.armor)
                    {
                        StatActivate(true, true);
                        texts.stat.text = StrOpe.tiny + tooltipWords.DamageReducing + StrOpe.space + rockItem.damageReducing;
                    }
                    if (rockItem.rockType == RockType.armor)
                    {
                        StatActivate(true, true);
                        texts.stat.text = StrOpe.tiny + tooltipWords.güç + StrOpe.space + rockItem.damage;
                        texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + rockItem.AttackSpeed;
                    }
                }
                break;
            case ItemClass.bow:
                if (currentItem is BowItem bowItem)
                {
                    StatActivate(true, true);
                    texts.stat.text = StrOpe.tiny + tooltipWords.güç + StrOpe.space + bowItem.Damage;
                    texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + bowItem.AttackSpeed;
                }
                break;
            default:
                texts.stat.text = string.Empty;
                texts.stat1.text = string.Empty;
                break;
        }
        if (currentItem is EatingItem eatingItem)
        {
            Debug2.Log("yes");
            StatActivate(true, true);
            texts.stat.text = StrOpe.tiny + tooltipWords.doyma + StrOpe.space + eatingItem.doyuruculuk;
            if (!isInfo)
            {
                CreateButtons(buttonsInMemory.ye);
            }
        }

        CanvasActive(true);
    
    }

    public void GenerateToolTip(ItemNR item, bool isInfo = false)
    {
        ZeroTooltip();

        CanvasActive(true);

        texts.isim.text = item.Title;
        texts.detay.text = item.description;
        ImageSprite.sprite = item.icon;
        texts.fiyat.text = $"{tooltipWords.fiyat} {item.Price} C";

        currentItem = item;

        switch (item)
        {
            case SwordNew swordNew:
                StatActivate(true, true);
                texts.stat.text = StrOpe.tiny + tooltipWords.güç + StrOpe.space + swordNew.TopDamage;
                texts.stat1.text = StrOpe.tiny + tooltipWords.AttackSpeed + StrOpe.space + swordNew.TopAttackSpeed;
                ShowRocks(swordNew);
                if (!isInfo)
                {
                    CreateButtons(buttonsInMemory.at, buttonsInMemory.giy);
                }
                break;
            case ArmorNew armorNew:
                StatActivate(false, true);
                texts.stat.text = StrOpe.tiny + tooltipWords.DamageReducing + StrOpe.space + armorNew.TopDamageReducing;
                if (!isInfo)
                {
                    CreateButtons(buttonsInMemory.at, buttonsInMemory.giy);
                }
                ShowRocks(armorNew);
                break;
        }
    }

    /// <summary>
    /// sadece taşları göstermeye yarar
    /// </summary>
    private void ShowRocks(ItemNR item)
    {
        HideRocks();
        
        if (item.rockPack == null)
            return;
        
        if (item.rockPack.rock != default)
        {
            rockImg.color = Color.white;
            rockImg.sprite = item.rockPack.rock.icon;
        }
        if (item.rockPack.rock1 != default)
        {
            rockImg1.sprite = item.rockPack.rock1.icon;
            rockImg1.color = Color.white;
        }
        if (item.rockPack.rock2 != default)
        {
            rockImg2.sprite = item.rockPack.rock2.icon;
            rockImg2.color = Color.white;
        }
    }

    /// <summary>
    /// tooltip ekranında gözüken taşları saklar
    /// </summary>
    private void HideRocks()
    {
        rockImg.sprite = null;
        rockImg1.sprite = null;
        rockImg2.sprite = null;
        
        rockImg.color = Color.clear;
        rockImg1.color = Color.clear;
        rockImg2.color = Color.clear;
    }

    private T CastItem<T>()
    {
        return ItemSC.CastItem<T>(currentItem);
    }

    public void CloseToolTip()
    {
        CanvasActive(false);

        texts.stat.gameObject.SetActive(true);
        texts.stat1.gameObject.SetActive(true);
        ZeroTooltip();
        HideRocks();

        buttonsInMemory.at.SetActive(false);
        buttonsInMemory.giy.SetActive(false);
        buttonsInMemory.ye.SetActive(false);
    }
    
    private void CreateButtons(params GameObject[] ActiveButtons)
    {
        buttonsInMemory.at.SetActive(false);
        buttonsInMemory.giy.SetActive(false);
        buttonsInMemory.ye.SetActive(false);

        for (short i = 0; i < ActiveButtons.Length; i++)
        {
            ActiveButtons[i].SetActive(true);
        }
    }

    /// <summary>
    /// stat1 ve 2 nin açık olup olmaması
    /// </summary>
    /// <param name="Stat1Acık"></param>
    /// <param name="StatAcıkmı"></param>
    private void StatActivate(bool Stat1Acık, bool StatAcıkmı)
    {
        texts.stat1.gameObject.SetActive(Stat1Acık);

        texts.stat.gameObject.SetActive(StatAcıkmı);
    }

    public void EatFood()
    {
        byte heal = 0;

        if (currentItem is ItemSC food)
        {
            if (food.currentClass == ItemClass.potion)
            {
                heal = (byte)CastItem<PotionItem>().Healing;
            }
            else if (food.currentClass == ItemClass.crafting)
            {
                heal = (byte)CastItem<EatingItem>().doyuruculuk;
            }
            CharacterInventory.instance.RemoveItem(food.Id);
        }
        CanvasActive(false);
        CharacterHealth.instance.AddHealth(heal);
    }

    public void EquipItem()
    {
        if (currentItem is BowItem bowItem)
        {
            if (handTool.savedItem != null){
                handTool.RemoveItem();
            }

            bowItem.InventoryId = 62;
            PlayerInfo.instance.EquipItem(bowItem);
            CharacterInventory.instance.RemoveItem(bowItem.Id);
            PlayerInfo.AttackDamage += bowItem.Damage;
        }

        if (currentItem is AxeItem axeItem)
        {
            if (handTool.savedItem != null){
                handTool.RemoveItem();
            }

            GameManager.instance.audioSource.PlayOneShot(SwordEquip);

            axeItem.InventoryId = 62;
            PlayerInfo.instance.EquipItem(axeItem);
            CharacterInventory.instance.RemoveItem(axeItem.Id);
            PlayerInfo.WoodCutting = axeItem.WoodCutting;
        }

        if (currentItem is PickaxeItem pickaxeItem)
        {
            if (handTool.savedItem != null){
                handTool.RemoveItem();
            }

            GameManager.instance.audioSource.PlayOneShot(SwordEquip);

            pickaxeItem.InventoryId = 62;
            PlayerInfo.instance.EquipItem(pickaxeItem);
            CharacterInventory.instance.RemoveItem(pickaxeItem.Id);
            PlayerInfo.Digging = pickaxeItem.Digging;
        }

        if (currentItem is SwordItem swordItem)
        {
            if (handTool.savedItem != null){
                handTool.RemoveItem();
            }

            GameManager.instance.audioSource.PlayOneShot(SwordEquip);

            swordItem.InventoryId = 62;
            PlayerInfo.instance.EquipItem(swordItem);
            CharacterInventory.instance.RemoveItem(swordItem.Id);
            PlayerInfo.AttackDamage += swordItem.Damage;
            
        }
        else if (currentItem is ArmorItem armorItem)
        {
            if (armor.savedItem != null){
                armor.RemoveItem();
            }

            GameManager.instance.audioSource.PlayOneShot(ArmorEquip);

            armorItem.InventoryId = 61;
            PlayerInfo.instance.EquipItem(armorItem);
            CharacterInventory.instance.RemoveItem(armorItem.Id);

        }
        else if (currentItem is SwordNew swordNew)
        {
            if (handTool.savedItem != null){
                handTool.RemoveItem();
            }

            GameManager.instance.audioSource.PlayOneShot(ArmorEquip);

            swordNew.InventoryId = 62;
            PlayerInfo.instance.EquipItem(swordNew);
            CharacterInventory.instance.RemoveItem(swordNew);
        }
        else if (currentItem is ArmorNew armorNew)
        {
            if (armor.savedItem != null) {
                armor.RemoveItem();
            }
            armorNew.InventoryId = 61;
            PlayerInfo.instance.EquipItem(armorNew);
            CharacterInventory.instance.RemoveItem(armorNew);
        }
        
        CloseToolTip();
    }

    public void DropdownItem()
    {
        if (currentItem is ItemSC itemSC)
        {
            CharacterInventory.instance.RemoveItem(itemSC.Id);
            PoolManager.instance.ReuseObject(itemSC.poolName, NpcController2.Player.transform.position + new Vector3(0, 0, 2), Quaternion.identity);
        }
        else if (currentItem is ItemNR itemNR)
        {
            CharacterInventory.instance.RemoveItem(itemNR);
            PoolManager.instance.ReuseObject(itemNR.poolName, NpcController2.Player.transform.position + new Vector3(0, 0, 2), Quaternion.identity);
        }
        CanvasActive(false);
    }

    public void ZeroTooltip()
    {
        buttonsInMemory.at.SetActive(false);
        buttonsInMemory.giy.SetActive(false);
        buttonsInMemory.ye.SetActive(false);

        texts.isim.text = string.Empty;
        texts.detay.text = string.Empty;
        texts.fiyat.text = string.Empty;
        texts.stat.text = string.Empty;
        texts.stat1.text = string.Empty;
    }

    private void CanvasActive(bool value)
    {
        GameMenu.instance.canvases.TooltipCanvas.SetVisuality(value);
    }

    public void ChangeLanguage(TooltipWords dialogWords)
    {
        this.tooltipWords = dialogWords;
        buttonsInMemory.at.GetComponentInChildren<Text>().text = tooltipWords.at;
        buttonsInMemory.ye.GetComponentInChildren<Text>().text = tooltipWords.ye;
        buttonsInMemory.giy.GetComponentInChildren<Text>().text = tooltipWords.giy;
    }

}
