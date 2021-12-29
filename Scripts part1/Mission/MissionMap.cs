using UnityEngine;
using UnityEngine.EventSystems;

public class MissionMap : MonoBehaviour,IPointerClickHandler
{
    private const string Open = "Open";
    private const string Close = "Close";
    [SerializeField] private Animator BlackBG;
    private Animator animator;
    private bool isOpen;

    public void OnPointerClick(PointerEventData eventData)
    {
        var anim = isOpen ? Open : Close;
        isOpen = !isOpen;
        animator.SetTrigger(anim);
        BlackBG.SetTrigger(anim);
    }
}
