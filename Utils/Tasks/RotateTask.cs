using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnilTools.Move
{
    // Todo : add Ease. complated

    public class RotateTask : ITickable, IDisposable
    {
        internal const byte DefaultSpeed = 3;
        internal const byte DefaultRotationSpeed = 2;
        private const byte RotationTolerance = 1;

        public readonly Transform from;
        private readonly MoveType moveType;
        private readonly Queue<Quaternion> queue;
        private event Action EndAction;
        internal Quaternion targetRotation;

        public float RotateSpeed = DefaultRotationSpeed;

        // editörde gözükmesi için public yapıldı
        public bool RotationReached
        {
            get
            {
                if (Quaternion.Angle(from.rotation, targetRotation) < RotationTolerance)
                {
                    return true;
                }

                return targetRotation == Quaternion.identity;
            }
        }

        private readonly bool IsLocal = false;

        public AnimationCurve animationCurve;

        private float time;

        public void Tick()
        {
            if (!RotationReached){
                if (moveType == MoveType.Curve)
                {
                    if (IsLocal){
                        from.localRotation = Quaternion.Lerp(from.localRotation, targetRotation, time / RotateSpeed);
                    }
                    else{
                        from.rotation = Quaternion.Lerp(from.rotation, targetRotation, time / RotateSpeed );
                    }
                    time += Time.deltaTime;
                }
                else if (moveType == MoveType.lerp){
                    if (IsLocal){
                        from.localRotation = Quaternion.Lerp(from.localRotation, targetRotation, RotateSpeed * Time.deltaTime);
                    }
                    else{
                        from.rotation = Quaternion.Lerp(from.rotation, targetRotation, RotateSpeed * Time.deltaTime);
                    }
                }
                else if (moveType == MoveType.towards){
                    if (IsLocal){
                        from.localRotation = Quaternion.RotateTowards(from.localRotation, targetRotation, RotateSpeed * Time.deltaTime);
                    }
                    else{
                        from.rotation = Quaternion.RotateTowards(from.rotation, targetRotation, RotateSpeed * Time.deltaTime);
                    }
                }
            }

            if (RotationReached){
                if (queue.Count == 0){
                    Debug2.Log("rotated");
                    from.rotation = targetRotation;
                    EndAction?.Invoke();
                    Dispose();
                    return;
                }
                targetRotation = queue.Dequeue();
            }
        }

        public void AddFinishEvent(Action action)
        {
            EndAction += action;
        }

        public RotateTask JoinRotation(Quaternion Rotation)
        {
            queue.Enqueue(Rotation);
            return this;
        }

        public RotateTask(in Transform from, Quaternion targetRot,AnimationCurve animationCurve,MoveType moveType, float rotateSpeed = DefaultRotationSpeed, bool isLocal = true, Action endAction = null)
        {
            this.moveType = moveType;
            this.animationCurve = animationCurve;
            this.from = from;
            this.queue = new Queue<Quaternion>();
            targetRotation = targetRot;
            RotateSpeed = rotateSpeed;
            IsLocal = isLocal;
            EndAction += endAction;
        }

        public unsafe RotateTask(in Transform from, Quaternion* targetRot, AnimationCurve animationCurve, MoveType moveType, float* rotateSpeed, bool isLocal = true, Action endAction = null)
        {
            this.moveType = moveType;
            this.animationCurve = animationCurve;
            this.from = from;
            this.queue = new Queue<Quaternion>();
            targetRotation = *targetRot;
            RotateSpeed = *rotateSpeed;
            IsLocal = isLocal;
            EndAction += endAction;
        }

        public int InstanceId()
        {
            return from.GetInstanceID() + from.GetInstanceID();
        }

        public void Dispose()
        {
            AnilUpdate.Remove(this);
            GC.SuppressFinalize(this);
        }
    }
}
