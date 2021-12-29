
using AnilTools.Move;
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class ScaleEvent : MonoBehaviour
    {
        public Transform target;
        
        public Vector3 targetScale;
        public float speed;
        public UnityEvent endEvent;

        public void Invoke()
        {
            var rts = new RTS(scale: targetScale);
            target.TransformationLerp(ref rts, TransposeFlags.scale, 
                                      MoveType.lerp,null,() => endEvent.Invoke());    
        }
    }
}
