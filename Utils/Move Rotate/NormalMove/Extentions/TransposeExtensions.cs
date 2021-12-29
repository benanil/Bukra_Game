
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnilTools.Move
{
    ///  EXAMPLE:   

    ///  const TransposeFlags transposeFlags = TransposeFlags.position | TransposeFlags.rotation | TransposeFlags.scale | TransposeFlags.isLocal;

    ///  NpcController2.Player.TransformationLerp(
    ///  new RTS
    ///  (
    ///      position: transform.localPosition + transform.forward,
    ///      rotation: Quaternion.identity,
    ///      scale   : Vector3.one,

    ///      scaleSpeed : 5,
    ///      rotateSpeed: 5,
    ///      moveSpeed  : 10
    ///  ), transposeFlags,MoveType.lerp, null, endAction : () => {}).Start();

    [Flags]
    public enum TransposeFlags : byte
    {
        position = 0, rotation = 1, scale = 2, isLocal = 4, isPlus = 8
    }

    public class TransformWorker : IDisposable
    {
        private readonly Action endAction;

        private bool moved, rotated, scaled;

        readonly MoveTask moveTask;
        readonly RotateTask rotateTask;
        readonly ScaleTask scaleTask;

        public TransformWorker(ref MoveTask moveTask, ref RotateTask rotateTask, ref ScaleTask scaleTask, ref Action endAction)
        {
            this.endAction = endAction;

            this.moveTask = moveTask;
            this.rotateTask = rotateTask;
            this.scaleTask = scaleTask;

            moved = moveTask == null;
            rotated = rotateTask == null;
            scaled = scaleTask == null;

            moveTask?.AddFinishEvent(() => {
                moved = true; Check();
            });
            rotateTask?.AddFinishEvent(() => {
                rotated = true; Check();
            });
            scaleTask?.AddFinishEvent(() => {
                scaled = true; Check();
            });
        }

        public void Start(){
            if (moveTask != null) AnilUpdate.Register(moveTask);
            if (rotateTask != null) AnilUpdate.Register(rotateTask);
            if (scaleTask != null) AnilUpdate.Register(scaleTask);
        }

        public void Check(){
            if (moved & rotated & scaled){
                endAction?.Invoke();
                Dispose();
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }

    class WorkerManager
    {
        readonly TransposeFlags transformationFlags;
        readonly Transform from;
        readonly Queue<TransformWorker> transposeWorkers;

        readonly Action endAction;

        public WorkerManager(Transform from, ref TransformWorker firstWorker, TransposeFlags transposeFlags, Action endAction) {
            transposeWorkers = new Queue<TransformWorker>();
            this.transformationFlags = transposeFlags;
            this.from = from;
            this.endAction = endAction;
            firstWorker.Start();
        }

        internal void Add(ref RTS rts)
        {
            transposeWorkers.Enqueue(from.TransformationLerp(ref rts, transformationFlags, MoveType.Curve, ReadyVeriables.Linear, Request));
        }

        internal void Request()
        {
            var worker = transposeWorkers.Dequeue();
            if (worker != null)
            {
                worker.Start();
                return;
            }
            Dispose();
        }

        public void Dispose(){
            endAction.Invoke();
            GC.SuppressFinalize(this);
        }
    }

    public static class TranslateExtensions
    {
        /// <summary>
        /// DONT FORGET .Start()
        /// </summary>
        public unsafe static TransformWorker TransformationLerp(this Transform from, ref RTS rts, in TransposeFlags transposeFlags = TransposeFlags.position,
                                                        MoveType moveType = MoveType.towards, AnimationCurve animationCurve = null, Action endAction = null)
        {
            MoveTask move = null;
            RotateTask rotate = null;
            ScaleTask scale = null;

            if (transposeFlags.HasFlag(TransposeFlags.position))
            {
                fixed (float* speedPointer = &rts.moveSpeed)
                fixed (Vector3* positionPointer = &rts.position)
                move = new MoveTask(from, positionPointer, speedPointer, transposeFlags.HasFlag(TransposeFlags.isPlus), transposeFlags.HasFlag(TransposeFlags.isLocal), animationCurve, moveType);
            }
            if (transposeFlags.HasFlag(TransposeFlags.rotation))
            {
                fixed (float* rotateSpeed = &rts.rotateSpeed)
                fixed (Quaternion* rotationPointer = &rts.rotation)
                rotate = new RotateTask(from, rotationPointer, animationCurve, moveType, rotateSpeed, transposeFlags.HasFlag(TransposeFlags.isLocal));
            }
            if (transposeFlags.HasFlag(TransposeFlags.scale))
            {
                fixed (float* scaleSpeed = &rts.scaleSpeed)
                fixed (Vector3* scalePointer = &rts.scale)
                scale = new ScaleTask(from, scalePointer, scaleSpeed, transposeFlags.HasFlag(TransposeFlags.isPlus), animationCurve, moveType);
            }
            return new TransformWorker(ref move, ref rotate, ref scale, ref endAction);
        }

        public static void TransformationAray(this Transform from, TransformationArrayData transposeDataArray, TransposeFlags transposeFlags = 0, Action endAction = null)
        {
            transposeDataArray.Proceed(from, transposeFlags, endAction);
        }
    }

    [Serializable]
    public class TransformationArrayData : IDisposable
    {
        public List<Transform> transforms;

        public float timePerPoint;

        public Vector3 anchor;

        Queue<Tuple<Transform, float>> _transposions;
        Queue<Tuple<Transform, float>> Transformations
        {
            get {
                if (_transposions == null){
                    _transposions = new Queue<Tuple<Transform, float>>();
                    for (int i = 0; i < transforms.Count - 1; i++){
                        _transposions.Enqueue(new Tuple<Transform, float>(transforms[i], transforms[i].Distance(transforms[i + 1])));
                    }
                }
                return _transposions;
            }
        }

        WorkerManager workerManager;

        public void Proceed(Transform from, TransposeFlags transposeFlags = 0, Action endAction = null)
        {
            var data = Transformations.Dequeue();
            float average = 0, percent = 0;

            Transformations.ToList().ForEach(x => average += x.Item2);

            average /= transforms.Count - 1;
            percent = timePerPoint * (data.Item2 / average);

            var firstRts = new RTS(data.Item1.position + anchor, data.Item1.rotation, data.Item1.localScale, percent);

            var firstWorker = data.Item1.TransformationLerp(ref firstRts, transposeFlags, MoveType.Curve, ReadyVeriables.Linear, RequestFirstOne);

            workerManager = new WorkerManager(from, ref firstWorker, transposeFlags, endAction);

            for (int i = 0; i < Transformations.Count; i++)
            {
                data = Transformations.Dequeue();
                percent = timePerPoint * (data.Item2 / average);
                var rts = new RTS(data.Item1.position + anchor, data.Item1.rotation, data.Item1.localScale, percent);
                workerManager.Add(ref rts);
            }
        }

        private void RequestFirstOne() {
            workerManager.Request();
            Dispose();
        }

        public void Dispose(){
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// try use it in inspector
        /// </summary>
        public TransformationArrayData(List<Transform> transforms, float timePerPoint){
            this.transforms = transforms;
            this.timePerPoint = timePerPoint;
        }
    }

    [Serializable]
    public struct RTS
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        [Min(0.01f)]
        public float moveSpeed, rotateSpeed, scaleSpeed;

        public void SetTime(float time) {
            moveSpeed = time; rotateSpeed = time; scaleSpeed = time;
        }

        public RTS(Transform from, float time){
            position = from.position;
            rotation = from.rotation;
            scale = from.localScale;
            this.moveSpeed = time;
            this.rotateSpeed = time;
            this.scaleSpeed = time;
        }

        public RTS(Transform from, float moveSpeed = 3, float rotateSpeed = 3, float scaleSpeed = 3){
            position = from.position;
            rotation = from.rotation;
            scale = from.localScale;
            this.moveSpeed = moveSpeed;
            this.rotateSpeed = rotateSpeed;
            this.scaleSpeed = scaleSpeed;
        }

        public RTS(Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), Vector3 scale = new Vector3(), float moveSpeed = 3, float rotateSpeed = 3, float scaleSpeed = 3){
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.moveSpeed = moveSpeed;
            this.rotateSpeed = rotateSpeed;
            this.scaleSpeed = scaleSpeed;
        }
    }
}
