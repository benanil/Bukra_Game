using System;
using System.Linq;
using UnityEngine;

namespace AnilTools.Move
{
    public static partial class NormalMove
    {
        public static MoveTask Move(this Transform from, Vector3 to, float speed = MoveTask.DefaultSpeed, AnimationCurve animationCurve = null,bool IsPlus = false, bool isLocal = false, MoveType moveType = MoveType.towards,
                                     Action endAction = null)
        {
            var task = new MoveTask(from, to , speed ,IsPlus,isLocal,animationCurve,moveType,endAction);
            
            AnilUpdate.Register(task);
            return task;
        }

        public static MoveTask MoveArray(this Transform from, Transform[] to , float speed = MoveTask.DefaultSpeed, Action endAction = null,AnimationCurve animationCurve = null,
                                        MoveType moveType = MoveType.towards , bool reverse = false)
        {
            var transforms = to.ToList();
            if (reverse){
                transforms.Reverse();
            }
            
            var task = new MoveTask(from, transforms[0].position , speed ,false,false,animationCurve,moveType, endAction);
            
            for (int i = 1; i < to.Length; i++)
            {
                task.Join(transforms[i].position);
            }
            
            AnilUpdate.Register(task);
            return task;
        }

    }
}