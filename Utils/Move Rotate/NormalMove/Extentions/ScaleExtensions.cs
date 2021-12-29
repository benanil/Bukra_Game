
using System;
using UnityEngine;

namespace AnilTools.Move
{
    public static class ScaleExtensions
    {
        public static MoveTask ScaleLerp(this Transform from, Vector3 targetScale, float speed = MoveTask.DefaultSpeed, AnimationCurve animationCurve = null, bool IsPlus = false, bool isLocal = false, MoveType moveType = MoveType.towards,
                                     Action endAction = null)
        {
            var task = new MoveTask(from, targetScale, speed, IsPlus, isLocal, animationCurve, moveType, endAction);
            AnilUpdate.Register(task);
            return task;
        }
    }
}
