using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnilTools.Move
{
    // Todo : add Ease. complated

    public unsafe class MoveTask : ITickable, IDisposable
    {
        internal const byte DefaultSpeed = 3;
        internal const float DefaultRotationSpeed = 5;
        private const float PositionTolerance = .001f;
        public readonly Transform from;

        private readonly Queue<Vector3> queue;
        private readonly MoveType moveType = MoveType.towards;
        private event Action EndAction;
        public Vector3 TargetPosition;

        /// <summary>
        /// <param><b>Move Speed: </b></param>
        /// <para>movetype lerp yada towards değilse nokraya varacağı süre</para>
        /// </summary>
        public float MoveSpeed = DefaultSpeed;

        public bool PositionReached{
            get{
                
                if (from.Distance(TargetPosition) < PositionTolerance) return true;

                return TargetPosition == Vector3.zero;
            }
        }

        /// <summary> it will add position to transform</summary>
        private readonly bool IsLocal = false;

        public AnimationCurve animationCurve;
        private float timer;

        public void Tick()
        {
            if (!PositionReached){

                if (moveType == MoveType.lerp)
                {
                    if (IsLocal) from.localPosition  = Vector3.Lerp(from.localPosition, TargetPosition, MoveSpeed * Time.deltaTime);
                    else         from.position       = Vector3.Lerp(from.position, TargetPosition, MoveSpeed * Time.deltaTime);
                }
                else if (moveType == MoveType.towards)
                {
                    if (IsLocal) from.localPosition  = Vector3.MoveTowards(from.localPosition, TargetPosition, MoveSpeed * Time.deltaTime);
                    else from.position               = Vector3.MoveTowards(from.position, TargetPosition, MoveSpeed * Time.deltaTime);
                }
                else if (moveType == MoveType.Curve)
                {
                    if (IsLocal) from.localPosition  = Vector3.LerpUnclamped(from.localPosition, TargetPosition, animationCurve.Evaluate(timer / MoveSpeed));
                    else         from.position       = Vector3.LerpUnclamped(from.position, TargetPosition, animationCurve.Evaluate(timer / MoveSpeed));
                    
                    timer += Time.deltaTime;
                }
            }

            if (PositionReached)
            {
                if (queue.Count == 0)
                {
                    from.position = TargetPosition;
                    EndAction?.Invoke();
                    Dispose();
                    return;
                }
                TargetPosition = queue.Dequeue();
            }
        }

        public MoveTask(Transform from, Vector3* targetPosition, float* moveSpeed, bool isPlus, bool isLocal,
                        AnimationCurve animationCurve = null,MoveType moveType = 0, Action endAction = null)
        {
            this.from = from;
            this.MoveSpeed = *moveSpeed;
            this.IsLocal = isLocal;
         
            this.moveType = moveType;
            this.animationCurve = animationCurve;

            queue = new Queue<Vector3>();

            if (isLocal){
                if (isPlus) this.TargetPosition = from.localPosition + (*targetPosition);
                else        this.TargetPosition = *targetPosition;
            }
            else{
                if (isPlus) this.TargetPosition = from.position + (*targetPosition);
                else        this.TargetPosition = *targetPosition;
            }

            EndAction += endAction;
        }

        public MoveTask(Transform from, Vector3 targetPosition, float moveSpeed, bool isPlus, bool isLocal,
                        AnimationCurve animationCurve = null, MoveType moveType = 0, Action endAction = null)
        {
            this.from = from;
            this.MoveSpeed = moveSpeed;
            this.IsLocal = isLocal;

            this.moveType = moveType;
            this.animationCurve = animationCurve;

            queue = new Queue<Vector3>();

            if (isLocal)
            {
                if (isPlus) this.TargetPosition = from.localPosition + targetPosition;
                else this.TargetPosition = targetPosition;
            }
            else
            {
                if (isPlus) this.TargetPosition = from.position + targetPosition;
                else this.TargetPosition = targetPosition;
            }

            EndAction += endAction;
        }

        public MoveTask Join(Vector3 position){
            queue.Enqueue(position);
            return this;
        }

        public void AddFinishEvent(Action action){
            EndAction += action;
        }

        public int InstanceId(){
            return from.GetInstanceID();
        }

        public void Dispose(){
            AnilUpdate.Remove(this);
            GC.SuppressFinalize(this);
        }
    }
}
