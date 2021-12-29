using AnilTools.Update;
using UnityEngine;

namespace AnilTools.Unsafe
{ 
    public unsafe static class UnsafeFloat
    {
        public static void Move(float* pointer, float target, float speed = 3, MoveType moveType = MoveType.towards)
        {
            float newTarget = *pointer;
        
            RegisterUpdate.UpdateWhile(() =>
            {
                GetTarget(ref newTarget, ref target, ref speed, ref moveType);
                pointer->Set(newTarget);
            }, () => pointer->Diffrance(target) > 0.001);
        }

        public static void GetTarget(ref float newTarget, ref float target, ref float speed, ref MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.towards:
                    newTarget = Mathf.MoveTowards(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.lerp:
                    newTarget = Mathf.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.Curve:
                    newTarget = Mathf.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
            }
        }
}
}