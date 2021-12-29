#pragma warning disable IDE0051 // animation events uyarısı
using AnilTools;
using AnilTools.Lerping;
using AnilTools.Update;
using System;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Scripts.AI.Enemy
{
    public class GolemAI : EnemyBase
    {
        private static readonly int WallBreaked = Animator.StringToHash("Wall Breaked");
        
        private static GolemAI _instance;
        public static GolemAI instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GolemAI>();
                }
                return _instance;
            }
        }

        [NonSerialized]
        private Vector2 AttackTimeRandom; // mesafeye göre ölçülüyor
        
        public float TurnBackDist = 70;

        [System.Serializable]
        private class Particles
        { 
            [Tooltip("iki elini vurunca çıkan düz particle")]
            public ParticleSystem Attack1Particle;
            [Tooltip("sağ elini vurunca karakter üstünde çıkan particle")]
            public ParticleSystem Attack2Particle;
            public GameObject Attack2Prefab;        //2.atck
            public ParticleSystem rightParticle;
            public ParticleSystem leftParticle;
            public ParticleSystem rightHandParticle;
            public ParticleSystem rockParticle;
            public ParticleSystem BigParticle;
        }

        [SerializeField]
        private Particles particles;

        private readonly int AwakeHash = Animator.StringToHash("Awake");
       
        private Vector3 targetOldPosition;
        [SerializeField]
        private Transform Attack3pos;//3.atck
        
        Transform startPos;
        Transform targetPos;

        public AnimationCurve SpeedCurve;
        [SerializeField] private float speed;

        // character move
        private Rigidbody rig;

        [SerializeField] private float maxSpeed;
        [SerializeField] private float currentSpeed;
        public float RotateSpeed;

        private bool attacking;
        public bool awaken;

        [SerializeField]
        private Transform ThrowRock;
        [SerializeField]
        private Transform RightHand;

        Material golemLight;
        [ColorUsage(true,true)]
        [SerializeField] private Color RagedColor;

        private float PlayerDistance => transform.Distance(NpcController2.Player);
        private float _targetDistance;
        private float TargetDistance
        {
            get
            {
                _targetDistance = transform.Distance(targetPos);

                if (Mathf.Approximately(_targetDistance, 0))
                {
                    _targetDistance = Mathmatic.FloatMaxValue;
                }
                return _targetDistance;
            }
        }

        private float BaseDistance  => transform.Distance(startPos.position);
        private float EnemyDistance => CurrentEnemy.Distance(transform.position);

        private Transform _target;
        private Transform target
        {
            get
            {
                if (_target == tempPos)
                {
                    tempPos.position = Mathmatic.ClosestPointToCircaleAlloc(startPos.position, CurrentEnemy.position, MidRange);
                }
                return _target;            
            }
            set 
            {
                _target = value;
                targetPos = _target;
            }
        }

        private Transform tempPos;
        private Transform CurrentEnemy;
        
        private readonly int SneakingMessage = Animator.StringToHash("Sneaking");
        
        private Vector3 ThrowStartPosition;
        private Vector3 ThrowUpPosition;
        [SerializeField]
        private float RockThrowSpeed;
        [SerializeField] private AnimationCurve ThrowCurve;

        private Collider[] hits;
        
        [ContextMenu("set curve")]
        public void SetCurve()
        { 
            SpeedCurve.MoveKey(1, new Keyframe(200, maxSpeed));
        }

        private void Awake()
        {
            Messenger.AddListener(GolemInFight);// golem savaşa girince tetiklenecek eventler
        }

        private void Start()
        {
            attacking = false;
            golemLight = transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material;
            target = NpcController2.Player;
            // 2 kat intensity azaltır
            golemLight.color /= 4;
        }

        private void Update()
        {
            if (!awaken)
                return;

            if (CurrentEnemy.Distance(startPos) > MidRange || TargetDistance < closeRange)
            {                // hızlanma | yavaşlama
                currentSpeed = Mathmatic.Min(currentSpeed - Time.deltaTime * maxSpeed / 3, 0);
            }
            else
            {
                currentSpeed = Mathmatic.Max(SpeedCurve.Evaluate(currentSpeed + Time.deltaTime * speed) , maxSpeed);
            }
            
            if(!attacking)
            animator.SetFloat(AnimPool.Walk, currentSpeed);
            
            if (!attacking)
            {
                // move
                if (TargetDistance > closeRange) // çok yakınsa yürüme
                {
                    rig.AddForce(transform.forward * currentSpeed * Time.deltaTime, ForceMode.Force);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CurrentEnemy.position - transform.position).OnlyYRot(), RotateSpeed * Time.deltaTime);
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CurrentEnemy.position - transform.position).OnlyYRot(), RotateSpeed / 5 * Time.deltaTime);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Anil/Awake %#w")]
#endif
        public static void StartAwakeMenuItem()
        {
            instance.StartAwake();
        }

        // görev eventiyle açılacak
        [ContextMenu("Awake")]
        public void StartAwake()
        { 
            animator = instance.GetComponent<Animator>();
            rig = instance.GetComponent<Rigidbody>();
            enemyHealth = instance.GetComponent<EnemyHealth>();

            tempPos = new GameObject("GolemTempPos").transform;
            startPos = new GameObject("Golem Start Pos").transform;
            startPos.position = transform.position;
            startPos.rotation = transform.rotation;
            animator.SetTrigger(AwakeHash);

            // 2 kat intensity verir
            this.Delay(2f, () =>
             {
                 golemLight.ColorAnim("_Color",RagedColor, .5f);
             });

            // slider
            transform.GetChild(6).GetComponent<Canvas>().enabled = true;
            transform.GetChild(6).GetComponent<CanvasScaler>().enabled = true;

            var golemrock = GameObject.Find("golem_rock").transform;
            var childs = new Transform[golemrock.childCount];

            for (int i = 0; i < golemrock.childCount; i++)
            {
                childs[i] = golemrock.GetChild(i);

                StartCoroutine(golemrock.GetChild(i).ShakeCoroutine(8, .2f, 1, x =>
                {
                    var rig = x.GetComponent<Rigidbody>();
                    rig.isKinematic = false;
                    rig.AddForce(RandomReal.V3Randomizer(300) + Vector3.up * 20, ForceMode.Impulse);
                }));
            }

            this.Delay(2f, () =>
             {
                 RegisterUpdate.UpdateWhile(
                      () =>
                      {
                          for (int x = 0; x < childs.Length; x++)
                          {
                              childs[x].localScale += Vector3.one * -80 * Time.deltaTime;
                          }
                      },
                      () => childs[0].localScale.magnitude > 10, () => childs.Foreach(x => Destroy(x.gameObject)));
             });


            this.Delay(5f, () =>
             {
                 childs.Foreach(x =>
                 {
                     x.GetComponent<MeshRenderer>().material.AlphaFade(0, .4f);
                 });
             });
        }

        public const byte GolemInFight = 1;

        // uyanma animasyonu sonu çalışır
        private void Awaken()
        {
            AtackTimer = new Timer(0);
            AtackTimer.Start();
            StartCoroutine(WonderingUpdate());
            rig.isKinematic = false;
            target = NpcController2.Player;
            awaken = true;
            Messenger.SendMessage(GolemInFight);
        }

        protected override IEnumerator WonderingUpdate()
        {
            while (true)
            {
                //enemy yakın

                if (SearchForTarget()){ 
                    if (TargetDistance > TurnBackDist)
                        AtackTimer.Reset();

                    if (AtackTimer.TimeHasCome()) {
                        attacking = true;
                        animator.LerpAnim(AnimPool.Walk, 0, 2,ChoseAttack , 40);
                        break;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        protected override bool SearchForTarget()
        {
            CurrentEnemy = NpcDatabase.Patrols.OrderBy(x => x.transform.Distance(transform)).First().transform;

            if (!Sneaking)
            {
                CurrentEnemy = CurrentEnemy.Distance(transform) < transform.Distance(NpcController2.Player) ? CurrentEnemy : NpcController2.Player;
                if (!Messenger.GetMessage(WallBreaked))
                if (NpcController2.Player.Distance(startPos) < 20) {// mağraya yaklaşınca
                    StopCoroutine(WonderingUpdate());
                    targetPos = NpcController2.Player;
                    CurrentEnemy = NpcController2.Player;
                    animator.SetTrigger(AttackHashset[2]);
                    Messenger.SendMessage(SneakingMessage);
                    Messenger.SendMessage(WallBreaked);
                    return false;
                }
            }

            if (EnemyDistance > TurnBackDist) {                                                 
                target = startPos;                            
                AtackTimer.Reset();                           
                return false;                                 
            }                                                 
            else if(EnemyDistance > MidRange){
                tempPos.position = Mathmatic.ClosestPointToCircaleAlloc(startPos.position, target.position, MidRange);// mid range dışına çıkmayan pozisyon
                target = tempPos;
                return true;
            }
            else {
                target = CurrentEnemy;
            }

            return target;
        }

        private void ChoseAttack()
        {
            currentSpeed = 0;

            if (target == tempPos)
            {
                target = tempPos;
            }
            else if (target != CurrentEnemy)
            {
                target = startPos;
            }

            targetOldPosition = CurrentEnemy.position;

            animator.SetFloat(AnimPool.Walk, 0);

            if(EnemyDistance < closeRange)            animator.SetTrigger(AttackHashset[0]);
            else if (EnemyDistance < closeRange + 10) animator.SetTrigger(AttackHashset[1]);
            else if (EnemyDistance < closeRange + 20) animator.SetTrigger(AttackHashset[3]);
            else     animator.SetTrigger(AttackHashset[RandomReal.Range(0,2) == 1 ? 2 : 4]); 

            AtackTimer.DefaultTime = 0.65f;
        }

        private void LeftFoot()
        {
            particles.leftParticle.Play();
            CameraShake.ShakeOnce(1, 4, RandomReal.V3Randomizer(Vector3.one));
        }

        private void RightFoot()
        {
            particles.rightParticle.Play();
            CameraShake.ShakeOnce(1, 3, RandomReal.V3Randomizer(Vector3.one));
        }

        // attack 1 anim eventi
        private void Attack1()
        {
            particles.Attack1Particle.Play();

            GenerateOverlapSphere(particles.Attack1Particle.transform.position, 20 , Damagables , Damage,fly:true, GroundCrack: true,attackerPosition: transform.position);
            DelayAndContinueWalk(.5f);
            Shake();
        }

        // attack 2 anim eventi
        private void Attack2(){
            if (Messenger.GetMessage(SneakingMessage)) {
                Messenger.RemoveMessage(SneakingMessage);
                particles.BigParticle.gameObject.SetActive(true);
                particles.BigParticle.Play();
                GenerateOverlapSphere(startPos.position, 45, Damagables, (short)(Damage * 3), true);
            }
            else{
                particles.Attack2Prefab.SetActive(true);
                particles.Attack2Prefab.transform.position = targetOldPosition;
                particles.Attack2Particle.Play();
                this.Delay(1f, () => particles.Attack2Prefab.SetActive(false));
                GenerateOverlapSphere(particles.Attack2Prefab.transform.position, 15 , Damagables, Damage,fly:true,GroundCrack:true, attackerPosition: transform.position);
            }

            DelayAndContinueWalk(.5f);
            Shake();
        }

        // attack 3 anim eventi
        private void Attack3()
        {
            GenerateOverlapSphere(Attack3pos.position, 15 , Damagables, Damage, fly: true, GroundCrack: true, attackerPosition: transform.position);
            DelayAndContinueWalk(.5f);
            Shake();
        }

        /// <summary>close attack</summary>
        private void Attack4()
        {
            GenerateOverlapSphere(RightHand, 15, Damagables, Damage,lifeTime:2.5f, GroundCrack: true, attackerPosition: transform.position);
            DelayAndContinueWalk(.5f);
            Shake();
        }

        /// <summary>Throw rock</summary>
        private void Attack5()
        {
            ThrowRock.SetParent(null);

            targetOldPosition = CurrentEnemy.position + Vector3.down * 6;

            var distance = Vector3.Distance(RightHand.position,targetOldPosition);
            var ray  = new Ray(RightHand.position, (RightHand.position - targetOldPosition).normalized);
            
            var task = ThrowRock.CubicLerpAnim(RightHand.position, ray.GetPoint(distance / 2) + Vector3.up * distance / 3, targetOldPosition, RockThrowSpeed ,
                                               ThrowCurve, () => 
                                               {
                                                   particles.rockParticle.Play();
                                                   GenerateOverlapSphere(ThrowRock.position, 3, Damagables , Damage , fly:true , GroundCrack: true, attackerPosition: transform.position);
                                               });
           
            DelayAndContinueWalk(2);
        }

        private void RightHandParticle()
        {
            particles.rightHandParticle.Play();
            ThrowRock.GetComponent<Rigidbody>().isKinematic = true;
            ThrowRock.SetParent(RightHand);
            ThrowRock.localPosition = new Vector3(0.0126f, -0.01657f, 0.0229199994f);
        }

        private bool Sneaking;

        public void SneakingEnter(bool value)
        {
            Sneaking = value;
        }

        private void DelayAndContinueWalk(float delay)
        {
            new Timer(delay, () =>
            {
                currentSpeed = 0;
                AtackTimer.Reset();
                StartCoroutine(WonderingUpdate());
            });
            attacking = false;
        }

        private void Shake()
        { 
            CameraShake.ShakeOnce(1, 5, RandomReal.V3Randomizer(Vector3.one * 2));
            Vibration.Vibrate(50);
        }

        public void OnDrawGizmos()
        {
            /*
            if (startPos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(startPos.position, closeRange);
                Gizmos.color = new Color(.5f, .5f , 0);
                Gizmos.DrawWireSphere(startPos.position, closeRange + 20);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(startPos.position, MidRange);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(startPos.position, TurnBackDist);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, closeRange);
                Gizmos.color = new Color(.5f, .5f, 0);
                Gizmos.DrawWireSphere(transform.position, closeRange + 20);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, MidRange);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, TurnBackDist);
            }

            if (targetOldPosition != default)
            {
                var ray = new Ray(RightHand.position, (RightHand.position - targetOldPosition).normalized);
                Gizmos.DrawLine(ray.origin,ray.GetPoint(-20));
            }
            */
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(GolemAI))]
    public class GolemEditor : Editor
    {

    }
#endif

}