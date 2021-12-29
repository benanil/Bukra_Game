using AnilTools;
using Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// görüş alanı ekleniyor
namespace Assets.Resources.Scripts.AI.Enemy
{
    using Random = UnityEngine.Random;

    [SelectionBase]
    [RequireComponent(typeof(Animator))]
    public class CanavarControl : MonoBehaviour
    {
        public static readonly int[] AttackHashset = {
            Animator.StringToHash("Atack4"),  // close attack
            Animator.StringToHash("Atack1"),
            Animator.StringToHash("Atack2"),
            Animator.StringToHash("Atack3"),
            Animator.StringToHash("Atack5") // firlatma gelecek
        };

        // IK
        private const sbyte LookHeight = 2; private const float Weight = 0.7f;
        private const float HeadWeight = 1.2f;

        [Header("Attack")]
        public byte Damage;

        [SerializeField]
        private LayerMask Damagables;

        [SerializeField]
        private Vector2 AttackTimeRandom;//10-20

        private NavMeshAgent nav;
        private EnemyHealth enemyHealth;
        private Animator animator;

        [SerializeField, NonSerialized]
        private Timer AtackTimer;

        [NonSerialized]
        public bool IsInPlayerRange;

        public byte closeRange = 10; public byte MidRange = 20;
        private const byte AlertDistance = 15;

        // mem alloc D
        private int randomInt;

        private Vector3 RandomPos;
        private readonly Vector3 RandomValue = new Vector3(5, 1, 5);

        private bool alerted;

        public bool IsDead => enemyHealth.isDead;

        private Transform target;

        private float distance
        {
            get
            {
                if (target) return target.Distance(transform);
                else return byte.MaxValue; // 200 mt den uzaksa düşmanı görmez
            }
        }

        [SerializeField]
        private byte attackAnimCount = 1;

        //diger kodlar
        public EnemyData enemyData;

        private Rigidbody rig;

        private Vector3 BasePos;

        private bool HasWondering = false;

        public float defaultSpeed = 8;
        private float slowSpeed => defaultSpeed / 2;

        [SerializeField]
        private bool HasDodge, initialized, CanUpdate;

        private CanavarControl[] AlertableEnemies = Array.Empty<CanavarControl>();

        [NonSerialized]
        public float dist;
        System.Random randomSys;
        
        private void OnEnable()
        {
            alerted = false;
        }

        private void Start()
        {
            randomSys = new System.Random();
            AtackTimer = new Timer(AttackTimeRandom.Range(), GetInstanceID());
            RandomPos = transform.position;
            HasWondering = RandomReal.FiftyPercent();

            enemyHealth = GetComponent<EnemyHealth>();
            animator = GetComponent<Animator>();
            rig = GetComponent<Rigidbody>();
            nav = GetComponent<NavMeshAgent>();
            StartCoroutine(WaitAndStart());
            nav.isStopped = false; CanUpdate = true;
        }

        private void Update()
        {
            MonsterUpdate();
        }

        public void Die()
        {
            Destroy(nav);
            CanUpdate = false;
            StopAllCoroutines();
        }

        private void MonsterUpdate()
        {
            if (!CanUpdate) return;
            animator.SetFloat(AnimPool.Blend, nav.velocity.magnitude);

            // enemy yakın
            if (SearchForTarget() || alerted)
            {
                LookTarget();
                if (distance < 1.88f)
                {
                    HasWondering = false;
                    if (AtackTimer.TimeHasCome())
                    {
                        AtackTimer.DefaultTime = AttackTimeRandom.Range();
                        AtackTimer.Reset();
                        if (!CheckRoll())
                        {
                            ChoseAttack();
                            CanUpdate = false;
                        }
                    }
                }
                if (distance > AlertDistance)
                {
                    nav.SetDestination(BasePos);
                    alerted = false;
                }
                else
                {
                    Vector3 direction = (target.position - transform.position).normalized;
                    float distance = Vector3.Distance(transform.position, target.position);

                    // enemy will go 1 mt front of target(player, patrol) if you multiply direction by 2 enemy will go 2 metter front of player
                    nav.SetDestination(transform.position + (direction * distance) - (direction * 2));
                }
            }
            else if (HasWondering) nav.SetDestination(GetrandomBasePos());
        }

