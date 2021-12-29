using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnilTools.Move
{
    public class ScaleTask : ITickable, IDisposable
    {
        internal const byte DefaultSpeed = 3;
        internal const float DefaultRotationSpeed = 5;
        private const float ScaleTolerance = .001f;

        public readonly Transform from;

        private readonly Queue<Vector3> queue;
        private readonly MoveType moveType = MoveType.towards;
        private event Action EndAction;
        public Vector3 TargetScale;

        /// <summary>
        /// <param><b>Move Speed: </b></param>
        /// <para>if movetype is curve this means decided scale time</para>
        /// </summary>
        public float ScaleSpeed = DefaultSpeed;

        public bool ScaleReached{
            get{
                if (Vector3.Distance(from.localScale, TargetScale) < ScaleTolerance) return true;
                return TargetScale == Vector3.zero;
            }
        }

        public AnimationCurve animationCurve;
        private float timer;

        public void Tick()
        {
            if (!ScaleReached)
            {
                if (moveType == MoveType.lerp){
                    from.localScale = Vector3.Lerp(from.localScale, TargetScale, ScaleSpeed * Time.deltaTime);
                }
                else if (moveType == MoveType.towards){
                    from.localScale = Vector3.MoveTowards(from.localScale, TargetScale, ScaleSpeed * Time.deltaTime);
                }
                else if (moveType == MoveType.Curve){
                    from.localScale = Vector3.Lerp(from.localScale, TargetScale, animationCurve.Evaluate(timer / ScaleSpeed));
                    timer += Time.deltaTime;
                }
            }

            if (ScaleReached){
                if (queue.Count == 0){
                    from.localScale = TargetScale;
                    EndAction?.Invoke();
                    Dispose();
                    return;
                }
                TargetScale = queue.Dequeue();
            }
        }

        public unsafe ScaleTask(Transform from, Vector3* TargetScale, float* moveSpeed, bool isPlus,
                        AnimationCurve animationCurve = null, MoveType moveType = 0, Action endAction = null)
        {
            this.from = from;
            this.ScaleSpeed = *moveSpeed;

            this.moveType = moveType;
            this.animationCurve = animationCurve;

            queue = new Queue<Vector3>();

            if (isPlus) this.TargetScale = from.localScale + (*TargetScale);
            else this.TargetScale = *TargetScale;

            EndAction += endAction;
        }

        public  ScaleTask(Transform from, Vector3 TargetScale, float moveSpeed, bool isPlus,
                        AnimationCurve animationCurve = null, MoveType moveType = 0, Action endAction = null)
        {
            this.from = from;
            this.ScaleSpeed = moveSpeed;

            this.moveType = moveType;
            this.animationCurve = animationCurve;

            queue = new Queue<Vector3>();

            if (isPlus) this.TargetScale = from.localScale + TargetScale;
            else this.TargetScale = TargetScale;

            EndAction += endAction;
        }

        public ScaleTask Join(Vector3 scale){
            queue.Enqueue(scale);
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
