// A Middle Games product
// kodun amacı yere raycast atarak değen noktalara çimen meshlerini yerleştirmek

using AnilTools;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MiddleGames.Misc
{
    [ExecuteInEditMode]
    public class GrassRendering : MonoBehaviour
    {
        [Header("Properties")]
        public Mesh grassMesh;
        public Material grassMaterial;
        //public Texture2D NoiseTex;
        [SerializeField] private int RayStartHeight = 20;
        [SerializeField] private float grassHeight = .5f;
        [Range(0,1)]
        [SerializeField] private float noiseHeight = .3f;

        [Header("Customization", order = 1)]
        [SerializeField] private Vector2 GrassSize = new Vector2(80, 100);
        [SerializeField] private Vector2 randomZRot = new Vector2(-10, 10);
        [SerializeField] private int seed;
        [Tooltip("detect Ground color and place grass",order = 1)]
        [SerializeField] private Color GroundColor;
        [SerializeField] private LayerMask ground;
        [SerializeField] private float diffrance;
        [SerializeField] private bool wind;
        [SerializeField] private Vector3 windDir = new Vector3(1,.005f,.5f);

        private static Color _grassColor;
        public static Color GrassColor
        {
            get{
                return _grassColor;
            }
            set{
                _grassColor = value;
                materialProperty?.SetColor("_Color", _grassColor);
            }
        }

        [Header("Optimization"),Range(10, 1023)]
        public int grassCount = 1023;
        [Tooltip("Enable Ambient lighting it will decrease Performance")]
        public bool AmbientLight = true;
        public bool useLocalDenisty; // true = grass handellerdeki denistiyi kullanma
        public bool CanUpdate;

        // Todo: Add shadow

        private List<Matrix4x4> matrecies;
        private static MaterialPropertyBlock materialProperty;
        private Vector3 range;

#if UNITY_EDITOR
        private Vector3 lastPos;
        private Vector3 lastScale;
        private Quaternion oldRot;
        public Color GizmosColor;
#endif

        [ContextMenu("reset")]
        private void Reset(){
            transform.localScale = new Vector3(50, 30, 65);
        }

        private void Start(){
            CanUpdate = true;
        }

        public void OnValidate(){
            if (!CanUpdate || GrassHandeller.instance == null)
                return;
            /*
            if (!initializing){
                StartCoroutine(InitializeMatrixes());
            }
            */

            if (!useLocalDenisty)
                grassCount = GrassHandeller.GrassLocalDenisty;

            range = transform.localScale;

            Random.InitState(seed);
            
            matrecies = new List<Matrix4x4>();

            for (int i = 0; i < grassCount; i++)
            {
                Vector3 origin = transform.position;
                origin.y += RayStartHeight;
                origin += transform.right * range.x * Random.Range(-0.5f, 0.5f);
                origin += transform.forward * range.z * Random.Range(-0.5f, 0.5f);

                var perlinHeight = Mathf.PerlinNoise(origin.x, origin.z);
                if (perlinHeight > noiseHeight && matrecies.Count < 1023){
                    if (Physics.Raycast(new Ray(origin,Vector3.down),out RaycastHit hit, 500 , ground , QueryTriggerInteraction.Ignore)){
                        if (hit.transform.CompareTag("NoGrass") != true)
                        matrecies.Add(Matrix4x4.TRS(hit.point + Vector3.up * grassHeight,
                                      Quaternion.FromToRotation(Vector3.forward, hit.normal).rotateY(Random.Range(0,360)),
                                      Vector3.one * GrassSize.Rangef()));
                    }
                }
            }

            materialProperty = new MaterialPropertyBlock();
            _grassColor = GrassHandeller.instance.grassColor;

            materialProperty.SetColor("_Color", _grassColor);
            materialProperty.SetFloat("_Directional", 3.5f);
            materialProperty.SetVector("_wind_dir", windDir);

            if (AmbientLight) grassMaterial.EnableKeyword("AMBIENT");
            else              grassMaterial.DisableKeyword("AMBIENT");

            if (wind) grassMaterial.EnableKeyword("WIND");
            else      grassMaterial.DisableKeyword("WIND");

            CanUpdate = matrecies.Count > 0;
        }

        private void LateUpdate()
        {
            if (!CanUpdate)
                return;

#if UNITY_EDITOR
            if (lastPos != transform.position || oldRot != transform.rotation || transform.localScale != lastScale) OnValidate();

            lastPos = transform.position;
            oldRot = transform.rotation;
            lastScale = transform.localScale;
#endif

            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial , matrecies , materialProperty);
        }

        private void OnDisable()
        {
            // buffer dispose
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying){
                GizmosColor = Color.white;
                CanUpdate = true;
            }
            Gizmos.color = GizmosColor;
            DrawCube(transform.position, transform.rotation, transform.localScale);
        }

        public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix *= cubeTransform;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = oldGizmosMatrix;
        }
#endif

    }
}