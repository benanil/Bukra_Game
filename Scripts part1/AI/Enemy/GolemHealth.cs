
using UnityEngine;
using AnilTools;
using System;

namespace Assets.Enemy
{
    public class GolemHealth : HealthBase
    {
        Animator animator;
        System.Random randomSys;

        private void Start()
        {
            randomSys = new System.Random();
            animator = GetComponent<Animator>();
        }

        public override void Die()
        {
            animator.SetTrigger(AnimPool.Die);
        }

        public override bool AddDamage(short damage, bool silent = false , bool fly = false, Vector3 attackerPosition = new Vector3())
        {
            if ((int)Math.Round(randomSys.NextDouble()) == 1)
            {
                animator.SetTrigger((int)Math.Round(randomSys.NextDouble()) == 1 ? AnimPool.GetHit : AnimPool.GetHit1);
            }
            return Health <= 0;
        }

    }
}