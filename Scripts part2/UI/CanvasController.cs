using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CanvasController
{
    public CanvasScripts WarningCanvas;
    public CanvasScripts BaseCanvas;
    public CanvasScripts InventoryCanvas;
    public CanvasScripts MenuCanvas;
    public CanvasScripts TooltipCanvas;

    [System.Serializable]
    public struct CanvasScripts
    {
        public CanvasScaler canvasScaler;
        private Canvas _canvas;
        public Canvas canvas
        {
            get
            {
                if (_canvas == null) _canvas = canvasScaler.GetComponent<Canvas>();
                return _canvas;
            }
        }

        private GraphicRaycaster _graphicRaycaster;
        public GraphicRaycaster graphicRaycaster
        {
            get
            {
                if (_graphicRaycaster == null) _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                return _graphicRaycaster;
            }
        }

        public void SetActive(bool value)
        {
            canvasScaler.enabled = value;
            canvas.enabled = value;
            graphicRaycaster.enabled = value;
            canvas.gameObject.SetActive(value);
        }

        public void SetVisuality(bool value)
        {
            canvasScaler.enabled = value;
            canvas.enabled = value;
            graphicRaycaster.enabled = value;
        }
    }
}

