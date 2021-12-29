
using Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Crafting
{
    public class CraftSlot : AnilButton, IPointerEnterHandler , IPointerExitHandler
    {
        public Craft craft;
        private List<CraftSlotData> craftSlotDatas;

        private RectTransform container;

        List<byte> counts;

        public void OnPointerEnter(PointerEventData eventData){
            CraftingController.CurrentSlot = this;
            CraftingController.instance.UpdateYapBtn();
            Sellect();
        }

        public void OnPointerExit(PointerEventData eventData){
            Desellect();
        }

        public void UpdateUI() {
            counts = CharacterInventory.instance.Counts(craft.items).ToList();

            for (int i = 0; i < craftSlotDatas.Count; i++)
            {
                craftSlotDatas[i].CountTxt.text = counts[i].ToString();
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Init")]
        public void Init(){ // todo editorda yap oyun açılışında zorlamasın 

            craftSlotDatas = new List<CraftSlotData>(craft.items.Length);

            container = transform.GetChild(0).GetComponent<RectTransform>();

            for (int i = 0; i < craft.items.Length; i++)
            {
                var slot = Instantiate(CraftDatabase.instance.SlotPrefab, container);
                var symbol = Instantiate(CraftDatabase.instance.SlotPrefab, container);
                var symbolEqualsCondition = i == craft.items.Length - 1;

                slot.name = "Slot " + i;
                symbol.name = symbolEqualsCondition ? "Equals": "Plus";

                slot.GetComponentInChildren<Image>().sprite = craft.items[i].icon;
                slot.GetComponentInChildren<Text>().text = craft.counts[i].ToString();
                slot.localScale = Vector3.one * 0.7f;

                symbol.GetComponentInChildren<Text>().text = string.Empty;
                symbol.GetComponentInChildren<Image>().sprite = symbolEqualsCondition ? CraftDatabase.instance.EqualsPrefab : CraftDatabase.instance.plusPrefab;
                symbol.GetComponentInChildren<Image>().color = Color.black;
                symbol.localScale = Vector3.one * .4f;
            }

            var product = Instantiate(CraftDatabase.instance.SlotPrefab, container); //product
            product.name = "product";
            product.GetComponentInChildren<Image>().sprite = craft.ProductItem.icon;
            product.GetComponentInChildren<Text>().text = craft.PruductItemCount.ToString();

           // UpdateUI();
        }

        [ContextMenu("clear")]
        public void Clear()
        {
            for (int i = 0; i < container.childCount; i++)
            {
                DestroyImmediate(container.GetChild(i).gameObject);
            }
        }
#endif

    }

    [SerializeField]
    public struct CraftSlotData
    {
        public Text CountTxt;
        public Image image;
    }
}