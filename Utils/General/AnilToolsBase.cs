

using System;
using System.Collections;
using UnityEngine;

namespace AnilTools
{
    public static class AnilToolsBase
    {
        public const int UpdateSpeed = 20;
        public const int UpdateSlow = 35;

        public static IEnumerator DoFor(Action action, short Count, float waitTime = .5f)
        {
            var time = new WaitForSecondsRealtime(waitTime);

            for (short i = 0; i < Count; i++)
            {
                action.Invoke();
                yield return time;
            }
        }
    
        
    }
}
