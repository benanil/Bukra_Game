using AnilTools;
using Dialog;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UrFairy;

namespace Assets.Resources.Scripts.AI.Enemy
{
    using static GameConstants;
    using Random = UnityEngine.Random;
    using PoolObject = Inventory.PoolObject;

    public class EnemyHealth : HealthBase
    {
        [SerializeField] PoolObject Name;
        SlotInfo slotInfo;

        // mission ile ilgili
        [Header("Damage Numbers")]
        [SerializeField] TextMesh DamageNumber;
        [SerializeField] Slider HealthSlider;
        Vector3 RandomPosition = new Vector3();
        Vector3 firstScale = new Vector3();

        bool UpdateNumber;
        const float tolerance = 0.1f;

        // roll
        [SerializeField]
        float dodgeSpeed;

        [Header("Animation")]
        [SerializeField]
        private Animator animator;

        const float ScaleSpeed = 0.4f;  const float animSpeed = 0.4f;

        private bool dead;

        private Vector3 BigScale;

        private readonly PoolObject DeadSkelton = PoolObject.skeletonCDead;

        CanavarControl CanavarControl;

        public PoolObject[] hitParticle;

        private void Start()
        {
            CanavarControl = GetComponent<CanavarControl>();
            HealthSlider.maxValue = Health;
            HealthSlider.gameObject.SetActive(false);
            firstScale = DamageNumber.transform.localScale;
            BigScale = firstScale * 2;

        }
        private void Update()
        {
            if (!DamageNumber) return; 
            UpdateEnemyHealth();
        }

        private void UpdateEnemyHealth()
        {
            if (UpdateNumber && DamageNumber) {
                DamageNumber.transform.position = Vector3.Lerp(DamageNumber.transform.position, RandomPosition, animSpeed);
                DamageNumber.transform.localScale = Vector3.Lerp(DamageNumber.transform.localScale, BigScale, ScaleSpeed);

                if (transform.Distance(RandomPosition) > tolerance)
                    return;
                
                UpdateNumber = false;
                DamageNumber.text = string.Empty;
                DamageNumber.transform.localScale = firstScale;
            }
            else return;
        }

        public void Spawn(SlotInfo ınfo)
        {
            slotInfo = ınfo;
        }

        public override bool AddDamage(short damage, bool silent = false, bool fly = false, Vector3 attackerPosition = new Vector3())
        {
            CanavarControl.AlertWithFrends();

            Debug.Log("Add Damage");

            if (!silent) CameraShake.ShakeOnce(.5f, 5, transform.right * 2 + transform.up * 2);

            Health -= damage;

            HealthSlider.value = Health;
            HealthSlider.gameObject.SetActive(true);

            animator.Play(AnimPool.GetHit);

            // damage number
            if (DamageNumber) {
                GenerateRandomPos(damage);
                StartCoroutine(NumberShowUp());
            }

            // daying
            if (Health <= 0 && !dead) { if (fly) FatalityDie(); else Die(); }
            
            return isDead;
        }

        private void GenerateRandomPos(short damage)
        {
            DamageNumber.transform.position = transform.position + SVector3.up;
            DamageNumber.text = damage.ToString();
            Random.InitState((int)DateTime.Now.Ticks);
            RandomPosition = DamageNumber.transform.position + new Vector3(Random.Range(1, 2),
                                                                           Random.Range(1, 2),
                                                                           Random.Range(1, 2));
            UpdateNumber = true;
        }

        private WaitForSecondsRealtime waitForNumber = new WaitForSecondsRealtime(3);

        private IEnumerator NumberShowUp()
        {
            yield return waitForNumber;
            if (DamageNumber)
                DamageNumber.text = string.Empty;
        }

        public float RequiredFatalitiyDist;

        private void FatalityDie()
        {
            var enemyControl = GetComponent<CanavarControl>();
            SpawnController.instance.canavars.Remove(enemyControl);
            animator.Play("Fatality");

            enemyControl.Die();

            if (slotInfo != null) {
                slotInfo.EnemyCount -= 1;
                slotInfo.SpawnUpdate();
            }

            if (HealthSlider) HealthSlider.gameObject.SetActive(false);

            if (MissionHandeller.OnMission) // eğer görev ise kill ekle
                if (Name == MissionHandeller.instance.ReturnCurrentMission().Enemy)
                    MissionHandeller.instance.Addkill();
            
            transform.eulerAngles = transform.eulerAngles.YPlus(180); // turn player because animation rotation is inverse
            transform.position += transform.forward * RequiredFatalitiyDist; 

            dead = true;
            gameObject.tag = Tags.Untagged;
            // this.DelayNew(5f, () => Destroy(gameObject));
        }

        public override void Die()
        {
            var enemyControl = GetComponent<CanavarControl>();
            SpawnController.instance.canavars.Remove(enemyControl);
            animator.enabled = false;
            
            enemyControl.Die();
            
            var deadBody = PoolManager.instance.ReuseObject(DeadSkelton, transform.position , transform.rotation);
            
            if (slotInfo != null){
                slotInfo.EnemyCount -= 1;
                slotInfo.SpawnUpdate();
            }

            if (HealthSlider) HealthSlider.gameObject.SetActive(false);

            if (MissionHandeller.OnMission) // eğer görev ise kill ekle
                if (Name == MissionHandeller.instance.ReturnCurrentMission().Enemy)
                    MissionHandeller.instance.Addkill();

            dead = true;
            gameObject.tag = Tags.Untagged;

            // var deadMesh = deadBody.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
            this.Delay(.2f,() => Destroy(gameObject));

        }
    }
}