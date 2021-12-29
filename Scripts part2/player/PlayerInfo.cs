using AnilTools;
using Horse;
using Inventory;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : Singleton<PlayerInfo>
{
    private const string LevelUp = "Level Up";

    // Level ile alakalı
    private static short _level;

    public static short Level
    {
        get
        {
            return _level;
        }
        set
        {
            if (Time.timeSinceLevelLoad > 5)
            {
                instance.LevelUpParticle.gameObject.SetActive(true);
                instance.LevelUpParticle.Play();
            }
            _level = value;
        }
    }

    public static int Exp;

    // Equipment ile alakalı
    public static float AttackSpeed = 0.7f, DamageReducing = 0;

    public static short AttackDamage = 20, Digging = 0, WoodCutting, RequiredEXP = 500;

    public static bool PlayerOnHorse, Sliding;

    [SerializeField] private Text CurrentLevel;

    public static LayerMask Damagables;

    public ParticleSystem LevelUpParticle;

    // patrols
    public byte FallowingPatrols;

    private readonly List<NpcController2> ActivePatrols = new List<NpcController2>();

    [NonSerialized]
    public Sprite HandSprite;

    [Space]
    [Header("Equipments")]
    [SerializeField]
    // Hands
    private Transform RightHand;

    [SerializeField]
    private Transform LeftHand;

    // Armor
    public SkinnedMeshRenderer armor;

    public Mesh DefaultArmor;
    public Material specularMaterial;
    public Material DefaultMaterial;

    [Space]
    public EquipmentInfo HandInfo;

    public EquipmentInfo ArmorInfo;

    public static HorseAI CurrentHorse;

    public SkinnedMeshRenderer swordRenderer;
    public GameObject swordBack;

    public ItemSC TasKılıc;
    internal Animator animator;

    public static bool WeaponActive;
    
    /// <summary> plays sword draw animation </summary>
    /// <returns>can we draw sword, if (already drawed | has no item) returns false </returns>
    internal static bool DrawSword() {
        if (!HasHandItem() || WeaponActive) return false;
        instance.animator.Play("SwordTake");
        CharacterMove.SetPlayer(false);
        instance.Delay(1.5f, () => CharacterMove.SetPlayer(true));
        return true;
    }
    
    /// <summary> plays sword replace animation </summary>
    /// <returns>can we replace sword, if (already replace | has no item) returns false </returns>
    internal static bool  ReplaceSword() {
        if (!HasHandItem() || !WeaponActive) return false;
        instance.animator.Play("SwordTake");
        CharacterMove.SetPlayer(false);
        instance.Delay(1.5f, () => CharacterMove.SetPlayer(true));
        return true;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        this.Delay(2f, () =>
        {
            CharacterInventory.instance.AddItem(TasKılıc.Id);
            ToolTip.instance.currentItem = TasKılıc;
            ToolTip.instance.EquipItem();
        });
    }

    private void Update()
    {
        // todo add vissle anim and sound
        if (CurrentHorse)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("vissle");
                CurrentHorse.Vissle();
            }
        }
    }

    public static bool HasHandItem()
    {
        return instance.HandInfo.savedItem != null;
    }

    /// <summary>
    /// exp ekler
    /// </summary>
    /// <param name="value">exp değerini giriniz</param>
    public void AddExp(int value)
    {
        Exp += value;

        if (Exp >= RequiredEXP)
        {
            RequiredEXP += 500;
            Level++;
            CanvasManager.instance.ShowTxt(Header.H1, LevelUp, Color.blue);
            CurrentLevel.text = "Level " + Level.ToString();
        }
    }

    // Animation event hides or wears sword
    public void HideWeapon()
    {
        ToggleWeapon(CombatControl.PlayerInDanger());
    }
    
    public static void ToggleWeapon(bool value)
    {
        Debug.LogError("Toggle: " + value);
        instance.swordBack.SetActive(!value);
        instance.swordRenderer.gameObject.SetActive(value);
        WeaponActive = value;
    }


    // horse anim event listenners
    public void MountEnd()
    {
        PlayerOnHorse = true;
        CurrentHorse.MountEnd();
    }

    public void DismountEnd()
    {
        PlayerOnHorse = false;
        CurrentHorse.DismountEnd();
    }

    // -- PATROL --
    public byte AddPatrol(NpcController2 newPatrol)
    {
        if (!ActivePatrols.Contains(newPatrol))
        {
            ActivePatrols.Add(newPatrol);
        }
        FallowingPatrols++;
        return (byte)(FallowingPatrols - 1);
    }

    public void RemovePatrol(NpcController2 removedPatrol)
    {
        ActivePatrols.Remove(removedPatrol);
        FallowingPatrols--;
    }

    public void MoveOrStop()
    {
        for (int i = 0; i < ActivePatrols.Count; i++)
        {
            ActivePatrols[i].MoveOrStop();
        }
    }

    // -- ITEM --
    public void EquipItem(ItemSC item)
    {
        HandSprite = item.icon;
        Damagables = item.Damagables;

        switch (item.currentClass)
        {
            case ItemClass.HandTool:
                swordRenderer.sharedMesh = item.mesh;
                HandInfo.AddItem(item);
                swordRenderer.gameObject.SetActive(true);
                swordBack.SetActive(false);
                WeaponActive = true;
                if (item is SwordItem sword)
                {
                    Damagables = sword.Damagables;
                }
                break;

            case ItemClass.Armor:
                var _armor = ItemSC.CastItem<ArmorItem>(item);
                armor.sharedMesh = _armor.ArmorMesh;
                armor.sharedMaterial = specularMaterial;
                ArmorInfo.AddItem(item);
                break;
        }
    }
    public void EquipItem(ItemNR item)
    {
        HandSprite = item.icon; Damagables = item.Damagables;

        switch (item)
        {
            case SwordNew swordNew:
                swordRenderer.gameObject.SetActive(true);
                swordRenderer.sharedMesh = item.mesh;
                HandInfo.AddItem(swordNew);
                Damagables = swordNew.Damagables;
                WeaponActive = true;
                break;

            case ArmorNew armorNew:
                armor.sharedMesh = armorNew.ArmorMesh;
                armor.sharedMaterial = specularMaterial;
                ArmorInfo.AddItem(armorNew);
                break;
        }

    }

    public void RemoveItem(ItemSC item)
    {
        HandSprite = item.icon;
        if (item.currentClass == ItemClass.HandTool) Damagables = (LayerMask)0;

        switch (item)
        {
            case SwordItem swordItem:
                swordRenderer.sharedMesh = item.mesh;
                AttackDamage -= swordItem.Damage;
                WeaponActive = false;
                break;

            case ArmorItem _:
                armor.sharedMesh = DefaultArmor;
                armor.sharedMaterial = DefaultMaterial;
                break;

            case BowItem bowItem:
                AttackDamage -= bowItem.Damage;
                swordRenderer.gameObject.SetActive(false);
                break;

            case AxeItem axe:
                swordRenderer.gameObject.SetActive(false);
                break;

            case PickaxeItem pickaxeItem:
                swordRenderer.gameObject.SetActive(false);
                break;
        }
    }

    public void RemoveItem(ItemNR item)
    {
        HandSprite = item.icon;
        if (item.currentClass == ItemClass.HandTool) Damagables = (LayerMask)0;

        switch (item.currentClass)
        {
            case ItemClass.HandTool:
                swordRenderer.gameObject.SetActive(false);
                WeaponActive = false;
                break;

            case ItemClass.Armor:
                armor.sharedMesh = DefaultArmor;
                armor.sharedMaterial = DefaultMaterial;
                break;
        }
    }
}