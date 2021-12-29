

using UnityEngine;

[CreateAssetMenu(menuName = "Anil/Enemy/EnemyData")]

public class EnemyData : ScriptableObject
{
    public string Name = "Skeleton";
    // dodge
    public const byte dodgeSpeed = 10;

    public Vector3 attackOffset;
    public Vector3 lookOffset;
}
