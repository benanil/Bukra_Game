using AnilTools;
using AnilTools.Unsafe;
using AnilTools.Update;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UrFairy;

namespace Horse
{
    using static Bukra.Facade;
    using static Etkilesim;
    using Debug = UnityEngine.Debug;

    public enum HorseState
    {
        mounted, wondering, constant, following
    }

    public enum MoveState : byte
    {
        walking = 4, fastWalking = 6, running = 10
    }

    [RequireComponent(typeof(CharacterController))]
    public class HorseAI : MonoBehaviour
    {
        // Todo: wondering ekle

        #region CONSTANTS

        private static readonly int HeadShake = Animator.StringToHash("Head Shake");
        private static readonly int jump = Animator.StringToHash("Horse Jump");
        private static readonly int Nigh = Animator.StringToHash("Nigh");
        private static readonly int Mount = Animator.StringToHash("Mount");
        private static readonly int Dismount = Animator.StringToHash("Dismount");

        // threads
        private const short HeadShakeTime = 10000;

        private const float gravity = -14f;
        private const float RayDistance = 1.2f;

        #endregion CONSTANTS

        #region fields & properties

        [SerializeField]
        private HorseState _horseState = HorseState.constant;

        private HorseState HorseState
        {
            get => _horseState;
            set
            {
                _horseState = value;
                DecideWhatToDo();
            }
        }

        private HorseState startState;

        // moving
        private byte Speed => (byte)MoveState;

        [SerializeField] private float Acerrelation;
        [SerializeField] private float JumpSpeed = 25;

        [SerializeField] private AudioClip[] footSounds;
        [SerializeField] private float footStepSpeed = 1;

        // bools
        public static bool PlayerOnHorse;

        private MoveState MoveState;

        private bool CanMove = true, HoldRope = false;

        private bool IsGrounded => Physics.Raycast(Getray(), RayDistance, Ground);

        private Transform cameraPosition;
        private CharacterController rig;

        private Ray Getray() => new Ray(transform.position + Vector3.up, -transform.up);

        private Ray RayDown => new Ray(ForwardPos, Vector3.down);
        Vector3 ForwardPos => transform.localPosition + Vector3.up + transform.forward * 3;
        LayerMask Ground => LayerMask.GetMask("Ground");

        [NonSerialized]
        public Transform Atilla;

        [SerializeField] private Transform PlayerPos;

        [SerializeField] private Transform rightHandPlayer, leftHandPlayer, rightRope, leftRope;

        [SerializeField]
        private Animator animator;

        private Animator PlayerAnimator => CharacterMove.instance.animatior;

        // input
        private Joystick joystickMove;

        [SerializeField] private float cameraFovSpeed;
        [SerializeField] private Transform MountTransform;
        [SerializeField] private Sprite DismountSprite;

        [SerializeField] private float airControlPercent;
        [SerializeField] private float turnSmoothTime = 0.2f;

        public float currentSpeed;

        private float velocityX, velocityY, targetSpeed, turnSmoothVelocity, targetX, targetRotation, headTime;
        private Vector3 movePos;

        private Camera mainCam;

        public static HorseAI currentHorse;

        private float jumpDelay;

        public GameObject windParticle;
        #endregion fields & properties

        // maybe we must need to use this future but for now we dont need to use it in gameplay
#if UNITY_EDITOR
        private bool _record;
        public bool record
        {
            get => _record;
            set
            {
                Application.targetFrameRate = value ? 60 : 0;
                if (value == true)
                {
                    Debug2.Log("recording", Color.cyan);
                    File.Delete(directory + fileName);
                    keycodes = new List<int>();
                }
                else
                {
                    if (keycodes.Count != 0)
                    {
                        Debug2.Log("recording stoped saving", Color.cyan);
                        SaveKeys();
                    }
                }
                _record = value;
            }
        }

        private bool _automatic;

        private bool automatic
        {
            get
            {
                if (_automatic == true)
                {
                    if (automaticIndex >= keyDatas.Count)
                    {
                        Debug2.Log("Automatic Finished", Color.cyan);
                        _automatic = false;
                    }
                }
                return _automatic;
            }

            set
            {
                Application.targetFrameRate = value ? 60 : 0;

                if (_automatic == false && value == true)
                {
                    LoadKeys();
                    automaticIndex = 0;
                    Debug2.Log("Started Moving", Color.cyan);
                }
                _automatic = value;
            }
        }

