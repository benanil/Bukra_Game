
using Random = UnityEngine.Random;
using System;
using System.Collections;
using UnityEngine;

namespace AnilTools
{
    public static class ShakeUpdate
    {
        public static IEnumerator ShakeCoroutine(this Transform transform, float speed, float radius, float time, Action<Transform> then)
        {

            var targetPos = transform.position + Random.insideUnitSphere * radius;

            while (time > 0)
            {
                time -= Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                
                if (transform.Distance(targetPos) < Mathf.Epsilon)
                {
                    Random.InitState((int)DateTime.Now.Ticks);
                    targetPos = transform.position + RandomReal.V3Randomizer(5) * radius;
                }
                
                yield return null;
            }

            then.Invoke(transform);

        }

    }
}
