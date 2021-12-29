using AnilTools;
using Assets.Resources.Scripts.AI.Enemy;
using Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UrFairy;

public enum HandState
{
    Punch = 0, Sword, Pickaxe,
    Axe, BOW
}

namespace Player
{
    using Sstring = StringOperationUtil.OptimizedStringOperation;

    public partial class CombatControl : Singleton<CombatControl>
    {
        private void Awake()
        {
            handState = 0;
            HeadLookTemp = new GameObject("Head Temp").transform;
        }

        // now we dont need to big collider on top of the player
        // this void checks enemies and make list of them
        private void EnemyDetect()
        {
            const float CloseEnemyDistance = 20;

            CurrentEnemy = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < SpawnController.instance.canavars.Count; i++)
            {
                CanavarControl enemy = SpawnController.instance.canavars[i];
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < CloseEnemyDistance)
                {
                    if (distance < minDistance)
                    { // sellect closest enemy to target
                        minDistance = distance;
                        CurrentEnemy = enemy.transform;
                    }

                    if (enemy.IsInPlayerRange == false && !PlayerInfo.PlayerOnHorse) // trigger enter
                    {
                        CloseEnemies.Add(enemy.transform);

                        if (CloseEnemies.Count == 1) // if we close to enemy draw your sword
                        {
                            PlayerInfo.DrawSword();
                        }
                        else {
                            animator.SetBool(AnimPool.InCombat, true);
                        }
                    }
                }
                else
                {
                    if (enemy.IsInPlayerRange == true)
                    { // trigger exit
                        CloseEnemies.Remove(enemy.transform);
                        if (CloseEnemies.Count == 0 && !PlayerInfo.PlayerOnHorse)
                        {
                            CurrentEnemy = null;
                            animator.SetBool(AnimPool.InCombat, false);
                        }
                    }
                }