        private int automaticIndex;

        private readonly KeyCode[] currentKeys = new KeyCode[4];

        public unsafe struct KeyData
        {
            public fixed short keycodes[5];

            public short this[byte index]
            {
                get => keycodes[index];
                set => keycodes[index] = value;
            }
        }

        private const string directory = "C:/TempHorse";
        private const string fileName = "/HorseMoveSave.txt";
        private List<int> keycodes;
        private List<KeyData> keyDatas;

        private void SaveKeys()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            using StreamWriter stream = File.CreateText(directory + fileName);

            keycodes.RemoveAt(keycodes.Count - 1); // removes last character if not we can give error
            for (int i = 0; i < keycodes.Count; i++)
            {
                if (keycodes[i] == 0)
                    stream.Write('\n');
                else
                {
                    stream.Write(keycodes[i] + " ");
                }
            }
        }

        [ContextMenu(nameof(LoadKeys))]
        private void LoadKeys()
        {
            if (!Directory.Exists(directory) || !File.Exists(directory + fileName)) return;

            keyDatas = new List<KeyData>();

            using StreamReader file = File.OpenText(directory + fileName);

            string line;
            int lineIndex = 0;

            while ((line = file.ReadLine()) != null)
            {
                KeyData keydata = new KeyData();
                string[] values = line.Split(' ');

                if (short.TryParse(values[values.Length - 2], out short resultRun))
                {
                    keydata[4] = resultRun; // 2 becuse last onse is space
                }
                else continue;

                for (byte i = 0; i < values.Length - 2; i++) // length max 4
                {
                    if (short.TryParse(values[i], out short value))
                    {
                        keydata[i] = value;
                    }
                }

                keyDatas.Add(keydata);
                lineIndex++;
            }
        }

        private void AutomaticMove()
        {
            KeyData keyData = keyDatas[Mathf.Min(automaticIndex, keyDatas.Count)];

            MoveState = (MoveState)keyData[4];

            for (byte i = 0; i < 4; i++)
            {
                currentKeys[i] = (KeyCode)keyData[i];
            }
            automaticIndex++;
        }
#endif
        private void Start()
        {
            FootSources = new ComponentPool<AudioSource>(30, transform, (a) =>
            {
                a.volume = .4f;
                a.maxDistance = 50;
                a.spatialBlend = 1;
            });

            HoldRope = false;
            // animator = GetComponent<Animator>();
            startState = HorseState;
            Atilla = CharacterMove.instance.transform;

            rig = GetComponent<CharacterController>();
            joystickMove = FindObjectOfType<Joystick>();
            mainCam = Camera.main;
            cameraPosition = mainCam.transform;
        }

        private void Update()
        {
            if (CharacterMove.UsingMinimap()) return;

            jumpDelay -= Time.deltaTime;
            if (PlayerOnHorse)
            {
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.P)) automatic = !automatic;
                if (Input.GetKeyDown(KeyCode.R)) record = !record;

