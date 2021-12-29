using System;
using UnityEngine;

namespace AnilTools.Paint
{
    [Serializable]
    public class Brush
    {
        public PaintMode paintMode;
        public LayerMask layerMask;
        public Color color;
        public bool erase;
        public int widthHeight;
        public int radius;
        public Texture2D Decal;

#if UNITY_EDITOR
        public bool Cube => paintMode == PaintMode.Cube;
        public bool Circale => paintMode == PaintMode.Circale;
        public bool Texture => paintMode == PaintMode.Texture;
#endif

    }

    public enum PaintMode
    {
        Circale, Cube , Texture 
    }

}
