using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class RotateEvent : MonoBehaviour
    {
        public Vector3 euler;
        
        public float speed;
        
        public UnityAction EndEvent;

        public void Update(){
            transform.eulerAngles += euler * speed;
        }
    }
}