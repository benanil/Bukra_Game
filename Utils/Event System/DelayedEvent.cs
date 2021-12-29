
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class DelayedEvent : MonoBehaviour
    {
        public float delay;
        public UnityEvent action;

        public void Invoke(){
            this.Delay(delay, () =>action.Invoke());
        }
    }
}
