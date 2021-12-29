using AnilTools.Update;
using System;
using UnityEngine;

namespace AnilTools.Unsafe
{
    public unsafe static class UnsafeVec3
    {
        public static void Move(Vector3* pointer, Vector3 target,float speed = 3,MoveType moveType = MoveType.towards,Action endaction = null)
        {
            Vector3 newTarget = new Vector3(pointer->x, pointer->y, pointer->z);

            RegisterUpdate.UpdateWhile(() =>
            {
                GetTarget(ref newTarget,ref target,ref speed,ref moveType);
                pointer->Set(newTarget.x, newTarget.y, newTarget.z);
            },() => Vector3.Distance(*pointer,target) > 0.001,endaction);
        }

        public static void GetTarget(ref Vector3 newTarget,ref Vector3 target,ref float speed, ref MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.towards:
                    newTarget = Vector3.MoveTowards(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.lerp:
                    newTarget = Vector3.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.Curve:
                    newTarget = Vector3.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
            }
        }
    }
}
