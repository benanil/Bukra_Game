using System;
using UnityEngine;

namespace AnilTools.Move
{
    public partial class  NormalMove
    {
        public static RotateTask RotateTransform(this Transform from, Quaternion to, float rotationSpeed = 5,
                                 bool isLocal = false,MoveType moveType = 0 ,AnimationCurve animationCurve = null, Action endAction = null)
        {
            var task = new RotateTask(from, to , animationCurve,moveType,rotationSpeed,isLocal,endAction);
            AnilUpdate.Register(task);
            return task;
        }

        public static RotateTask RotateTransforms(this Transform from, Quaternion[] to, float rotationSpeed = 5, bool isLocal = false, 
                                 MoveType moveType = 0, AnimationCurve animationCurve = null, Action endAction = null)
        {
            var task = new RotateTask(from, to[0],animationCurve,moveType,rotationSpeed,isLocal,endAction);

            for (int i = 1; i < to.Length; i++){
                task.JoinRotation(to[i]);
            }

            AnilUpdate.Register(task);
            return task;
        }
    }
}
