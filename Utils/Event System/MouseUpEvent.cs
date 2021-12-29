using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class MouseUpEvent : MonoBehaviour
    {
        public UnityEvent unityEvent;

        private void OnMouseUp()
        {
            unityEvent.Invoke();
        }
    }
}