        public void AlertWithFrends()
        {
            alerted = true;
            nav.speed = defaultSpeed;
            AlertableEnemies.Foreach(x => x.alerted = true);
        }

        private bool CheckRoll()
        {
            if (HasDodge && distance < 5)
            {
                if (Random.Range(0, 5) == 1)
                {
                    rig.isKinematic = false; rig.useGravity = true;
                    nav.isStopped = true;
                    rig.AddForce(-transform.forward * EnemyData.dodgeSpeed, ForceMode.Impulse);
                    animator.Play(AnimPool.Dodge);
                    StartCoroutine(ReKinematic());
                    return true;
                }
            }
            return false;
        }

        private IEnumerator ReKinematic()
        {
            yield return new WaitForSecondsRealtime(2);
            nav.isStopped = false; rig.isKinematic = true;
            rig.useGravity = false; CanUpdate = true;
        }

        private IEnumerator WaitAndStart()
        {
            //execution Order amaç npcDatabase ve spawn controllerden sonra çalıştırmak
            yield return new WaitForSecondsRealtime(Random.Range(0.4f, .8f));

            AlertableEnemies = SpawnController.instance
                                              .canavars
                                              .FindAll(x =>
                                                           transform.Distance(x.transform.position) < AlertDistance).ToArray();
            BasePos = transform.position;
            if (HasWondering) nav.speed = slowSpeed;
            target = NpcController2.Player.transform;
            initialized = true; CanUpdate = true;
        }

        private bool SearchForTarget()
        {
            if (NpcDatabase.Patrols.Count > 0)
            {
                int closestIndex = 0;
                float closestDistance = float.PositiveInfinity;

                for (int i = 0; i < NpcDatabase.Patrols.Count; i++)
                {
                    float distance = Vector3.Distance(transform.position, NpcDatabase.Patrols[i].transform.position);
                    if (distance < closestDistance)
                    {
                        closestIndex = i;
                        closestDistance = distance;
                    }
                }
                target = NpcDatabase.Patrols[closestIndex].transform;
            }

            if (transform.Distance(NpcController2.Player) < distance)
            {
                target = NpcController2.Player;
            }

            if (!target || distance > MidRange) return false; // target yok yada patrol|player uzak

            if (!alerted)
            {
                if (CharacterMove.instance.GetIsRunning())
                {
                    AlertWithFrends();
                }
                else if (Vector3.Angle((target.position - transform.position).normalized, transform.forward) < 90)
                {
                    AlertWithFrends();
                }
            }
            return alerted;
        }

        private void DelayAndContinueWalk(float delay)
        {
            this.Delay(delay, () =>
            {
                AtackTimer.Reset();
                CanUpdate = true;
            });
        }

        private void LookTarget()
        {
            transform.OnlyYRot(Quaternion.LookRotation(target.position - transform.position));
        }

        private void ChoseAttack()
        {
            if (CharacterHealth.isDead) return; // feature we must chech patrols dead or not
            nav.SetNav(false);
            randomInt = Random.Range(0, AttackHashset.Length);
            animator.SetTrigger(AttackHashset[randomInt]);
            DelayAndContinueWalk(.5f);
        }

        private Vector3 GetrandomBasePos()
        {
            if (Vector3.Distance(RandomPos, transform.position) < .5)
            {
                RandomPos = BasePos + RandomReal.V3Randomizer(RandomValue);
            }
            return RandomPos;
        }

        // anim event
        private void AttackFinish()
        {
            if (!nav) return;
            nav.SetNav(true);
            if (Vector3.Dot((target.position - transform.position).normalized, transform.forward) > 0.3 && distance < 3)
            {
                target.GetComponent<HealthBase>().AddDamage(Damage);
                if ((int)Math.Round(randomSys.NextDouble()) == 1)
                    CameraShake.ShakeOnce(.5f, 5, RandomReal.V3Randomizer(1));
            }
        }

        [ContextMenu("Test")]
        public void Test()
        {
            animator.Play("Fatality");
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (initialized)
            {
                animator.SetLookAtWeight(Weight, 0.5f, HeadWeight, 0, 0.5f);
                if (distance < closeRange && target)
                {
                    animator.SetLookAtPosition(target.transform.position + SVector3.up * LookHeight);
                }
            }
        }
    }
}