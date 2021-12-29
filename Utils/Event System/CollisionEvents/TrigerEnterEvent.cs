using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.Events
{
    public class TrigerEnterEvent : CollisionEventBase
    {
        public string targetTag;
        public LayerMask Layer;
        public UnityEvent Event;

        private void Reset(){
            var collider = GetComponent<Collider>();
            if (collider) collider.isTrigger = true;
            
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug2.Log("trriger enter");
            if (other.gameObject.layer.Equals(Layer) || other.CompareTag(targetTag)){
                Event.Invoke();
            }
        }
    }
}