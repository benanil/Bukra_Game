
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor")]
public class ArmorItem : ItemSC , IItem
{
    [Header("armor")]
    // Armor
    public short DamageReducing;
    public Mesh ArmorMesh;

}

