
using Inventory;

public class SwordNew : ItemNR, IItem
{
    public float AttackSpeed;
    public short Damage;

    internal float TopAttackSpeed
    {
        get
        {
            float attackSpeed = AttackSpeed;

            if (rockPack.rock)
            {
                attackSpeed += rockPack.rock.AttackSpeed;
            }
            if (rockPack.rock1)
            {
                attackSpeed += rockPack.rock1.AttackSpeed;
            }
            if (rockPack.rock2)
            {
                attackSpeed += rockPack.rock2.AttackSpeed;
            }
            return attackSpeed;
        }
    }

    internal float TopDamage
    {
        get
        {
            float TopDamage = Damage;

            if (rockPack.rock)
            {
                TopDamage += rockPack.rock.damage;
            }
            if (rockPack.rock1)
            {
                TopDamage += rockPack.rock1.damage;
            }
            if (rockPack.rock2)
            {
                TopDamage += rockPack.rock2.damage;
            }
            return TopDamage;
        }
    }

    public SwordNew( float attackSpeed, short damage)
    {
        AttackSpeed = attackSpeed;
        Damage = damage;
    }
}
