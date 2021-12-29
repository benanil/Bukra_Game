using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools
{
    public static class AsyncUpdate
    {
        public static void UpdateCoroutine(this MonoBehaviour monobehaviour, float UpdateTime, Action action, Func<bool> EndCondition, Action endAction = null)
        {
            monobehaviour.StartCoroutine(UpdateCoroutne(UpdateTime, action, EndCondition, endAction));
        }

        private static IEnumerator UpdateCoroutne(float UpdateTime, Action action, Func<bool> EndCondition, Action endAction)
        {
            WaitForSecondsRealtime time = new WaitForSecondsRealtime(UpdateTime);
            
            while (true)
            {
                action.Invoke();
                if (EndCondition.Invoke())
                {
                    endAction?.Invoke();
                    break;
                }
                yield return time;
            }
        }

        /// <summary>
        /// Delay frames And Do job
        /// </summary>
        /// usage:
        /// this.Delay(yourAction(),1);
        /// this.Delay(() => {/* your inline action*/},5);
        /// <param name="g">monobehaviour</param>
        /// <param name="frames">delay frames</param>
        /// <param name="f">delay action</param>
        /// <returns>addable delay queue</returns>
        public static CoroutineData<int> Delay(this MonoBehaviour g, int frames, Action f)
        {
            if (frames == 0) {
                f.Invoke();
                return default;
            }
            var coroutineData = new CoroutineData<int>(f, frames);
            g.StartCoroutine(DelayCoroutine(frames, coroutineData));
            return coroutineData;
        }

        public static Coroutine DelayNew(this MonoBehaviour g, float delay, Action f)
        {
            return g.StartCoroutine(DelayJob(delay, f));
        }

        private static IEnumerator DelayJob(float delay, Action action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action();
        }


        /// <summary>
        /// Delay And Do job
        /// </summary>
        /// usage:
        /// this.Delay(yourAction(),1f);
        /// this.Delay(() => {/* your inline action*/},5f);
        /// <param name="g">monobehaviour</param>
        /// <param name="seconds">delay seconds</param>
        /// <param name="f">delay action</param>
        /// <returns>addable delay queue</returns>
        public static CoroutineData<float> Delay(this MonoBehaviour g, float seconds, Action f)
        {
            if (seconds == 0f) {
                f.Invoke();
                return default;
            }
            var coroutineData = new CoroutineData<float>(f, seconds);
            g.StartCoroutine(DelayCoroutine(seconds, coroutineData));
            return coroutineData;
        }

        /// <summary>
        /// Delay And Do job
        /// </summary>
        /// usage:
        /// this.Delay(yourAction(),1f);
        /// this.Delay(() => {/* your inline action*/},5f);
        /// <param name="g">monobehaviour</param>
        /// <param name="seconds">delay seconds</param>
        /// <param name="f">delay action</param>
        /// <returns>addable delay queue</returns>
        public static CoroutineData<float> DelayUnscaled(this MonoBehaviour g, float seconds, Action f)
        {
            var coroutineData = new CoroutineData<float>(f, seconds);
            g.StartCoroutine(DelayCoroutineUnscaled(seconds, coroutineData));
            return coroutineData;
        }
        
        private static IEnumerator DelayCoroutine(int frames, CoroutineData<int> f)
        {
            for (var n = 0; n < frames; ++n)
            {
                yield return null;
            }

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0)
                {
                    for (int i = 0; i < f.currentAction.value1; i++)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    f.Dequeue();
                    f.Invoke();
                }
                else
                {
                    f.Dispose();
                    break;
                }
            }

        }

        private static IEnumerator DelayCoroutine(float seconds, CoroutineData<float> f)
        {
            yield return new WaitForSeconds(seconds);

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0)
                {
                    yield return new WaitForSecondsRealtime(f.currentAction.value1);
                    f.Dequeue();
                    f.Invoke();
                }
                else
                {
                    f.Dispose();
                    break;
                }
            }
        }

        private static IEnumerator DelayCoroutineUnscaled(float seconds, CoroutineData<float> f)
        {
            yield return new WaitForSeconds(seconds);

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0)
                {
                    yield return new WaitForSeconds(f.currentAction.value1);
                    f.Dequeue();
                    f.Invoke();
                }
                else
                {
                    f.Dispose();
                    break;
                }
            }
        }

        public struct CoroutineData<T> : IDisposable // int or float
        {
            internal Queue<Tuple2<Action, T>> queue;
            internal Tuple2<Action, T> currentAction;

            public CoroutineData(Action action, T delay){
                queue = new Queue<Tuple2<Action, T>>();
                currentAction = new Tuple2<Action, T>(action, delay);
            }

            public CoroutineData<T> Add(Action action, T delay){
                queue.Enqueue(new Tuple2<Action, T>(action, delay));
                return this;
            }

            public void Invoke(){
                currentAction.value.Invoke();
            }

            internal void Dequeue(){
                currentAction = queue.Dequeue();
            }

            public void Dispose(){
                queue = null;
                GC.SuppressFinalize(this);
            }
        }


        /// ////////////////////////////////////UNİTY ACTİON//////////////////////////////////////////   ////////////

        public static CoroutineDataUnity<int> Delay(this MonoBehaviour g, int frames, UnityEvent f)
        {
            var coroutineData = new CoroutineDataUnity<int>(f, frames);
            g.StartCoroutine(DelayCoroutineUnity(frames, coroutineData));
            return coroutineData;
        }

        public static CoroutineDataUnity<float> Delay(this MonoBehaviour g, float seconds, UnityEvent f)
        {
            var coroutineData = new CoroutineDataUnity<float>(f, seconds);
            g.StartCoroutine(DelayCoroutineUnity(seconds, coroutineData));
            return coroutineData;
        }

        public static CoroutineDataUnity<float> DelayUnscaled(this MonoBehaviour g, float seconds, UnityEvent f)
        {
            var coroutineData = new CoroutineDataUnity<float>(f, seconds);
            g.StartCoroutine(DelayCoroutineUnscaledUnity(seconds, coroutineData));
            return coroutineData;
        }
        
        private static IEnumerator DelayCoroutineUnity(int frames, CoroutineDataUnity<int> f)
        {
            for (var n = 0; n < frames; ++n) yield return null;

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0){
                    for (int i = 0; i < f.currentAction.value1; i++) yield return new WaitForEndOfFrame();
                    f.Dequeue();
                    f.Invoke();
                }
                else{
                    f.Dispose();
                    break;
                }
            }

        }

        private static IEnumerator DelayCoroutineUnity(float seconds, CoroutineDataUnity<float> f)
        {
            yield return new WaitForSeconds(seconds);

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0)
                {
                    yield return new WaitForSecondsRealtime(f.currentAction.value1);
                    f.Dequeue();
                    f.Invoke();
                }
                else
                {
                    f.Dispose();
                    break;
                }
            }
        }

        private static IEnumerator DelayCoroutineUnscaledUnity(float seconds, CoroutineDataUnity<float> f)
        {
            yield return new WaitForSeconds(seconds);

            f.Invoke();

            while (true)
            {
                if (f.queue.Count != 0)
                {
                    yield return new WaitForSeconds(f.currentAction.value1);
                    f.Dequeue();
                    f.Invoke();
                }
                else
                {
                    f.Dispose();
                    break;
                }
            }
        }


        public struct CoroutineDataUnity<T> : IDisposable // int or float
        {
            internal Queue<Tuple2<UnityEvent, T>> queue;
            internal Tuple2<UnityEvent, T> currentAction;

            public CoroutineDataUnity(UnityEvent action, T delay)
            {
                queue = new Queue<Tuple2<UnityEvent, T>>();
                currentAction = new Tuple2<UnityEvent, T>(action, delay);
            }

            public CoroutineDataUnity<T> Add(UnityEvent action, T delay){
                queue.Enqueue(new Tuple2<UnityEvent, T>(action, delay));
                return this;
            }

            public void Invoke(){
                currentAction.value.Invoke();
            }

            internal void Dequeue(){
                currentAction = queue.Dequeue();
            }

            public void Dispose(){
                queue = null;
                GC.SuppressFinalize(this);
            }
        }

    }

}