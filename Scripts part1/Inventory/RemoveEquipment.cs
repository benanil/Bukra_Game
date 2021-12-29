using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveEquipment : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private EquipmentInfo equipmentInfo;

    public void OnPointerClick(PointerEventData eventData)
    {
        equipmentInfo.RemoveItem();
    }
}
