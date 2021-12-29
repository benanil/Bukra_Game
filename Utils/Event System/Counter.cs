
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class Counter : MonoBehaviour
    {
        public int count;
        public int maxCount;

        public UnityEvent endEvent;

        public void Add()
        {
            count++;
            if (count >= maxCount)
            {
                endEvent.Invoke();
                Destroy(this);
            }
        }
    }
}
