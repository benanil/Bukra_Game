
using AnilTools;
using Skill;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public partial class CombatControl
    {
        #region static Veriables
        private static readonly int HandStateStr = Animator.StringToHash("HandState");
        private static readonly int BowTransition = Animator.StringToHash("Bow");
        private static readonly int BowFire = Animator.StringToHash("Fire Bow");
        private static readonly int BowDraw = Animator.StringToHash("Draw Bow");
        private static readonly int attack = Animator.StringToHash("Attack");
        private static readonly int Hit = Animator.StringToHash("hit");

        private const byte arrowHitDistance = 250;
        
        private static bool canAttack = true;
        public static float attackSpeed = 0.8f;
        public static bool InCombat = false;
        public static bool Geriyor;
        #endregion

        private float attackTime;
        public float DamageForce = 200;

        // ses
        [Header("Ses", order = 1)]
        [SerializeField] private AudioClip bodySound;
        [SerializeField] private AudioClip die;
        [SerializeField] private AudioClip ArrowThrowSound;
        [SerializeField] private AudioClip BowReloadSound;
        [Header("Animasyon")]

        public AnimationClip replacableAttackAnim;
        public AnimationClip[] DefaultAttackAnimations;

        AnimationClip[] CurrentAttackAnimations;
        AnimatorOverrideController OverrideController;

        public Animator animator;

        private HandState handState;
        public HandState HandState
        {
            get { return handState; }
            set
            {
                handState = value;
                // value changed
                if (value == HandState.BOW){  
                    animator.SetTrigger(BowTransition);
                    AttackButton.Image.sprite = PlayerInfo.instance.HandSprite;
                    AttackButton.clickMode = AnilButton.ClickMode.clickAndRelease;
                    AttackButton.UnityEvent = new UnityEngine.Events.UnityEvent();
                    AttackButton.ReleaseAction = new UnityEngine.Events.UnityEvent();
                    AttackButton.UnityEvent.AddListener(() => OkuGerme());
                    AttackButton.ReleaseAction.AddListener(() => OkuFırlatma());
                }
                else{
                    AttackButton.UnityEvent = new UnityEngine.Events.UnityEvent();
                    AttackButton.ReleaseAction = new UnityEngine.Events.UnityEvent();
                    AttackButton.UnityEvent.AddListener(Atract);
                    AttackButton.clickMode = AnilButton.ClickMode.click;
                    AttackButton.Image.sprite = PlayerInfo.instance.HandSprite;
                    
                    // hand
                    if (value == 0)
                        AttackButton.Image.sprite = PunchSprite;
                }
            }
        }
        
        // combo 
        [SerializeField] Animator ComboTxtAnimator;
        [SerializeField] private SkillButton SkillButton;
        [SerializeField] private SkillButton SkillButton1;
        [SerializeField] private SkillButton SkillButton2;

        [SerializeField]
        private float comboSpeed;
        private float comboDelay;
        private byte comboState;
        public Text comboTXT;

        public bool hasEnemy = CurrentEnemy;

        [Header("Bow")]
        [SerializeField] private ButtonNew AttackButton;
        [SerializeField] private Transform bow;

        [SerializeField] private Inventory.PoolObject Arrow;
        [SerializeField] private LineRenderer BowLine;
        [SerializeField] private Transform ArrowSpawnPos;
        [SerializeField] private Transform rightHand;
        [SerializeField] private byte ArrowThrowSpeed = 20;
        [SerializeField] private byte CameraLerpSpeed = 3;
        [SerializeField] private LayerMask DigMask;
        [SerializeField] private LayerMask WoodMask;

        [SerializeField] private Transform bowUp;
        [SerializeField] private Transform bowDown;
        private Vector3 bowUpStart, bowDownStart;
        [SerializeField] private Animation bowAnimation;

        private Transform CurrentArow;
        public Transform SwordPosition;
        private Image ArrowCircale;

        private Vector3 BowMid;
        private Vector3 ArrowCrosScale;

        private Vector3 LookAnchor => -Mathmatic.camera.transform.right * 1.4f + Mathmatic.camera.transform.forward * 1.2f + SVector3.up * 1;
        private RaycastHit hit;

        private Arrow arrow;

#if UNITY_EDITOR
        private GameObject debugObj;
#endif
        [Header("other"),SerializeField]
        private Sprite PunchSprite;
    }
}
