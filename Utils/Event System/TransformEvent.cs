
using AnilTools.Move;
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.EventSystem
{
    public class TransformEvent : MonoBehaviour
    {
        public Transform target;
        public AnimationCurve animationCurve;
        public TransposeFlags transposeFlags;
        public RTS rts;
        public float time = 1;

        public UnityEvent endEvent;

        private bool invoked;

        private void Reset()
        {
            target = this.transform;
            animationCurve = ReadyVeriables.Linear;
        }

        [ContextMenu("test")]
        public void Test()
        {
            Invoke();
        }

        public void Invoke()
        {
            if (invoked) return;
            invoked = true;
            rts.SetTime(time);
            target.TransformationLerp(ref rts, transposeFlags, MoveType.Curve,
                                      animationCurve, () => endEvent.Invoke()).Start();    
        }
    }
}
