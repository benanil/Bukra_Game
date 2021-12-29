using UnityEngine;

namespace Bukra
{
    public enum Direction : byte { x, y, z }
    
    public class Door : MonoBehaviour
    {
        public const float doorSpeed = 10;
        public float detectDistance = 3;
        private static float animDelay = 0;
        
        public Vector2 MinMaxAngle = new Vector2(-90, 90); // open and closee angle of door
        public Direction rotateIndex = Direction.z; // which rotation your door uses(euler angle) probably y
            
        private float startRotation;
        private bool closing;
    
        private Vector3 startUpDir;
        private bool isInRange;
    //
        private void Start()
        {
            startRotation = transform.localEulerAngles[(byte)rotateIndex];
            startUpDir = transform.up;
        }
    
        private void Update()
        {
            float distance = Vector3.Distance(transform.position, NpcController2.Player.position);
            Interaction(distance < detectDistance); // spheracle collision detection
    
            animDelay -= Time.deltaTime;
    
            if (closing)
            {
                Vector3 tempAngle = transform.localEulerAngles;
                tempAngle[(byte)rotateIndex] = Mathf.LerpAngle(transform.localEulerAngles[(byte)rotateIndex], startRotation, Time.deltaTime * doorSpeed);
                transform.localEulerAngles = tempAngle;
                if (Mathf.Abs(startRotation - transform.localEulerAngles[(byte)rotateIndex]) < .2)
                {
                    tempAngle[(byte)rotateIndex] = startRotation;
                    transform.localEulerAngles = tempAngle;
                }
            }
    
            if (isInRange) // kapıyla etkileşime girecek kadar yakın
            {
                closing = false;
                Vector3 targetAngle = transform.localEulerAngles;
    
                float angle = Vector3.Angle(NpcController2.Player.forward, startUpDir);
                bool targetFront = angle < 90;
                float targetRotation = targetFront ? MinMaxAngle.x : MinMaxAngle.y;
    
                float speedMultipler = 1 + ((angle / 90) * 2); // makes faster at start and slower at the end
    
                targetAngle[(byte)rotateIndex]
                    = Mathf.LerpAngle(transform.localEulerAngles[(byte)rotateIndex], targetRotation, Time.deltaTime * doorSpeed * speedMultipler);
    
                transform.localEulerAngles = targetAngle;
            }
        }
    
        private void Interaction(bool value)
        {
            if (value == true & isInRange == false) { // trigger enter
                OnTriggerEnter();
            }
    
            if (value == false & isInRange == true) { // trigger exit
                OnTriggerExit();
            }
            isInRange = value;
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, detectDistance); // if player is in this range door should open and close
        }
    
        private void OnTriggerEnter()
        {
            if (animDelay < 0)
            {
                NpcController2.Player.GetComponent<Animator>().SetTrigger(AnimPool.OpenDoor); // this could be your players door openning anim if you haven't delete this line
                animDelay = .3f;        
            }
        }
    
        private void OnTriggerExit() {
            closing = true;
        }
    }
}