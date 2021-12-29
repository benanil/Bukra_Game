
using UnityEngine;

namespace AnilTools.Paint
{
    public class MaterialBase : MonoBehaviour
    {
        protected const float ColorTolerance = 0.1f;
         public Texture2D texture2D;

        public int GetPercentage(Color color)
        {
            int trues = TruePixels(color);

            Debug.Log("percentage: " + (trues / texture2D.height * texture2D.width) * 100);

            return (trues / texture2D.height * texture2D.width) * 100;
        }

        public bool CheckAllPainted(Color color, int PixelTolerance = 0)
        {
            int trues = TruePixels(color);
            Debug.Log("true pixels: " + trues);

            return trues >= texture2D.height * texture2D.width - PixelTolerance;
        }

        public int TruePixels(Color color)
        {
            int trues = 0;

            for (int x = 0; x < texture2D.height; x++)
                for (int y = 0; y < texture2D.width; y++)
                    if (texture2D.GetPixel(x, y).Difrance(color) < ColorTolerance)
                        trues++;
            return trues;
        }
    }
}
