using AnilTools;
using AnilTools.Move;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CharacterHealth : HealthBase
    {
        private static CharacterHealth _instance;

        public static CharacterHealth instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterHealth>();
                }
                return _instance;
            }
        }

        private static float health = 100;

        public static float Health
        {
            get
            {
                return health;
            }
            private set
            {
                health = value;
            }
        }
        public static bool isDead;
        private WaitForSecondsRealtime kanSüresi = new WaitForSecondsRealtime(0.23f);
        [HideInInspector] public string healthText = "Health:";
        [SerializeField] private Slider HealthSlider;
        [SerializeField] private GameObject KanEfekti;

        [Header("Sesler")]
        [SerializeField] private AudioClip inhale;

        private float regenValue = 10f;

        // Animator
        [SerializeField]
        private Animator animator;

        public Rigidbody _rig;

        public Rigidbody rig
        {
            get
            {
                if (_rig == null)
                {
                    _rig = GetComponent<Rigidbody>();
                }
                return _rig;
            }
        }

        public void ChangeLanguage(string dil)
        {
            healthText = dil;
            HealthSlider.value = Health;
        }

        [ContextMenu("add damage")]
        public void AddDamage()
        {
            AddDamage(1);
        }

        [ContextMenu("Add Health")]
        public void AddHealth()
        {
            AddDamage(50);
        }

        public void AddHealth(byte value = 50)
        {
            health += value;
            HealthSlider.value = Health;
        }

        public override bool AddDamage(short damage = 100, bool silent = false, bool fly = false, Vector3 attackerPosition = new Vector3())
        {
            
            if (isDead) return false;
            if (PlayerInfo.DamageReducing != 0 && damage != 0)
            {
                Health -= damage / PlayerInfo.DamageReducing;
            }
            else
            {
                Health -= damage;
            }

            if (fly)
            {
                CharacterMove.CanMove = false;
                CharacterMove.instance.character.enabled = false;
                var direction = (-(attackerPosition - transform.position).normalized + Vector3.up / 2) * 20; // will be * damage

                transform.Move(transform.position + direction, 1.5f, ReadyVeriables.EaseOut, false, endAction: () =>
                {
                    CharacterMove.CanMove = true;
                    CharacterMove.instance.character.enabled = true;
                });
            }

            // Efektler
            GameManager.instance.audioSource.PlayOneShot(inhale);


            HealthSlider.value = Health;
            if (effectCoroutine != null) StopCoroutine(effectCoroutine);
            effectCoroutine = StartCoroutine(KanEfektiVer());

            if (Health <= 0)
            {
                CharacterMove.SetPlayer(false);
                CharacterMove.CanMove = false;
                CharacterMove.instance.character.enabled = false;

                KanEfekti.SetActive(false);
                animator.SetTrigger(AnimPool.Die);

                GameMenu.instance.PlayerDead();

                isDead = true;

                GetComponents<MonoBehaviour>().Foreach(x =>
                {
                    if (!(x is CharacterMove || x is CharacterHealth))
                        Destroy(x);
                });
            }
            else if (RandomReal.Range(0,2) < 1 && !CharacterMove.ComboCameraMode())
            {
                animator.SetTrigger("GetHit");
            }

            return health <= 0;
        }

        private Coroutine effectCoroutine;

        private IEnumerator KanEfektiVer()
        {
            KanEfekti.SetActive(true);
            yield return kanSüresi;
            KanEfekti.SetActive(false);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CharacterHealth))]
    public class CharacterHealthEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("Health = " + CharacterHealth.Health);
        }
    }

#endif
}