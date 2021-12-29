
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "Eating", menuName = "Items/food")]
public class EatingItem : ItemSC, IItem
{
    [Header("food")]
    public short doyuruculuk;
}
