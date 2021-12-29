using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonNew : AnilButton, IPointerExitHandler, IPointerDownHandler,IPointerUpHandler
{
    private bool Down;
    
    private void Update()
    {
        if (Down && Input.GetMouseButtonUp(0) && clickMode ==  ClickMode.clickAndRelease)
        {
            Down = false;
            OnPointerUp(null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down = true;
        StartCoroutine(Delay());
        UnityEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Desellect();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonState = 0; // none
        ReleaseAction.Invoke();
    }

}

