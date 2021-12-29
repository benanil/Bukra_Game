
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Axe", menuName = "Items/Axe")]
public class AxeItem : ItemSC, IItem
{
    [Header("axe")]
    public float AttackSpeed;
    // sword
    public short WoodCutting;
}

