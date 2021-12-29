
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
public class PickaxeItem : ItemSC, IItem
{
    [Header("PickAxe")]
    public float AttackSpeed;
    /// Pickaxe
    public short Digging;
}

