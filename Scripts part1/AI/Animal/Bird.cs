

using UnityEngine;
using AnilTools;

namespace Animal
{
    [RequireComponent(typeof(Animator))]
    public class Bird : MonoBehaviour
    {
        #region static veriables
        private const byte TerrainSize = 250;
        private static readonly BVector3 RandomSkyPos = new BVector3(TerrainSize,15,TerrainSize);
        private readonly int RightAnim = Animator.StringToHash("Right");
        private readonly int StraightAnim = Animator.StringToHash("Straight");
        #endregion 
        [SerializeField] private short Speed;

        // mem alloc
        private Vector3 DesiredPosition;
        public Vector3 StartPosition;

        private Vector3 offset;

        private Animator animator;

        public float angularSpeed;
        public float circleRad = 1f;
        
        public float currentAngle;

        public Timer timer;

        private bool circularMovement = true;

        private float distance => transform.Distance(DesiredPosition);
        
        private float sin;
        private float cos;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            animator.SetTrigger(RightAnim);
            StartPosition = transform.position;
            timer = new Timer(100,calledObjectId:GetInstanceID());
        }

        private void Update()
        {
            if (timer.TimeHasCome())
            {
                circularMovement = !circularMovement;
                if (circularMovement){
                    StartPosition = transform.position;
                    animator.SetTrigger(RightAnim);
                    DesiredPosition = default; 
                }
                else{
                    animator.SetTrigger(StraightAnim);
                    transform.LookAt(DesiredPosition);
                    DesiredPosition = RandomReal.V3Randomizer(RandomSkyPos);
                }
                timer.Reset();
            }

            if (circularMovement) CircularUpdate(); else RandomPosUpdate();
        }

        private void RandomPosUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, DesiredPosition , Time.deltaTime * Speed);
            if (distance < 1)
            {
                DesiredPosition = StartPosition + RandomReal.V3Randomizer(RandomSkyPos);
            }
        }

        private void CircularUpdate()
        {
            currentAngle += angularSpeed * Time.deltaTime;
            currentAngle = Mathf.Repeat(currentAngle, Mathmatic.PISqr);

            cos = Mathf.Cos(currentAngle);
            sin = Mathf.Sin(currentAngle);

            offset = new Vector3(sin, 0, cos) * circleRad;
            transform.position = StartPosition + offset;
            transform.localEulerAngles = transform.localEulerAngles.OnlyY(currentAngle * Mathf.Rad2Deg - 90);
        }

        public void OnDrawGizmos()
        {
            /*
            if (DesiredPosition != default){
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, DesiredPosition);
            }
            else{
                Gizmos.DrawWireSphere(StartPosition, circleRad);
            }
            */

        }
    }

}