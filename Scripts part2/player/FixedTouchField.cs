using AnilTools;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 touchDist;
    public Vector2 TouchDist
    {
        get
        {
            if (CombatControl.Geriyor)
            {
                touchDist.x = InputPogo.MouseX * 4;
                touchDist.y = InputPogo.MouseY * 4;
            }
            return touchDist;
        }
        set
        {
            touchDist = value;
        }
    }

    [HideInInspector] public Vector2 PointerOld;
    [HideInInspector] protected int PointerId;
    [HideInInspector] public bool Pressed;

    private void Update()
    {
        if (Pressed)
        {
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}