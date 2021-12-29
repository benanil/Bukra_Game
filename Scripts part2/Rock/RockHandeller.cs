using AnilTools;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rock
{
    public class RockHandeller : Singleton<RockHandeller>, UILanguage
    {
        

        [SerializeField] private Text stat;
        [SerializeField] private Text stat1;
        [SerializeField] private Text stat2;

        public ButtonNew Craft;

        public RockYuva RockYuva;
        public RockYuva RockYuva1;
        public RockYuva RockYuva2;

        public RockYuva ItemYuva;

        internal bool Ekleyebilir => ItemYuva.item != null;
        
        internal readonly List<RockItem> eklenecekler = new List<RockItem>(3);

        private void Start()
        {
            Craft.UnityEvent = new Button.ButtonClickedEvent();
            Craft.UnityEvent.AddListener(() => Ekle());
        }

        public void Ekle()
        {
            if (Ekleyebilir)
            {
                for (short i = 0; i < eklenecekler.Count; i++)
                {
                    RockSaveManager.instence.Ekle(ItemYuva.item , eklenecekler[i], ref ItemYuva.InventoryId);
                }
            }

            // todo: eklendi yazısı ve ses çıkart
            
            eklenecekler.Clear();
        }

        public void EklemektenVazgec()
        {
            for (int i = 0; i < eklenecekler.Count; i++)
            {
                CharacterInventory.instance.AddItem(eklenecekler[i].Id);
            }
            eklenecekler.Clear();
        }

        public void UpdateUI()
        {
            stat.text = string.Empty;
            stat1.text = string.Empty;
            stat2.text = string.Empty;

            if (ItemYuva.item is ArmorItem armorItem)
            {
                float damageReducing = 0;

                if (RockYuva.rock)
                {
                    Debug2.Log("asdasdasd");
                    if (RockYuva.rock.damageReducing != 0)
                        damageReducing += RockYuva.rock.damageReducing;
                }
                if (RockYuva1.rock)
                {
                    if (RockYuva1.rock.damageReducing != 0)
                        damageReducing += RockYuva1.rock.damageReducing;
                }
                if (RockYuva2.rock)
                {
                    if (RockYuva2.rock.damageReducing != 0)
                        damageReducing += RockYuva2.rock.damageReducing;
                }
                stat.text = ToolTip.instance.TooltipWords.DamageReducing + ": " + armorItem.DamageReducing.ToString() + $"<color=blue> + {damageReducing}</color>";
            }
            else if (ItemYuva.item is SwordItem swordItem)
            {
                int damage = 0;
                float attackspeed = 0;

                if (RockYuva.rock)
                {
                    Debug2.Log("asdasdasd");
                    if (RockYuva.rock.damage != 0)
                        damage += RockYuva.rock.damage;
                    if (RockYuva.rock.AttackSpeed != 0)
                        attackspeed -= RockYuva.rock.AttackSpeed;
                }
                if (RockYuva1.rock)
                {
                    if (RockYuva1.rock.damage != 0)
                        damage += RockYuva1.rock.damage;
                    if (RockYuva1.rock.AttackSpeed != 0)
                        attackspeed -= RockYuva1.rock.AttackSpeed;
                }
                if (RockYuva2.rock)
                {
                    if (RockYuva2.rock.damage != 0)
                        damage += RockYuva2.rock.damage;
                    if (RockYuva2.rock.AttackSpeed != 0)
                        attackspeed -= RockYuva2.rock.AttackSpeed;
                }
                stat.text = ToolTip.instance.TooltipWords.güç + ": " + swordItem.Damage + $"<color=green> +{damage}</color>";
                stat1.text = ToolTip.instance.TooltipWords.AttackSpeed + ": " + swordItem.AttackSpeed + $"<color=green> - {attackspeed}</color>";
            }

            Craft.Image.color = eklenecekler.Count > 0 ? Color.white : Color.grey;

        }

        internal void NewItem()
        {
            RockYuva.UpdateUI();
            RockYuva1.UpdateUI();
            RockYuva2.UpdateUI();
        }

        internal void RemoveItem()
        {
            RockYuva.ClearUI();
            RockYuva1.ClearUI();
            RockYuva2.ClearUI();
        }

        public void UpdateLanguage(int id)
        {
            var pack = GameMenu.instance.languages.languages[id].rockLanguage;
            Craft.GetComponentInChildren<Text>().text = pack.Craft;
        }
    }

}