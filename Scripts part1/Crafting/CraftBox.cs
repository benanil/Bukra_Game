
using Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Crafting
{
    public class CraftBox : MonoBehaviour , IPointerClickHandler
    {
        #region singleton
        private static CraftBox _instance;
        public static CraftBox instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CraftBox>();
                }
                return _instance;
            }
        }
        #endregion

        [SerializeField] private Image icon;
        private ItemSC item;

        public void Show(ItemSC item)
        {
            this.item = item; 
            icon.sprite = this.item.icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (item)
                ToolTip.instance.GenerateToolTip(item, true);
        }

    }
}