//#define CombatMovementTest
#pragma warning disable IDE0051 //for anim event

using AnilTools;
using System.Runtime.CompilerServices;
using System;
using UnityEngine;

namespace Player
{
    using Random = UnityEngine.Random;
    public partial class CharacterMove : Singleton<CharacterMove>{
        #region Static Veriables
        public float currentCameraDistance = 2;
        public float CurrentCameraHeight = 2;

        public static bool canLook = true, CanMove = true;
        
        private static Vector2 lookYlimit = new Vector2(-40, 45);

        public static Vector2 input;
        #endregion

        public float AttackForwardForce = 20;
        public float CombatAngle;

        // Todo
        internal static void PlaySlideFallAnim()
        {
            throw new NotImplementedException();
        }

        #region props & fields
        [Header("Movement")] private float currentSpeed;
        [SerializeField] private float RunMultipler = 4;
        [SerializeField] private float speed = 7.83f;
        private float gravity = -12;

        internal static void PlaySlideJumpAnim()
        {
            instance.animatior.Play("Slide Jump");
        }

        public float rollSpeed = 5;
        private readonly float speedSmoothTime = 0.1f; 
        private readonly byte stickSpeed = 1;

        private float speedSmoothVelocity;

        [SerializeField] private Color DirtColor;
        [SerializeField] private Color GrassColor;
        [SerializeField] private Color GroundColor;

        [NonSerialized] public CharacterController character;

        public float jumpSpeed = 1;
        private float turnSmoothTime = 0.2f;
        [NonSerialized] public float turnSmoothVelocity = 0.5f;

        [Header("Camera", order = 1)]
        // private FixedTouchField touchField;
        public Vector3 PositionAnchor;
        public Transform cameraTarget;

        // look
        public float lookSpeed;
        
        [SerializeField] private AudioClip[] landingSounds;

        [NonSerialized] public Animator animatior;
        public Joystick joystickMove;
        
        private Vector3 MoveDirection;
        
        private Transform debug;

        [SerializeField]
        private LayerMask ground;

        private Ray _rayToGround;
        private Ray GetRayToGround()
        {
            _rayToGround.origin = transform.position ;
            return _rayToGround;
        }

        private RaycastHit hit;
        private Rigidbody rig;
        
        private Vector3 velocity;
        private float yAnim, velocityY;

        private float distToGround;

        private bool CanJump = true;

        [SerializeField] private ParticleSystem landingParticle;

        private bool _grounded;
        public bool Grounded
        {
            get{
                return _grounded;
            }
            set {
                
                const byte MinFallingDistance = 8;

                if (value == true && MaxFallingDistance > MinFallingDistance) // landing
                {
                    CharacterHealth.instance.AddDamage((short)Mathf.Pow(MaxFallingDistance, falldamageMultipler));
                    // hard landing sound ekle
                    GameManager.instance.audioSource.PlayOneShot(landingSounds[4], 1.4f);
                    MaxFallingDistance = 0;
                    LandParticle();
                }
                if (value == true && _grounded == false)
                {
                    FootSteps(-1);
                    LandParticle();
                }
                _grounded = value;
            }
        }

        private bool _falling, _isRunning;
        private float jumpDelay;

        public bool GetIsRunning()
        {
            if (Grounded) _isRunning = Input.GetKey(KeyCode.LeftShift);
            else _isRunning = false;
            return _isRunning;
        }

        public int IsRunningInt()
        {
            if (Grounded && Input.GetKey(KeyCode.LeftShift)) return 1;
            else return 0;
        }

        [SerializeField] private float falldamageMultipler;

        private float MaxFallingDistance;

        private Camera _mainCam;
        private Camera mainCam
        {
            get
            {
                if (_mainCam == null) _mainCam = Camera.main;
                return _mainCam;
            }
        }

        #endregion

        System.Random randomSys;
        private ComponentPool<AudioSource> footstepSources;
        private ComponentPool<ParticleSystem> footParticles;

        public static float SpeedMultiplier = 1;

        private void SetFalling(bool value)
        {
            if (value == true && _falling == false) // düşme başlangıç
            {
                animatior.SetBool(AnimPool.Falling, true);
            }

            if (value == false && _falling == true)// düşme bitiş
            {
                animatior.SetBool(AnimPool.Falling, false);
            }

            _falling = value;
        }

        private void CheckGroundedOrFalling(){
            if (Physics.Raycast(GetRayToGround(), out hit , byte.MaxValue , ground , QueryTriggerInteraction.Ignore))
            {
                distToGround = Vector3.Distance(hit.point, GetRayToGround().origin);
            }

            if (distToGround > MaxFallingDistance){
                MaxFallingDistance = distToGround;
            }

            const float GroundedDistance = 0.35f;
            const byte MinFallingDistance = 8;

            Grounded = distToGround < GroundedDistance;
            SetFalling(distToGround > MinFallingDistance);
        }

        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
            rig.isKinematic = true;
        }

