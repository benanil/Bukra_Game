
using UnityEngine;

namespace AnilTools.Paint
{
    public static class Painter
    {
        public static readonly int _MainTex = Shader.PropertyToID("_MainTex");

        private static AnilMaterial anilMaterial;
        private static Texture2D texture2D;
        private static Vector2 pCoord;
        private static int xStart;
        private static int yStart;

        public static void ReplaceCircale(RaycastHit hit, int radius, Texture2D afterTexture){
            if (CheckAndLoad(ref hit))
                return;
    
            float rSquared = radius * radius;

            for (int u = xStart - radius; u < xStart + radius + 1; u++)
                for (int v = yStart - radius; v < yStart + radius + 1; v++)
                    if ((xStart - u) * (xStart - u) + (yStart - v) * (yStart - v) < rSquared)
                        texture2D.SetPixel(u, v, afterTexture.GetPixel(u,v));

            Apply();
        }

        public static void PaitCircale(RaycastHit hit, int radius , Color color){
            if (CheckAndLoad(ref hit))
                return;

            float rSquared = radius * radius;

            for (int u = xStart - radius; u < xStart + radius + 1; u++)
                for (int v = yStart - radius; v < yStart + radius + 1; v++)
                    if ((xStart - u) * (xStart - u) + (yStart - v) * (yStart - v) < rSquared)
                        texture2D.SetPixel(u, v, color);

            Apply();
        }

        public static void EraseCircale(RaycastHit hit, int radius){
            if (CheckAndLoad(ref hit))
                return;
            
            anilMaterial.EraseCricale(xStart, yStart, radius);
        }

        public static void PaintTexture(RaycastHit hit, Texture2D texture){
            if (CheckAndLoad(ref hit))
                return;

            for (int w = -texture.width / 2; w < texture.width / 2; w++)
                for (int h = -texture.height / 2; h < texture.height / 2; h++)
                    if (texture.GetPixel(w, h) != Color.clear) // alpha ise boyama
                        texture2D.SetPixel(xStart + w, yStart + h, texture.GetPixel(w,h));

            Apply();
        }

        public static void EraseCube(RaycastHit hit, int widthHeight){
            if (CheckAndLoad(ref hit))
                return;

            anilMaterial.EraseCube(xStart, yStart, widthHeight);
        }

        public static void PaitCube(RaycastHit hit, int widthHeight, Color color){
            if (CheckAndLoad(ref hit))
                return;
            
            for (int x = -widthHeight/2; x < widthHeight / 2; x++)
                for (int y = -widthHeight / 2; y < widthHeight / 2; y++)
                    texture2D.SetPixel(xStart + x, yStart + y, color);
     
            Apply();
        }

        private static void Apply(){ 
            texture2D.Apply();
            anilMaterial.renderer.material.SetTexture(_MainTex, texture2D);
        }

        private static bool CheckAndLoad(ref RaycastHit hit){
            anilMaterial = hit.collider.GetComponent<AnilMaterial>();
            
            if (!anilMaterial){
                Debug2.Log("hit object doesnt have material");
                return false;
            }

            if (!anilMaterial.texture2D.isReadable){
                Debug2.Log("texture is not readable");
                return true;
            }

            pCoord = hit.textureCoord;
            pCoord.x *= anilMaterial.texture2D.width;
            pCoord.y *= anilMaterial.texture2D.height;

            texture2D = anilMaterial.texture2D;
            xStart = Mathf.RoundToInt(pCoord.x * anilMaterial.renderer.material.mainTextureScale.x);
            yStart = Mathf.RoundToInt(pCoord.y * anilMaterial.renderer.material.mainTextureScale.y);

            return true;
        }
    }
}
