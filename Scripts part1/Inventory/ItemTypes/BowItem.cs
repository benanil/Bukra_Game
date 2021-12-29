
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Items/Bow")]
public class BowItem : ItemSC, IItem
{
    [Header("Bow")]
    public float AttackSpeed;
    public short Damage;
}