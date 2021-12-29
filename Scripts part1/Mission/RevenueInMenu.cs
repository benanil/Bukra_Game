using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dialog
{
    public class RevenueInMenu : MonoBehaviour, IPointerClickHandler
    {
        private ItemSC ItemSC;
        public Image ItemImg;
        public Text ItemText;

        public void Initialize(ItemSC itemSC, string text)
        {
            ItemImg.sprite = itemSC.icon;
            ItemText.text = text;
            ItemSC = itemSC;
        }

        public void Initialize(Sprite sprite, string text)
        {
            ItemImg.sprite = sprite;
            ItemText.text = text;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ItemSC)
            ToolTip.instance.GenerateToolTip(ItemSC, true);
        }
    }
}