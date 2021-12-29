using AnilTools;
using AnilTools.Update;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Resources.Scripts.AI.Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        public static readonly List<int> AttackHashset = new List<int>
        {
            Animator.StringToHash("Atack4"),  // close attack
            Animator.StringToHash("Atack1"),
            Animator.StringToHash("Atack2"),
            Animator.StringToHash("Atack3"),
            Animator.StringToHash("Atack5") // firlatma gelecek
        };

        // IK
        protected const sbyte LookHeight = 2;

        protected const float Weight = 0.7f;
        protected const float HeadWeight = 1.2f;

        [Header("Attack")]
        public byte Damage;

        [SerializeField]
        protected LayerMask Damagables;

        [SerializeField]
        protected Vector2 AttackTimeRandom;//10-20

        protected NavMeshAgent nav;
        protected EnemyHealth enemyHealth;
        protected Animator animator;

        [SerializeField, NonSerialized]
        protected Timer AtackTimer;

        public byte closeRange = 10;
        public byte MidRange = 20;

        protected readonly WaitForSecondsRealtime UpdateTime = new WaitForSecondsRealtime(.3f);

        // mem alloc D
        protected int randomInt;

        protected Vector3 RandomPos;
        protected readonly Vector3 RandomValue = new Vector3(5, 1, 5);

        protected bool alerted;

        [NonSerialized]
        public bool IsDead;

        protected Transform target;

        protected float distance
        {
            get
            {
                if (target) return target.Distance(transform);
                else return Mathmatic.FloatMaxValue;
            }
        }

        protected virtual bool SearchForTarget()
        {
            target = NpcDatabase.Patrols.OrderBy(x => transform.Distance(x.transform.position)).First().transform;

            if (transform.Distance(NpcController2.Player) < distance)
                target = NpcController2.Player;

            if (distance > MidRange)
            {
                target = null;
            }

            return target;
        }

        protected void DelayAndContinueWalk(float delay)
        {
            new Timer(delay, () =>
            {
                AtackTimer.Reset();
                StartCoroutine(WonderingUpdate());
            });
        }

        protected virtual IEnumerator WonderingUpdate()
        {
            yield return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateOverlapSphere(Vector3 Pos, byte radius, LayerMask layerMask, short Damage, bool allowMultiple = true, Action<Vector3> HitEvent = null,
                                                 Action DeadEvent = null, in float lifeTime = 0.4f, bool fly = false, in bool GroundCrack = false, Vector3 attackerPosition = new Vector3())
        {
            bool DamageColliderActivity = true;
            Collider[] hitColliders = new Collider[30];

            Pos += Vector3.up * radius;

            new Timer(lifeTime, () => DamageColliderActivity = false);

            RegisterUpdate.UpdateWhile(
            delegate
            {
                int hitCount = Physics.OverlapSphereNonAlloc(Pos, radius, hitColliders, layerMask, QueryTriggerInteraction.Ignore) - 1;

                while (hitCount > 0)
                {
                    Collider collider = hitColliders[hitCount];
                    if (collider.TryGetComponent(out HealthBase healthBase))
                    {
                        HitEvent?.Invoke(healthBase.transform.position);

                        if (healthBase.AddDamage(Damage, false, fly, attackerPosition))
                        {
                            DeadEvent?.Invoke();
                        }

                        DamageColliderActivity = false;
                        if (!allowMultiple)
                            break;
                    }
                    hitCount--;
                }
            },
            delegate
            {
                return DamageColliderActivity == true;
            },
            updateType: UpdateType.fixedTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateOverlapSphere(Transform transform, byte radius, LayerMask layerMask, short Damage, bool allowMultiple = true, Action<Vector3> HitEvent = null,
                                                 Action DeadEvent = null, float lifeTime = 0.4f, bool fly = false, bool GroundCrack = false, Vector3 attackerPosition = new Vector3())
        {
            bool DamageColliderActivity = true;
            Collider[] hitColliders = new Collider[30];

            new Timer(lifeTime, () => DamageColliderActivity = false);

            RegisterUpdate.UpdateWhile(
            delegate
            {
                int hitcount = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, layerMask, QueryTriggerInteraction.Ignore) - 1;

                while (hitcount > 0)
                {
                    Collider collider = hitColliders[hitcount];
                    if (collider.TryGetComponent(out HealthBase healthBase))
                    {
                        HitEvent?.Invoke(healthBase.transform.position);

                        if (healthBase.AddDamage(Damage, false, fly, attackerPosition))
                        {
                            DeadEvent?.Invoke();
                        }

                        DamageColliderActivity = false;
                        if (!allowMultiple) break;
                    }
                    hitcount--;
                }
            },
            delegate
            {
                return DamageColliderActivity == true;
            },
            updateType: UpdateType.fixedTime);
        }
    }
}