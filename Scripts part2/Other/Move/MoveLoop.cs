
using AnilTools;
using UnityEngine;
using UrFairy;

namespace Assets
{
    public class MoveLoop : MonoBehaviour
    {
        public enum Axis
        {
            x, y, z
        }

        public Axis axis;
        public float distance;
        public float Speed = 1;

        [Range(-1, 1f)]
        public float startSinValue;

        Vector3 rightTarget;
        Vector3 leftTarget;

        private void OnValidate()
        {
            Start();
        }

        private void Start()
        {
            switch (axis)
            {
                case Axis.x:
                    rightTarget = transform.position.XPlus(distance);
                    leftTarget = transform.position.XPlus(-distance);
                    break;
                case Axis.y:
                    rightTarget = transform.position.YPlus(distance);
                    leftTarget = transform.position.YPlus(-distance);
                    break;
                case Axis.z:
                    rightTarget = transform.position.ZPlus(distance);
                    leftTarget = transform.position.ZPlus(-distance);
                    break;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            transform.position = Vector3.Lerp(leftTarget, rightTarget, Mathmatic.Remap(Mathf.Sin(startSinValue + Time.time * Speed)));
        }
    }
}