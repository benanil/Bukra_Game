
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class MoveEvent : MonoBehaviour
    {
        public float distance = 100;
        public Vector3 target;

        public bool isForward;
        public float speed = 1f;
        
        public UnityEvent EndEvent;

        private bool canMove;

        public void Start(){
            if (isForward){
                target = transform.position + transform.forward * distance;
            }
        }

        private void Update(){
            if (canMove){
                transform.position += transform.forward * speed * Time.deltaTime;
                if (transform.Distance(target) < .1f){
                    canMove = false;
                    EndEvent.Invoke();
                }
            }
        }

        public void Move(){
            canMove = true;
        }

        public bool Forward(){
            return isForward;
        }
    }
}