                if (automatic) AutomaticMove();
                else
#endif
                    NormalMove();
            }

            if (HorseState == HorseState.wondering) AddGravity();
            if (HorseState == HorseState.following) FollowingUpdate();
            if (HorseState.Equals(HorseState.mounted) && CanMove) Move();
        }

        private void NormalMove()
        {
#if UNITY_EDITOR
            if (record)
            {
                //////////////////////////////////////////////////// delete this line
                if (Input.GetKey(KeyCode.A)) keycodes.Add((short)KeyCode.A);
                if (Input.GetKey(KeyCode.S)) keycodes.Add((short)KeyCode.S);
                if (Input.GetKey(KeyCode.D)) keycodes.Add((short)KeyCode.D);
                if (Input.GetKey(KeyCode.W)) keycodes.Add((short)KeyCode.W);
                keycodes.Add((byte)MoveState); // the data of running or not
                keycodes.Add(0);
            }
#endif

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                leftShiftAmount = Math.Min(leftShiftAmount + 1, 2);
                if (leftShiftAmount == 1)
                {
                    MoveState = MoveState.fastWalking;
                }
                else
                { // 2
                    MoveState = MoveState.running;
                }
#if UNITY_EDITOR
                if (record) keycodes.Add((short)KeyCode.C); // for the adding lineending C = \n
#endif
            }

            shiftDelay = Input.GetKey(KeyCode.LeftShift) ? 1 : shiftDelay -= Time.deltaTime;

            if (shiftDelay < 0)
            {
                leftShiftAmount = 0;
                MoveState = MoveState.walking;
            }
        }

        private void DecideWhatToDo()
        {
            StopAllCoroutines();
            if (HorseState == HorseState.mounted) return;

            if (HorseState == HorseState.constant)
            {
                SetAnims(0, 0);
                // constanta arada sırada kafa sallama ekle
                ConstantUpdate();
            }
            else if (HorseState == HorseState.wondering) StartCoroutine(WonderingUpdate());
        }

        public void ConstantUpdate()
        {
            this.UpdateWhile(() =>
            {
                headTime -= Time.deltaTime;

                if (headTime < 0)
                {
                    headTime = HeadShakeTime;
                    animator.SetTrigger(HeadShake);
                }
            }, delegate { return HorseState == HorseState.constant; });
        }

        private int leftShiftAmount;
        private float shiftDelay;

        private void LateUpdate()
        {
            if (HoldRope)
            {
                rightRope.position = rightHandPlayer.position;
                leftRope.position = leftHandPlayer.position;
            }
        }

        public void Vissle()
        {
            if (_horseState == HorseState.mounted) return;
            HorseState = HorseState.following;
            targetSpeed = (float)MoveState.walking;
            followDist = 4;
            follow = true;
        }

        private bool follow;
        private float followDist = 20;

        private void FollowingUpdate()
        {
            float dist = Vector3.Distance(transform.position, Atilla.position);

            // if player far enough follow player
            if (!follow && dist > followDist)
            {
                targetSpeed = (float)MoveState.walking;
                currentSpeed = 0;
                follow = true;
            }

            if (follow)
            {
                Vector3 fromPlayer = (NpcController2.Player.position - transform.position).normalized;
                targetRotation = Mathf.Atan2(fromPlayer.x, fromPlayer.z) * Mathf.Rad2Deg;

                FootstepUpdate(1f);

                // float x = (transform.eulerAngles.x - targetRotation) / 190;
                const float x = 0;

                targetX = Mathf.Lerp(targetX, -x, Time.deltaTime);

                velocityY = 300 * -TerrainDistance(); // returns distance to terrain if horse is falling gravity will higher

                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Acerrelation * Time.deltaTime);

                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, targetRotation, Time.deltaTime * 15), 0);
                //Mathf.LerpAngle(transform.eulerAngles.z, -x * 6, Time.deltaTime * 35));

                movePos = transform.forward * currentSpeed + Vector3.up * velocityY;

                rig.Move(movePos * Time.deltaTime);

                currentSpeed = new Vector2(rig.velocity.x, rig.velocity.z).magnitude;
                SetAnims(targetX, currentSpeed);

                if (dist < 4)
                {
                    follow = false; // if horse close enough she will stop
                    followDist = 20;
                    HorseState = startState;
                    SetAnims(0, 0);
                }
            }
        }
        private void Move()
        {
            const float XsmoothSpeed = 1f;
            const string MouseX = "Mouse X";
            const string Horizontal = "Horizontal";

            Vector2 input = Getinput();

            windParticle.SetActive(currentSpeed > (byte)MoveState.fastWalking);

            float slowness = ((float)MoveState.running - currentSpeed) / (float)MoveState.running; // slowness amount between 0 - 1 if horse stoped value is 0

            targetX = Mathf.Lerp(targetX, -Input.GetAxis(MouseX) * slowness, Time.deltaTime * XsmoothSpeed);

            targetSpeed = Speed * input.normalized.magnitude;
            targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraPosition.transform.eulerAngles.y;

            SetAnims(targetX * slowness, currentSpeed);

            FootstepUpdate((currentSpeed / (float)MoveState.running) * footStepSpeed);

            velocityY = 300 * -TerrainDistance(); // returns distance to terrain if horse is falling gravity will higher

            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Acerrelation * Time.deltaTime);

            transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime)),
                                                   Mathf.LerpAngle(transform.eulerAngles.z, -Input.GetAxis(Horizontal) * 6, Time.deltaTime * 35));

            movePos = transform.forward * currentSpeed + Vector3.up * velocityY;

            if (Input.GetKeyDown(KeyCode.Space) && jumpDelay < 0) Jump();

            rig.Move(movePos * Time.deltaTime);

            currentSpeed = new Vector2(rig.velocity.x, rig.velocity.z).magnitude;
        }

        public LayerMask terrainMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float TerrainDistance()
        {
            float dist = 1;
            if (Physics.Raycast(new Ray(transform.position, -transform.up), out var hit, 500, terrainMask))
            {
                dist = hit.distance;
            }
            return dist;
        }

        private float GetModifiedSmoothTime(in float smoothTime)
        {
            return rig.isGrounded ? smoothTime : smoothTime / airControlPercent;
        }

        public void Jump()
        {
            jumpDelay = 2;
            if (rig.isGrounded)
            {
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * JumpSpeed);
                velocityY = jumpVelocity;
                animator.SetTrigger(jump);
                PlayerAnimator.SetTrigger(jump);
            }
        }

        private void AddGravity()
        {
            rig.Move(gravity * Time.deltaTime * Vector3.down);
        }

        #region Audio
        private ComponentPool<AudioSource> FootSources;
        [SerializeField] float[] footTimeSpans = new float[] { 0, .2f, .4f, .8f , 1f, 1.2f};

        private int SpanIndex, FootSoundIndex;
        
        private float FootstepTime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FootstepUpdate(in float speed = 1)
        {
            FootstepTime += Time.deltaTime * speed;

            if (FootstepTime >= footTimeSpans[SpanIndex]) 
            {
                // better than mathf.repeat thing
                if ((++FootSoundIndex) == footSounds.Length) FootSoundIndex = 0;
                if ((++SpanIndex     ) == 6 ) SpanIndex = 0;

                if (FootstepTime >= 1.2f) FootstepTime = -0.12f - (speed * .2f);

                var source = FootSources.Get();
                source.transform.position = transform.position; // set sound position to horse's foot position
                source.PlayOneShot(footSounds[0]);
            }
        }
        // Anim Event Deprecated!
        private void HorseFoot() {}
        #endregion


        private void SetAnims(float x, float y)
        {
            animator.SetFloat(AnimPool.InputX, currentSpeed < (byte)MoveState.walking ? x : -x);
            animator.SetFloat(AnimPool.InputY, y);
            if (PlayerOnHorse) {
                CharacterMove.instance.animatior.SetFloat(AnimPool.InputX, x);
                CharacterMove.instance.animatior.SetFloat(AnimPool.InputY, y);
            }
        }

