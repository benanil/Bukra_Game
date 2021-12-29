using UnityEngine;

namespace AnilTools.EventSystem
{
    public class Rotate : MonoBehaviour
    {
        public Vector3 axis;
        public float angle;

        public void Update(){
            transform.Rotate(axis, angle, Space.Self);
        }
    }
}