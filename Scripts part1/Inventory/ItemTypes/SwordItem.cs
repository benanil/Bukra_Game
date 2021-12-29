
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Items/Sword")]
public class SwordItem : ItemSC, IItem
{
    [Header("Sword")]
    public float AttackSpeed;
    public short Damage;
}