#region coroutines

        private IEnumerator WonderingUpdate()
        {
            yield return WalkForward();
            yield return Rotate();
        }

        private IEnumerator Rotate()
        {
            Quaternion rotation = transform.rotation.rotateY(RandomReal.Rangef(0, 360));

            while (Quaternion.Angle(transform.rotation, rotation) < 5 && HorseState == HorseState.wondering && IsGrounded)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
                yield return new WaitForEndOfFrame();
            }
        }


        private IEnumerator WalkForward()
        {
            float targetTime = Time.time + RandomReal.Rangef(10, 20);
            while (Time.time < targetTime && HorseState == HorseState.wondering && IsGrounded)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator MountCoroutine()
        {
            while (Vector3.Distance(NpcController2.Player.position, MountTransform.position) > 0.2f)
            {
                NpcController2.Player.position = Vector3.Lerp(NpcController2.Player.position, MountTransform.position, Time.deltaTime * 5);
                yield return new WaitForEndOfFrame();
            }
            
            NpcController2.Player.SetPositionAndRotation(MountTransform.position, MountTransform.rotation);

            CharacterMove.instance.animatior.Play(Mount);
            NpcController2.Player.parent = PlayerPos;
        }

#endregion coroutines

#region mount - dismount

        // -- On Voids --
        public unsafe void BeginMount()
        {
            float delay = 0f;
            if (PlayerInfo.WeaponActive) {
                PlayerAnimator.Play("SwordTake");
                delay = 2f;
            }
            this.Delay(delay, () =>
            { 
                fixed (float* ptr = &CharacterMove.instance.CurrentCameraHeight)
                    UnsafeFloat.Move(ptr, .5f);

                CharacterMove.OnHorseBeginMount();
                SetAnims(0, 0);
                PlayerOnHorse = true;
                currentHorse = this;
                CanMove = false;
                PlayerActivity(false);

                gameMenu.MountHorse();
                PlayerInfo.CurrentHorse = this;

                StartCoroutine(MountCoroutine());
            });
        }

        // anim events from playerinfo
        public void MountEnd()
        {
            CanMove = true;
            HoldRope = true;
            SetAnims(0, 0);

            InputListenner.Set(KeyCode.E, DismountBegin);
        }

        public void StopHorse()
        {
            SetAnims(0, 0);
            CanMove = false;
        }

        public void StartHorse()
        {
            CanMove = true;
        }

        public unsafe void DismountBegin()
        {
            if (currentSpeed > 2) return;
            PlayerAnimator.Play(Dismount);
            fixed (float* ptr = &CharacterMove.instance.CurrentCameraHeight)
                UnsafeFloat.Move(ptr, 1.64f);
            HoldRope = false;
            CanMove = false;
            transform.eulerAngles = transform.eulerAngles.Z(0);

            fixed (Vector3* camPosAnchorPtr = &CharacterMove.instance.PositionAnchor)
                UnsafeVec3.Move(camPosAnchorPtr, new Vector3(1.3f, 0.7f, 0), 5);

            Etkilesim.Disable();
        }

        public unsafe void DismountEnd()
        {
            CharacterMove.OnHorseEndMount();
            Debug2.Log("DisMount End");
            HorseState = startState; Atilla.parent = null;
            currentHorse = null; PlayerOnHorse = false;
            this.Delay(1.2f, () => { PlayerActivity(true); });

            fixed (Vector3* camPosAnchorPtr = &CharacterMove.instance.PositionAnchor)
                UnsafeVec3.Move(camPosAnchorPtr, new Vector3(0f, 0f, 0f));

            gameMenu.DismountHorse();
        }

        private void PlayerActivity(bool value)
        {
            CharacterMove.SetPlayer(value);
            Atilla.transform.parent = value ? null : PlayerPos; // gerekirse late update ile takip ettir
            HorseState = value ? startState : HorseState.mounted;
        }

#endregion mount - dismount


        private Vector2 lastDirection;

        private Vector2 Getinput()
        {
            Vector2 direction = new Vector2();

#if UNITY_EDITOR
            if (automatic)
            {
                for (byte i = 0; i < 4; i++)
                {
                    if (currentKeys[i] == KeyCode.A) direction.x = -1;
                    if (currentKeys[i] == KeyCode.S) direction.y = -1;
                    if (currentKeys[i] == KeyCode.D) direction.x = 1;
                    if (currentKeys[i] == KeyCode.W) direction.y = 1;
                }
            }
            else
#endif
            {
                if (Input.GetKey(KeyCode.A)) direction.x = -1;
                if (Input.GetKey(KeyCode.S)) direction.y = -1;
                if (Input.GetKey(KeyCode.D)) direction.x = 1;
                if (Input.GetKey(KeyCode.W)) direction.y = 1;
            }

            direction = Vector2.MoveTowards(lastDirection, direction, Time.deltaTime * 3);

            lastDirection = direction;

            return direction.normalized;
        }

        [ContextMenu("TEST"), System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Test()
        {
            Debug.Log((int)Mathf.Repeat(6, 6));
        }

        // Animation Events Deprecated !
        public void FootStep(int state) {}

        // gizmos
        public void OnDrawGizmos()
        {
            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(transform.position, transform.position + Vector3.Cross(transform.forward, transform.up));
        }
    }
}