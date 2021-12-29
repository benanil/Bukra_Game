using Inventory;
using UnityEngine;

[System.Serializable]
public class ArmorNew : ItemNR, IItem
{
    // Armor
    public float DamageReducing;
    public Mesh ArmorMesh;

    internal float TopDamageReducing
    {
        get
        {
            float damageReducing = DamageReducing;

            if (rockPack.rock)
            {
                damageReducing += rockPack.rock.damageReducing;
            }
            if (rockPack.rock1)
            {
                damageReducing += rockPack.rock1.damageReducing;
            }
            if (rockPack.rock2)
            {
                damageReducing += rockPack.rock2.damageReducing;
            }
            return damageReducing;
        }
    }

    public ArmorNew(float damageReducing, Mesh armorMesh)
    {
        DamageReducing = damageReducing;
        ArmorMesh = armorMesh;
    }
}