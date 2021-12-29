using AnilTools;
using AnilTools.Save;
using Dialog;
using Inventory;
using Player;
using Rock;
using Skill;
using System;
using UnityEngine;
using SaveableObjects;
using Horse;
using Assets.Scripts;

[RequireComponent(typeof(GameMenu),typeof(DialogControl))]
public class XMLmanager : Singleton<XMLmanager>
{
    private const string PlayerPath = "Player/";
    private const string CharacterName = "PlayerStats.json";
    private static string SavesFolder => Application.persistentDataPath + "/Saves/";

    [HideInInspector] public Quaternion PlayerRot;
    [HideInInspector] public Transform PlayerPos;

    public static byte graphic = 2;

    public ItemEntry list;

    private void Awake()
    {
        // initialize
        PlayerPos = NpcController2.Player.transform;
    }

    private void Start()
    {
        Invoke(nameof(LoadVeriables), 0.5f);
    }

    public void SaveVeriables()
    {
        //kaydetmek istediğimiz veriler
        list = new ItemEntry
        {
            playerPos = PlayerPos.position,
            PlayerRot = PlayerPos.rotation,
            health = 100,
            para = DialogControl.instance.Money,
            //menu
            Senstivity = GameMenu.instance.Sens.value,
            ses = GameMenu.instance.volume.value,
            graphic = graphic,
            // Player Info
            Level = PlayerInfo.Level,
            Exp = PlayerInfo.Exp,
            RequiredEXP = PlayerInfo.RequiredEXP,
            InventoryCount = UIInventory.instance.SlotSayısı,

            // mission
            CurrentMission = (byte)MissionHandeller.instance.CurrentMision,

            //ınventory
            itemData = CharacterInventory.InventoryItems.ToArray(),
            horsePos = PlayerInfo.CurrentHorse ? PlayerInfo.CurrentHorse.transform.position : Vector3.zero,
            horseAI = PlayerInfo.CurrentHorse,
        };

        // skill
        SkillMenuHandeller.instance.Save();

        // rock
        RockSaveManager.instence.Save();

        //equipment
        list.HandTool = PlayerInfo.instance.HandInfo.savedItem as ItemSC;
        list.Armor = PlayerInfo.instance.ArmorInfo.savedItem as ItemSC;

        // saveableObjects
        SaveableObjectManager.instance.Save();

        JsonManager.Save(list, PlayerPath, CharacterName);
        StartCoroutine(SubtitleManager.instance._ShowImmediate(Language.GameSaved,null));
    }

    private void LoadVeriables()
    {
        Debug2.Log("Json dan çekmeye çalışır",Color.blue);

        list = JsonManager.Load<ItemEntry>(PlayerPath, CharacterName);

        if (list == default)
        {
            MissionHandeller.instance.UpdateMission(MainMisions.CollectMushrooms);
            MissionHandeller.instance.OnJsonLoaded();

            UIInventory.instance.AddSlot(10);
            return;
        }
        
        /// mission
        MissionHandeller.instance.UpdateMission((MainMisions)list.CurrentMission);
        MissionHandeller.instance.OnJsonLoaded();

        /// inventory
        CharacterInventory.instance.RemoveAllItems();
        UIInventory.instance.RemoveAllSlots();
       
        if (list.InventoryCount == 0)
        UIInventory.instance.AddSlot(10);
        else
        UIInventory.instance.AddSlot(list.InventoryCount);

        for (short i = 0; i < list.itemData.Length; i++){
            for (short x = 0; x < list.itemData[i].adet; x++){
                CharacterInventory.instance.AddItem(list.itemData[i].id);
            }
        }
        
        /// equipment
        if (list.Armor != null){
            ToolTip.instance.currentItem = list.Armor;
            ToolTip.instance.EquipItem();
        } 

        if (list.HandTool != null){
            ToolTip.instance.currentItem = list.HandTool;
            ToolTip.instance.EquipItem();
        }

        // Player Info
        PlayerInfo.Level = list.Level;
        PlayerInfo.Exp = list.Exp;
        PlayerInfo.RequiredEXP = list.RequiredEXP;
        
        // skill
        SkillMenuHandeller.instance.Load();

        // rock
        RockSaveManager.instence.Load();

        // game manager
        GameMenu.instance.languages.currentLanguage = (AvibleLanguages)list.language;
        GameMenu.instance.LoadVeriables();

        if (list.playerPos != Vector3.zero)
        {
            CharacterMove.SetPlayer(false);
            NpcController2.Player.transform.SetPositionAndRotation(list.playerPos, list.PlayerRot);
            CharacterMove.SetPlayer(true);
        }

        // saveableObjects
        SaveableObjectManager.instance.Load();

        MissionHandeller.instance.UpdateUI();

        // horse
        if (list.horseAI)
        if (list.horsePos != Vector3.zero)
        {
            list.horseAI.GetComponent<CharacterController>().enabled = false;
            list.horseAI.enabled = false;
            list.horseAI.transform.position = list.horsePos;
            list.horseAI.GetComponent<CharacterController>().enabled = true;
            list.horseAI.enabled = true;
        }
    }

    [Serializable]
    public class ItemEntry
    {
        // Buraya İstediğin Değeri Girebilirsin
        // Ayarlar
        public float Senstivity;
        public uint graphic = 2;
        public float ses;

        // Player
        public Quaternion PlayerRot;    //oyuncunun rotasyonunu ayarlar 
        public Vector3 playerPos;       //oyuncunun pozisyonunu ayarlar 
        public Vector3 horsePos;        //atın pozisyonunu kaydeder
        public HorseAI horseAI;
        public short health;            //sağlığı ayarlar
        public int para;                //para

        // Items
        // id ve adet
        public ItemData[] itemData;
        public ItemSC HandTool;
        public ItemSC Armor;

        // Mission
        public byte CurrentMission;

        // Player Info
        public short Level;
        public int Exp;

        public short RequiredEXP;
        public byte InventoryCount;
        public byte language;
    }

}

[Serializable]
public struct ItemData
{
    public byte id;
    public byte adet;

    public ItemData(byte id, byte adet)
    {
        this.id = id;
        this.adet = adet;
    }
}
