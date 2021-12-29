
using UnityEngine;

namespace AnilTools.EventSystem
{
    public class SetParentEvent : MonoBehaviour
    {
        public Transform parent;

        public void Set(){
            transform.parent = parent;
        }
    }
}
