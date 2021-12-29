using UnityEngine.Experimental.Rendering;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AnilTools.Paint
{
    public class AnilMaterial : MaterialBase
    {
        public Renderer renderer;

        protected Texture2D oldTexture;

        [Header("Mehmet için")]
        public bool transparentToTexture;

        private void Start()
        {
            renderer = GetComponent<Renderer>();
            
            oldTexture = renderer.material.mainTexture as Texture2D;

            texture2D = new Texture2D(oldTexture.height, oldTexture.width, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);

            if (transparentToTexture){
                var transparentColors = new Color32[oldTexture.height * oldTexture.width];
                transparentColors.Foreach(x => x = new Color32(0, 0, 0, 0));
                texture2D.SetPixels32(transparentColors);
            }
            else{
                texture2D.SetPixels32(oldTexture.GetPixels32());
            }
            Apply();
        }


        /// <summary>
        /// girilen textureyi materyale boyar
        /// </summary>
        /// <param name="startX">uv nin hangi noktasından başlanacak</param>
        /// <param name="startY">uv nin hangi noktasından başlanacak</param>
        /// <param name="BackgroundColor">arka plan transparent , beyaz , yada siyah ise onu boyamamak için</param>
        public void DrawTexture(Texture2D texture2D, int startX, int startY, Color BackgroundColor = new Color())
        {
            var left = 0;                   var right = texture2D.width;
            var up = texture2D.height;      var down = 0;

            int paintedPixels = 0;

            for (int x = left; x < right; x++){
                for (int y = down; y < up; y++){
                    var colorRequest = texture2D.GetPixel(x, y);
                    if (colorRequest.Difrance(BackgroundColor) > ColorTolerance){
                        paintedPixels++;
                        this.texture2D.SetPixel(startX + x, startY + y, colorRequest);
                    }
                }
            }

            if (paintedPixels < (texture2D.width * texture2D.height) / 5){
                Debug2.Log("olması gerekenden az piksel boyandı background color yanlış olabiilr");
            }

            Apply();
        }

        private void Apply(){ 
            texture2D.Apply();
            renderer.material.SetTexture(Painter._MainTex, texture2D);
        }

        public void EraseCricale(int x, int y, int radius)
        {
            float rSquared = radius * radius;

            for (int u = x - radius; u < x + radius + 1; u++)
                for (int v = y - radius; v < y + radius + 1; v++)
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                        texture2D.SetPixel(u, v, oldTexture.GetPixel(u, v));
            Apply();
        }

        public void EraseCube(int xStart, int yStart, int widthHeight)
        {
            for (int x = -widthHeight / 2; x < widthHeight / 2; x++)
                for (int y = -widthHeight / 2; y < widthHeight / 2; y++)
                    texture2D.SetPixel(xStart + x, yStart + y, oldTexture.GetPixel(xStart + x, yStart + y));
            
            Apply();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnilMaterial))]
    public class MaterialEditor : Editor
    {
        string saveName;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                base.OnInspectorGUI();
                saveName = EditorGUILayout.TextField("Save Name");
                if (GUILayout.Button("save"))
                {
                    var savedTex = ((AnilMaterial)target).texture2D.EncodeToPNG();
                    // adminstator yerine kullanıcı adınız
                    // texture yi istediğiniz yere kaydedin
                    File.WriteAllBytes("C:/Users/Administrator/Desktop/" + saveName + ".png", savedTex);
                }
            }
            else
            {
                var _target = (AnilMaterial)target;
                EditorGUILayout.LabelField("You Are Ready");
                _target.transparentToTexture = EditorGUILayout.ToggleLeft("Mehmet Mod", _target.transparentToTexture);
            }
        }
    }
#endif
}