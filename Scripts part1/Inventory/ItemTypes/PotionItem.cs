
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Items/potion")]
public class PotionItem : ItemSC, IItem
{
    [Header("potion")]
    public short Healing;
}