        private void Start() {

            randomSys = new System.Random();
            groundTex = new Texture2D(4, 4, TextureFormat.RGBA32, false, false);
            footParticles   = new ComponentPool<ParticleSystem>(6, footParticle);
            footstepSources = new ComponentPool<AudioSource>(6, (aSource) =>
            {
                aSource.volume = .22f;
                aSource.maxDistance = 50;
                aSource.spatialBlend = 1; // make sound 3d
            });

            animatior = GetComponent<Animator>();
            GameMenu.SetCursor(false);
            
            InputListenner.Add(KeyCode.V, ChangeCameraMod);

            _rayToGround = new Ray(transform.position + Vector3.up, Vector3.down);
            rolling = false; CanMove = true; canLook = true; CanJump = true;

            var obj = new GameObject("debug");
            debug = obj.transform;
            debug.position = transform.position;
            
            character = GetComponent<CharacterController>();
            rig.isKinematic = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Update(){
            if (camState != CameraState.normal) return;

#if CombatMovementTest
            animatior.SetBool(AnimPool.InCombat, true);
#endif
            currentCameraDistance = Mathf.Clamp(currentCameraDistance - Input.mouseScrollDelta.y, 2, 20); // camera zoom in out

            if (rolling) {
                transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, mainCam.transform.eulerAngles.y, Time.deltaTime * 150), 0);
                rollDuration -= Time.deltaTime;
                if (rollDuration < 0)
                {
                    rolling = false;
                    CanMove = true;
                    rig.isKinematic = true;
                    character.enabled = true;
                    Time.timeScale = 1;
                }
            }
            else {
                Move(); // this is important to be here
            }

            
            if (!CanMove) return;

            // GTA4CameraEffect
            { 
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, GetIsRunning() ? 75 : 65, Time.deltaTime * 4);
                if (!GetIsRunning() && yaw > -5)  // means when looking down wee will add gta4 field of wiew effect in camera control
                    mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 70 , Time.deltaTime * 2);
            }

            CheckGroundedOrFalling();

            // input = new Vector2(joystickMove.Horizontal, joystickMove.Vertical);
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (character.velocity.magnitude < 0.5f) yAnim = Mathf.Lerp(yAnim, 0, Time.deltaTime * 5);
            else
            {

#if !CombatMovementTest
                if (!CombatControl.PlayerInDanger()) 
                {
                    yAnim = Mathf.Lerp(yAnim, IsRunningInt() + 1, Time.deltaTime * 5);
                }
                else
#endif
                {
                    yAnim = Mathf.Lerp(yAnim, input.y * (IsRunningInt() + 1), Time.deltaTime * 5);
                }
            }

            const float motionAnimDampTime = 0.05f;

            animatior.SetFloat(AnimPool.InputY, yAnim);
            animatior.SetFloat(AnimPool.InputX, currentSpeed, motionAnimDampTime, Time.deltaTime);
            MoveBase(input);

            jumpDelay -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && jumpDelay < 0)
            {
                Debug.Log("Jump");
                Jump();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MoveBase(Vector2 inputDir) {
            if (character.enabled == false) return;

            if (inputDir != Vector2.zero)
            {
                float z = Mathf.LerpAngle(transform.eulerAngles.z, -inputDir.x * 3, Time.deltaTime * 25);
#if !CombatMovementTest
                if (!CombatControl.PlayerInDanger())
                {
                    float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
                    float y = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
                    transform.eulerAngles = new Vector3(0, y, z);
                }  else
#endif
                transform.eulerAngles = new Vector3(0, mainCam.transform.eulerAngles.y, z);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, 0, Time.deltaTime * 30));
            }

#if !CombatMovementTest
            if (!CombatControl.PlayerInDanger())
            {
                float targetSpeed = (GetIsRunning() ? speed * RunMultipler : speed * inputDir.magnitude) * SpeedMultiplier;
                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
                velocity = transform.forward * currentSpeed;
            }
            else
