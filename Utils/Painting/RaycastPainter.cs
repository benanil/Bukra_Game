using UnityEngine;

namespace AnilTools.Paint
{
    public class RaycastPainter : MonoBehaviour
    {
        public Brush brush;

        private void Update()
        {
            if (InputPogo.MouseHold)
            {
                if (Mathmatic.RaycastFromCamera(out RaycastHit hit, Mathmatic.FloatMaxValue, ReadyVeriables.Ground))
                {
                    switch (brush.paintMode)
                    {
                        case PaintMode.Circale:
                        if (brush.erase) Painter.PaitCircale(hit, brush.radius, brush.color);
                        else             Painter.EraseCircale(hit, brush.radius);
                            break;
                        case PaintMode.Cube:
                            if (brush.erase) Painter.PaitCube(hit, brush.widthHeight, brush.color);
                            else            Painter.EraseCube(hit, brush.widthHeight);
                            break;
                        case PaintMode.Texture:
                            if (brush.erase) Painter.PaintTexture(hit,  brush.Decal);
                            else             Painter.EraseCube(hit, brush.widthHeight);
                            break;
                    }
                }
            }
        }
    }
}