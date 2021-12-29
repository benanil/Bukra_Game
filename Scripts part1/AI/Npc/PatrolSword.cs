using Assets.Resources.Scripts.AI.Enemy;
using UnityEngine;
using static GameConstants;

[RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
public class PatrolSword : MonoBehaviour
{
    [SerializeField] private short damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            other.GetComponent<EnemyHealth>().AddDamage(damage,true);
        }
    }
}
