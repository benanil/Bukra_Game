using AnilTools.Save;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Rock
{
    public class RockSaveManager : MonoBehaviour
    {
        public const string ArmorName = "SavedArmors.json";
        public const string ArmorPath = "Rocks/Armors/";

        public const string SwordName = "SavedSwords.json";
        public const string SwordPath = "Rocks/Swords/";
        private const string KayıtEdildi = "----- Rock Pack Kayıt edildii ----";
        private const string TaşOlanOBjyok = "taş olan obje yok";
        private static RockSaveManager _instance;
        public static RockSaveManager instence
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RockSaveManager>();
                }
                return _instance;
            }
        }

        // saved armor and sword
        public List<ArmorNew> SavedArmors { get; private set; } //: json üzerinden save edilen tas paketidir
        public List<SwordNew> SavedSwords { get; private set; }

        private object SavedSword => PlayerInfo.instance.HandInfo.savedItem;
        private object SavedArmor => PlayerInfo.instance.ArmorInfo.savedItem;

        /// <summary>
        /// amaç ınventory deki taşlı itemleri xml olarak save etmek
        /// </summary>
        [ContextMenu("Save")]
        public void Save()
        {
            SavedArmors = new List<ArmorNew>();
            SavedSwords = new List<SwordNew>();

            if (SavedArmor is ArmorNew saveArmor)
            {
                saveArmor.InventoryId = 61;
                SavedArmors.Add(saveArmor);
            }

            if (SavedSword is SwordNew savedSword)
            {
                savedSword.InventoryId = 62;
                SavedSwords.Add(savedSword);
            }

            for (short i = 0; i < CharacterInventory.SpecialItems.Count; i++)
            {
                if (CharacterInventory.SpecialItems[i] is SwordNew swordNew)
                {
                    swordNew.InventoryId = i;
                    SavedSwords.Add(swordNew);
                }
                else if (CharacterInventory.SpecialItems[i] is ArmorNew armorNew)
                {
                    armorNew.InventoryId = i;
                    SavedArmors.Add(armorNew);
                }
            }

            if (SavedArmors.Count > 0)
            {
                JsonManager.SaveList(ArmorPath, ArmorName, SavedArmors);
                JsonManager.SaveList(SwordPath, SwordName, SavedSwords);
                Debug2.Log(KayıtEdildi, Color.blue);
            }
            else
            {
                Debug2.Log(TaşOlanOBjyok);
            }

        }

        // amaç xml den veriyi çekmek ve envanterdeki kılıç ve armorlara uygulamak
        public void Load()
        {
            SavedArmors = JsonManager.LoadList<ArmorNew>(ArmorPath, ArmorName).ToList();
            SavedSwords = JsonManager.LoadList<SwordNew>(SwordPath, SwordName).ToList();
            
            Debug2.Log(SavedArmors.Count + " kadar saved");

            if (SavedArmors == default)
            {
                Debug2.Log("kaydedilen rock pack yok",Color.yellow);
                return;
            }

            for (short i = 0; i < SavedArmors.Count; i++)
            {
                if (SavedArmors[i].InventoryId == 61)
                {
                    ToolTip.instance.currentItem = SavedArmors[i];
                    ToolTip.instance.EquipItem();
                }
                else
                {
                    CharacterInventory.instance.AddItem(SavedArmors[i]);
                }
            }
            for (short i = 0; i < SavedSwords.Count; i++)
            {
                if (SavedSwords[i].InventoryId == 62)
                {
                    ToolTip.instance.currentItem = SavedSwords[i];
                    ToolTip.instance.EquipItem();
                }
                else
                {
                    CharacterInventory.instance.AddItem(SavedSwords[i]);
                }
            }
            RockDatabase.instance.UpdateUI();
        }

        #region degistir
        private void Degistir(out SwordNew degisecek, ItemSC old)
        {
            degisecek = new SwordNew
            (
                ((SwordItem)old).AttackSpeed,
                ((SwordItem)old).Damage
            )
            {
                Id = (byte)Random.Range(100, byte.MaxValue),
                poolName = old.poolName,
                currentClass = old.currentClass,
                handState = old.handState,
                description = old.description,
                icon = old.icon,
                Price = (short)(old.Price + 50),
                HandPosition = old.HandPosition,
                mesh = old.mesh,
                rockPack = old.rockPack,
            };
        }

        private void Degistir(out ArmorNew degisecek, ItemSC old)
        {
            degisecek = new ArmorNew(((ArmorItem)old).DamageReducing, ((ArmorItem)old).ArmorMesh)
            {
                Id = (byte)Random.Range(100, byte.MaxValue),
                poolName = old.poolName,
                currentClass = old.currentClass,
                handState = old.handState,
                description = old.description,
                icon = old.icon,
                Price = (short)(old.Price + 50),
                HandPosition = old.HandPosition,
                mesh = old.mesh,
                rockPack = old.rockPack,
             };
        }

        private void Degistir(out SwordNew degisecek, ItemNR old)
        {
            degisecek = new SwordNew(((SwordNew)old).AttackSpeed,((SwordNew)old).Damage)
            {
                Id = (byte)Random.Range(100, byte.MaxValue),
                poolName = old.poolName,
                currentClass = old.currentClass,
                handState = old.handState,
                description = old.description,
                icon = old.icon,
                Price = (short)(old.Price + 50),
                HandPosition = old.HandPosition,
                mesh = old.mesh,
                rockPack = old.rockPack,
            };
        }

        private void Degistir(out ArmorNew degisecek, ItemNR old)
        {
            degisecek = new ArmorNew(((ArmorNew)old).DamageReducing , ((ArmorNew)old).ArmorMesh)
            {
                Id = (byte)Random.Range(100, byte.MaxValue),
                poolName = old.poolName,
                currentClass = old.currentClass,
                handState = old.handState,
                description = old.description,
                icon = old.icon,
                Price = (short)(old.Price + 50),
                HandPosition = old.HandPosition,
                mesh = old.mesh,
                rockPack = old.rockPack,
            };
        }
        #endregion

        internal void Ekle(object item, RockItem eklenecek, ref short inventoryID)
        {
            if (item is SwordItem old)
            {
                Degistir(out SwordNew degisecek, old);
                degisecek.InventoryId = inventoryID;

                if (degisecek.rockPack == null)
                {
                    degisecek.rockPack = new RockPack(RockType.armor, default, default, default);
                }

                if (degisecek.rockPack.rock == null) degisecek.rockPack.rock = eklenecek;
                else if (degisecek.rockPack.rock1 == null) degisecek.rockPack.rock1 = eklenecek;
                else if (degisecek.rockPack.rock2 == null) degisecek.rockPack.rock2 = eklenecek;

                if (inventoryID < 60) {
                    CharacterInventory.instance.RemoveItem(old);
                    CharacterInventory.instance.AddItem(degisecek);
                }
                else { // itemin equipment de olduğunu gösterir
                    PlayerInfo.instance.HandInfo.savedItem = degisecek;
                }

                old.rockPack = null;
                RockHandeller.instance.ItemYuva.item = degisecek;
            }
            else if (item is ArmorItem old1)
            {
                Degistir(out ArmorNew degisecek, old1);
                
                degisecek.InventoryId = inventoryID;

                if (degisecek.rockPack == null) {
                    degisecek.rockPack = new RockPack(RockType.armor,default, default,default);
                }

                if (degisecek.rockPack.rock == null) degisecek.rockPack.rock = eklenecek;
                else if (degisecek.rockPack.rock1 == null) degisecek.rockPack.rock1 = eklenecek;
                else if (degisecek.rockPack.rock2 == null) degisecek.rockPack.rock2 = eklenecek;

                if (inventoryID < 60) // BURDAKİ KONTROL ENVANTERDEMİ KARAKTERİN ÜSTÜNDEMİ ONA BAKMAK
                {
                    CharacterInventory.instance.RemoveItem(old1);
                    CharacterInventory.instance.AddItem(degisecek);
                }
                else // itemin equipment de olduğunu gösterir
                {
                    PlayerInfo.instance.ArmorInfo.savedItem = degisecek;
                }
                old1.rockPack = null;
                RockHandeller.instance.ItemYuva.item = degisecek;
            }
            else if (item is ArmorNew old2)
            {
                Debug2.Log("new armır");
                Degistir(out ArmorNew degisecek, old2);
                degisecek.InventoryId = inventoryID;

                if (degisecek.rockPack.rock == null) degisecek.rockPack.rock = eklenecek;
                else if (degisecek.rockPack.rock1 == null) degisecek.rockPack.rock1 = eklenecek;
                else if (degisecek.rockPack.rock2 == null) degisecek.rockPack.rock2 = eklenecek;

                if (inventoryID < 60) { // BURDAKİ KONTROL ENVANTERDEMİ KARAKTERİN ÜSTÜNDEMİ ONA BAKMAK
                    CharacterInventory.instance.RemoveItemInventory(old2);
                    CharacterInventory.instance.AddItem(degisecek);
                }
                else { // itemin equipment de olduğunu gösterir
                    PlayerInfo.instance.ArmorInfo.savedItem = degisecek;
                }
                old2.rockPack = null;
                RockHandeller.instance.ItemYuva.item = degisecek;
            }
            else if (item is SwordNew old3)
            {

                Degistir(out SwordNew degisecek, old3);
                degisecek.InventoryId = inventoryID;

                if (degisecek.rockPack.rock == null) degisecek.rockPack.rock = eklenecek;
                else if (degisecek.rockPack.rock1 == null) degisecek.rockPack.rock1 = eklenecek;
                else if (degisecek.rockPack.rock2 == null) degisecek.rockPack.rock2 = eklenecek;

                if (inventoryID < 60) { // BURDAKİ KONTROL ENVANTERDEMİ KARAKTERİN ÜSTÜNDEMİ ONA BAKMAK
                    CharacterInventory.instance.AddItem(degisecek);
                    CharacterInventory.instance.RemoveItemInventory(old3);
                }
                else { // itemin equipment de olduğunu gösterir
                    PlayerInfo.instance.ArmorInfo.savedItem = degisecek;
                }
                old3.rockPack = null;
                RockHandeller.instance.ItemYuva.item = degisecek;
            }
            
            Debug2.Log("tas eklendi");
            // envanterdeki taşı eksilt
            CharacterInventory.instance.RemoveItem(eklenecek);
         
            RockDatabase.instance.UpdateUI();
        }
    }

    [System.Serializable]
    public class RockPack
    {
        public RockType RockType { get; set; }
        public RockItem rock { get; set; }
        public RockItem rock1 { get; set; }
        public RockItem rock2 { get; set; }

        public bool HasRock()
        {
            return rock != default || rock1 != default || rock2 != default;
        }

        public RockPack(RockType rockType, RockItem rock, RockItem rock1, RockItem rock2)
        {
            RockType = rockType;
            this.rock = rock;
            this.rock1 = rock1;
            this.rock2 = rock2;
        }
    }

    public enum RockPositions
    {
        None, Inventory, Equipment
    }

}