#endif
            {
                inputDir.Normalize();
                float targetSpeed = (GetIsRunning() ? speed * RunMultipler : speed) * SpeedMultiplier;
                float targetY = targetSpeed * input.y;
                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetY, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
                velocity = (transform.forward * currentSpeed) + (transform.right * (targetSpeed * input.x * 0.8f)) * 0.8f;
                animatior.SetFloat(AnimPool.InputX, input.x);
            }

            velocityY += Time.deltaTime * gravity;
        }

        private void Move()
        {
            if (!character.enabled) return;
            Vector3 stickVelocity = CanJump ? (-hit.normal) * stickSpeed : Vector3.zero;

            character.Move((velocity + stickVelocity + Vector3.up * velocityY) * Time.deltaTime);
            if (character.isGrounded)  velocityY = 0;
        }

        private bool rolling; // music
        private float rollDuration;

        /// <summary>
        /// öne ve geri takla atar
        /// </summary>
        /// <param name="direction">direction to roll</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Roll(Vector3 direction, in float duration = 1, in bool slowMo = false)
        {
            rollDuration = duration;
            rolling = true;            CanMove = false;
            character.enabled = false; rig.isKinematic = false;
            direction.y = 0;

            if (slowMo) Time.timeScale = 0.4f;

            rig.AddForce(rollSpeed * direction, ForceMode.Impulse);
        }

        public void Jump(){
            if (!CanMove && CanJump) return;

            CanJump = false;

            new Timer(1,() => CanJump = true);

            if (Grounded){
                animatior.SetTrigger(AnimPool.jump);
                velocityY = Mathf.Sqrt(jumpSpeed * -1 * gravity);
            }
            jumpDelay = 1.2f;
        }
        
        private float GetModifiedSmoothTime(float smoothTime){
            const float airControlPercent = .3f;
            return character.isGrounded ? smoothTime : airControlPercent;
        }
        
#region FootStep
        [Header("FootStep")]
        // [SerializeField] private RenderTexture groundRenderer;
        [SerializeField] private Camera groundCamera;
        [SerializeField] private ParticleSystem footParticle;

        Texture2D groundTex; // initialized in start
        /// <summary> renders one pixel ground texture </summary>
        public void GetGroundColor()
        {
            RenderTexture rt = new RenderTexture(4, 4, 24);
            groundCamera.forceIntoRenderTexture = true;
            groundCamera.targetTexture = rt;
            groundCamera.Render();

            RenderTexture.active = rt;

            groundTex.ReadPixels(new Rect(0, 0, 4, 4), 0, 0);
            groundTex.Apply();

            RenderTexture.active = null;

            GroundColor = groundTex.GetPixel(0, 0) * 1.25f;
        }

        [SerializeField] private Transform rightFoot, leftFoot;                    
        [SerializeField] private Material footstepMaterial;
                                                                                           
        // anim event den çek

        /// state is bitfield:
        /// none = 0, left = 1, right = 2, run = 4, walk = 8  
        /// 9 walk, left     10 walk, right
        /// 5 run, left      6  run, right
        private void FootSteps(int state)
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                if (state > 8) return; // running but state is walk return.
            }
            else if (state < 8) return; // walking but state is run return.

            if (Input.GetAxis("Vertical") < 0.1f) return;

            GetGroundColor();
            float dirtDiffrance  = GroundColor.DifranceRGB(DirtColor);
            float grassDiffrance = GroundColor.DifranceRGB(GrassColor);

            ParticleSystem particle = footParticles.Get();
            AudioSource source = footstepSources.Get();
            
            source.transform.position = transform.position;
            particle.transform.position = (state & 2) != 0 ? rightFoot.position : leftFoot.position; // 2 is right 

            if (dirtDiffrance < grassDiffrance) {
                source.PlayOneShot(landingSounds[(int)Math.Round(randomSys.NextDouble())]);
            }
            else  {
                source.PlayOneShot(landingSounds[2 + (int)Math.Round(randomSys.NextDouble())]);
            }
            footstepMaterial.SetColor("_TintColor", GroundColor); 
            particle.Play();
        }
        
        private void LandParticle(){
            if (landingParticle) {
                landingParticle.main.ChangeColor(GroundColor);
                landingParticle.Play();
            }
        }
#endregion FootStep

        [ContextMenu("Test")]
        public void Test()  { }
        // anim event 
        private void WoodCut(){
            CombatControl.instance.WoodCut();
        }

        // anim event
        private void Dig(){
            CombatControl.instance.Dig();
        }

        /// <summary>sets players movebility</summary>
        public static void SetPlayer(bool value)
        {
            instance.character.enabled = value;
            CanMove = value; canLook = value;
            instance.GetComponent<CapsuleCollider>().enabled = value;
            instance.GetComponent<Rigidbody>().isKinematic = value;
        }
        
        public static void SetWalkAnim(float speed = 5) {
            instance.animatior.SetFloatLerp(AnimPool.InputY, 0, speed);
        }

        // ---- menu ----
        public void ChangeLookSpeed(float value) {
            lookSpeed = value;
        }

        // ---- Camera ----

        public void ChangeCameraMod() {
            // pc 
            // CurrentCameraHeight += Input.mouseScrollDelta.y * 1f;
        }

        [Tooltip("spine")]
        public Transform horseCamTarget;

        public static void OnHorseBeginMount() {
            instance.cameraTarget = instance.horseCamTarget;
        }

        public static void OnHorseEndMount() {
            instance.cameraTarget = instance.transform;
        }

        // ---- Dialog ----
        public void OnChangeDialogAnim(){
            if (animatior.GetBool(AnimPool.DialogStr)) {
                animatior.SetTrigger(AnimPool.Idle);
                return;
            }
            animatior.SetBool(AnimPool.Dismount, true);
        }

    }
}
