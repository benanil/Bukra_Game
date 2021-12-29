using UnityEngine;

namespace AnilTools.Collision
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class SphereCollisionDetection : MonoBehaviour
    {
        protected Transform target; 
        protected bool isInRange;

        // spheracle collision detection
        protected virtual void Update()
        {
            float distance = Vector3.Distance(transform.position, target.position);
            Interaction(distance < 3);
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

        protected abstract void OnTriggerEnter();

        protected abstract void OnTriggerExit();
    }
}
