
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.Events
{
    public class CollisionEnterEvent : CollisionEventBase
    {
        public UnityEvent Event;

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            Event.Invoke();
        }

    }
}