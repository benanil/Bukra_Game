

using System;
using UnityEngine;

namespace AnilTools
{
    public static class ReadyVeriables
    {
        public static readonly Func<bool> TrueFunc = () => true;
        public static readonly Action EmptyAction = () => {};

        public static void EmptyArgs(object sender, EventArgs e){}
        
        public static readonly AnimationCurve EaseIn = new AnimationCurve(
            new Keyframe(0, 0)      { weightedMode = WeightedMode.None },
            new Keyframe(.35f, .5f) { weightedMode = WeightedMode.None },
            new Keyframe(1, 1)      { weightedMode = WeightedMode.None });

        public static readonly AnimationCurve EaseOut = new AnimationCurve(
            new Keyframe(0, 0)      { weightedMode = WeightedMode.None },
            new Keyframe(.4f, .55f) { weightedMode = WeightedMode.None },
            new Keyframe(1, 1)      { weightedMode = WeightedMode.None });

        public static readonly AnimationCurve Linear = new AnimationCurve(
            new Keyframe(0, 0) { weightedMode = WeightedMode.None },
            new Keyframe(.5f, .5f) { weightedMode = WeightedMode.None },
            new Keyframe(1, 1) { weightedMode = WeightedMode.None });

        public static LayerMask Ground = 2048; // 2 ^ 11

    }
}
