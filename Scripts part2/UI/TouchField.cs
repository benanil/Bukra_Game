
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchField : AnlField, IPointerDownHandler,IDragHandler,IEndDragHandler,IPointerUpHandler
{
    private static TouchField _instance;
    public static TouchField instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TouchField>();
            }
            return _instance;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        position = Vector2.zero;
        StartPos = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPos = Input.mousePosition;
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}

