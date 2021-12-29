using UnityEngine;

namespace AnilTools
{
    public static class ShaderPool
    {
        public static readonly Shader DissolveShader = Shader.Find("Anil/Dissolve");
        public static readonly Shader FadeShader = Shader.Find("Anil/Transparent Texture");
        public static readonly Shader NormalShader = Shader.Find("Anil/Sample");
        public static readonly int DissolveId = Shader.PropertyToID("_Level");
        public static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        public static readonly int Color = Shader.PropertyToID("_Color");
    }
}