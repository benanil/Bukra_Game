
using UnityEngine;
using AnilTools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MiddleGames.Misc
{
    // mesafeye göre grass rendererlerini aktif ve deaktif etmek için kullanılır
    // gizmos üzerinde görünen yeşil chunklar lod içinde anlamında kırmızı gizmos tam tersi
    // çimen yoğunluğunu tek bir yerden kontrol edebilirsiniz yada grass renderer üzerinden tek tek
    public class GrassHandeller : Singleton<GrassHandeller>
    {
        // Todo add shadow

        public GrassRendering[] rendereres;
        [Tooltip("How far grasses will be render")]
        public float farDist;
        [Range(10,2023),Tooltip("global çimen yoğunluğu")]
        public int grassLocalDenisty = 360;
        public bool ShowGrassChunks;

        public Color grassColor = new Color(0,.8f,.25f);
        public Color grassNightColor = new Color(0, .8f, .25f) / 2;

        public static bool DisableAllGrasses
        {
            get => instance.disableAllGrasses;
            set
            {
                if (value != instance.disableAllGrasses) // value changed
                {
                    if (value == true)  instance.rendereres.Foreach(x => x.CanUpdate = false);
                    if (value == false) instance.rendereres.Foreach(x => x.CanUpdate = true);
                }
                
                instance.disableAllGrasses = value;
            }
        }

        [SerializeField]
        private bool disableAllGrasses = false;

        [SerializeField]
        private bool disableAllShadows = false;

        public static int GrassLocalDenisty = 360;
        private Transform[] RendererTransforms;

        private bool[] result; // artarsa değiştir

        public void ChangeColors(bool isNight)
        {
            GrassRendering.GrassColor = isNight ? grassNightColor : grassColor;
        }

        private void OnValidate()
        {
            GrassLocalDenisty = grassLocalDenisty;
            DisableAllGrasses = disableAllGrasses;

            for (int i = 0; i < rendereres.Length; i++)
            {
                rendereres[i].OnValidate();    
            }
        }

        private void Start()
        {
            farDist = Mathf.Pow(farDist,2);
            RendererTransforms = new Transform[rendereres.Length];

            result = new bool[rendereres.Length];

            for (int i = 0; i < rendereres.Length; i++)
            {
                RendererTransforms[i] = rendereres[i].transform;
            }

            this.UpdateCoroutine(1,() =>
            {
                if (DisableAllGrasses)
                    return;

                for (int i = 0; i < rendereres.Length; i++)
                    result[i] = RendererTransforms[i].DistanceSqr(NpcController2.Player.position) < farDist;

                for (int i = 0; i < rendereres.Length; i++)
                {
#if UNITY_EDITOR
                    rendereres[i].GizmosColor = result[i] == true ? Color.green : Color.red;
#endif       
                    rendereres[i].CanUpdate = result[i];
                }           
                
            },() => false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GrassHandeller))]
    public class GrassHandellerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Refresh"))
            {
                GrassHandeller.instance.rendereres = FindObjectsOfType<GrassRendering>();
            }
        }
    }
#endif

}
