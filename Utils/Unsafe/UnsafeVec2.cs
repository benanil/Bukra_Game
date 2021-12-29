using AnilTools.Update;
using UnityEngine;

namespace AnilTools.Unsafe
{
    public unsafe static class UnsafeVec2
    {
        public static void Move(Vector2* pointer, Vector2 target, float speed = 3, MoveType moveType = MoveType.towards)
        {
            Vector2 newTarget = new Vector2(pointer->x, pointer->y);

            RegisterUpdate.UpdateWhile(() =>
            {
                GetTarget(ref newTarget, ref target, ref speed, ref moveType);
                pointer->Set(newTarget.x, newTarget.y);
            }, () => pointer->Distance(target) > 0.001f);
        }

        public static void GetTarget(ref Vector2 newTarget, ref Vector2 target, ref float speed, ref MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.towards:
                    newTarget = Vector2.MoveTowards(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.lerp:
                    newTarget = Vector2.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
                case MoveType.Curve:
                    newTarget = Vector2.Lerp(newTarget, target, Time.deltaTime * speed);
                    break;
            }
        }
    }
}