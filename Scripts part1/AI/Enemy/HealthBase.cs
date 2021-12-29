
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBase : MonoBehaviour
{
    protected static readonly int Dead = Animator.StringToHash("Dead");

    public short Health;
    public bool isDead => Health < 0;

    /// <summary>
    /// returns is dead
    /// </summary>
    public virtual bool AddDamage(short damage,bool silent = false, bool fly = false , Vector3 AttackerPosition = new Vector3())
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            return true;
        }
        else { return false; }
    }

    public virtual void Die()
    {
        
    }
}