                enemy.IsInPlayerRange = distance < CloseEnemyDistance;
            }

        }
        public static bool PlayerInDanger() => instance.CloseEnemies.Count > 0;
        /// <returns>-1 or 1</returns>
        public static int PlayerInDangerInt() => instance.CloseEnemies.Count > 0 ? 1 : -1;

        private void Update()
        {
            EnemyDetect();
            if (Input.GetKeyDown(KeyCode.Alpha1)) SkillButton.IfJust(x => x.OnPointerClick(null));
            if (Input.GetKeyDown(KeyCode.Alpha2)) SkillButton1.IfJust(x => x.OnPointerClick(null));
            if (Input.GetKeyDown(KeyCode.Alpha3)) SkillButton2.IfJust(x => x.OnPointerClick(null));

            if (Input.GetMouseButtonDown(0) && !GameMenu.PlayerOnMenu)
            {
                Atract(); // vuruş yapmayı sol mouse a atar
            }

            attackTime -= Time.deltaTime;
            AttackStateTime -= Time.deltaTime;
            comboDelay -= Time.deltaTime;

            if (comboDelay < 0)
            {
                //comboTXT.text = string.Empty;
                // SkillButton.SetActive(false);
                // SkillButton1.SetActive(false);
                // SkillButton2.SetActive(false);
            }

            if (attackTime < 0)
            {
                canAttack = true;
                attackTime = attackSpeed;
            }
        }

        private void Start()
        {

            camera = Camera.main.transform;

#if UNITY_EDITOR
            debugObj = new GameObject("Debug");
#endif

            OverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = OverrideController;

            CurrentAttackAnimations = DefaultAttackAnimations;
        }

        public void UpdateHandState(HandState handState)
        {
            HandState = handState;
            animator.SetInteger(HandStateStr, (int)handState);
        }

        private Coroutine atractDelayRoutine;
        public GameObject trail;
        Coroutine trailCoroutine;
        
        public void Atract()
        {
            if (!canAttack || PlayerInfo.PlayerOnHorse || GameMenu.PlayerOnMenu || !PlayerInfo.WeaponActive || !CurrentEnemy) return;

            if (atractDelayRoutine != null) StopCoroutine(atractDelayRoutine);

            IncrementComboState();

            canAttack = false;
            
            float dist = Vector3.Distance(transform.position, CurrentEnemy.position);
            
            if (CloseEnemies.Count == 1) // lastEnemy standing
            {                            // combo attack animation
                var enemyHealth = CurrentEnemy.GetComponent<EnemyHealth>();
                bool canCombo = true;//Vector3.Dot(transform.forward, enemyHealth.transform.forward) > 00f;
                if (enemyHealth.Health <= PlayerInfo.AttackDamage && canCombo && dist > 1) // enemy health is to low
                {
                    Debug.LogError("Combo Started");
                    CharacterMove.SetPlayer(false); // player can't move when combo 
                    CharacterMove.instance.ComboCamera(); // camera zoom effect

                    Vector3 dirToEnemy = (transform.position - CurrentEnemy.position).normalized;
                    
                    transform.eulerAngles = transform.eulerAngles.OnlyY(Mathf.Atan2(dirToEnemy.x, dirToEnemy.z) * Mathf.Rad2Deg + 180) ;
                    // transform.position -= transform.forward;

                    this.DelayNew(2f, () => CharacterMove.SetPlayer(true));

                    animator.SetBool(AnimPool.InCombat, false);
                    animator.Play("Combo1");

                    this.DelayNew(11f, () => { // replace sword back combat ended 
                        PlayerInfo.ReplaceSword();
                    });

                    enemyHealth.RequiredFatalitiyDist = -dist;
                    enemyHealth.AddDamage(PlayerInfo.AttackDamage, fly: true);
                    CloseEnemies.Clear(); // last player died clear the list
                }
                else // enemy is not die use normal attack
                {
                    animator.SetTrigger(attack);
                }
            }
            else // normal attack animation 
            {
                animator.SetTrigger(attack);
                if (dist > 1.66f) 
                CharacterMove.instance.Roll(camera.forward, .3f);
            }

            IEnumerator TrailCoroutine()
            {                           // Sword trail effect
                trail.SetActive(true);
                trail.GetComponent<ParticleSystem>().Play();
                yield return new WaitForSeconds(1);
                trail.SetActive(false);
            }

            if (trailCoroutine != null) StopCoroutine(trailCoroutine);
            trailCoroutine = StartCoroutine(TrailCoroutine());

            CharacterMove.SpeedMultiplier = .5f; // when attacking we want our character slow a bit

            atractDelayRoutine = this.DelayNew(1f, () => CharacterMove.SpeedMultiplier = 1f);

            OverrideController[replacableAttackAnim] = CurrentAttackAnimations[(int)Mathf.Repeat(AttackState, CurrentAttackAnimations.Length)];
            animator.runtimeAnimatorController = OverrideController;

            AttackState++;
        }

        private void IncrementComboState()
        {
            if (AttackStateTime <= 0) AttackState = 0;
            AttackStateTime = AttackStateTimeDefault;

            if (handState == HandState.Sword)
            {
                if (comboDelay > 0)
                {
                    comboState += 1;
                    //comboTXT.text = Sstring.tiny + comboState.ToString() + Language.X;
                    ComboTxtAnimator.SetTrigger(Hit);
                }
                else
                {
                    comboDelay = comboSpeed;
                    comboState = 0;
                    //comboTXT.text = string.Empty;
                }
                if (comboState >= 3)
                {
                    SkillButton.SetActive(true);
                    SkillButton1.SetActive(true);
                    SkillButton2.SetActive(true);
                }
            }
        }

        private int AttackState;
        private float AttackStateTime = AttackStateTimeDefault;
        private const float AttackStateTimeDefault = 3.5f;

        public void Combo(AnimationClip clip)
        {
            SkillButton.SetActive(false);
            SkillButton1.SetActive(false);
            SkillButton2.SetActive(false);

            comboState = 0;
            //comboTXT.text = string.Empty;
            canAttack = false;

            PlayerInfo.AttackDamage *= 2;
            this.Delay(.5f, () => PlayerInfo.AttackDamage /= 2);

            OverrideController[replacableAttackAnim] = clip;

            animator.runtimeAnimatorController = OverrideController;
            animator.SetTrigger(attack);
        }

        // use instead of generate sphere
        public void AttackFinish()
        {
            if (!PlayerInDanger()) {
                // if player is not in danger we want to normal locomotion system not combat locomotion system
                // locomotion system = idle walk run 
                animator.SetTrigger("Dismount"); 
                return;
            }
            foreach (var enemy in SpawnController.instance.canavars)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < 3 &&
                    Vector3.Angle((enemy.transform.position - transform.position).normalized, transform.forward) < 45)
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                    GameManager.instance.audioSource.PlayOneShot(GameManager.instance.RockSound);

                    // var particlePos = transform.position + ((transform.forward * 1.5f) + transform.up * 1.8f);
                    var particle = PoolManager.instance.ReuseObject(enemyHealth.hitParticle.GetRandom(), enemyHealth.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
                    particle.gameObject.SetActive(true);
                    particle.Play();
                    
                    this.Delay(1.5f, () =>
                    {
                        particle.Pause();
                        particle.gameObject.SetActive(false);
                    });

                    if (enemyHealth.AddDamage(PlayerInfo.AttackDamage)) // enemydead
                    {
                        CloseEnemies.Remove(enemyHealth.transform);
                        if (CloseEnemies.Count == 0)
                        {
                            animator.SetBool(AnimPool.InCombat, false);
                            animator.SetTrigger("Dismount");
                        }
                        break;
                    }
                }
            }

            if (HandState != HandState.Sword && CloseEnemies.Count > 1)
                CameraShake.ShakeOnce(.5f, 5, transform.right * 2 + transform.up * 2);
        }

        public LayerMask damagables;

        // todo convert them to the angle and distance thing
        /// <summary>Anim event ile çalıştır</summary>
        public void Dig()
        {
            EnemyBase.GenerateOverlapSphere(SwordPosition, 2, DigMask, 10, false,
                (x) =>
                {
                    PoolManager.instance.ReuseObject(Inventory.PoolObject.RockPart, x, Quaternion.identity);
                });
        }

        /// <summary>Anim event ile çalıştır</summary>
        public void WoodCut()
        {
            EnemyBase.GenerateOverlapSphere(SwordPosition, 2, WoodMask, 10, false,
                (x) =>
                {
                    PoolManager.instance.ReuseObject(Inventory.PoolObject.WoodPart, x, Quaternion.identity);
                });
        }

        /// <summary> this means enemies in fight range </summary>
        public readonly HashSet<Transform> CloseEnemies = new HashSet<Transform>();

        /// <summary>the enemy we are fighting this will change soon cause for now its detecting from distance</summary>
        public static Transform CurrentEnemy
        {
            get => _currentEnemy;
            set
            {
                _currentEnemy = value;
                // HeadLookTemp.parent = _currentEnemy;
                // HeadLookTemp.localPosition = Vector3.zero;
                // HeadLookTemp.localPosition += Vector3.up * 2;
                // HeadLook.SetLookPosition(null); // for now its null because head turn realy quick
            }
        }

        private static Transform _currentEnemy;
        private static Transform HeadLookTemp;

        [ContextMenu("Test")]
        public void Test()
        {
            SceneManager.LoadScene(0);
        }
    }
}