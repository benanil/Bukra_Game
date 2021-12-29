
using Dialog;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
using AnilTools;

public class MerchantSlot : MonoBehaviour, UILanguage
{
    public Text Name;
    public Text priceTXT;
    public Image icon;

    public Button Buy;
    public Button Sell;

    public ItemSC item;

    public void Build(ItemSC item)
    {
        priceTXT.text = item.Price.ToString()+" C";

        icon.sprite = item.icon;

        Name.text = item.Title;

        this.item = item;

        transform.localScale = Vector3.one;

        var dialog = DialogControl.instance;

        var uiItem = transform.GetChild(0).GetComponent<UIItem>();
        uiItem.UpdateItem(item);

        this.Delay(1f,() => uiItem.ItemIcon.color = Color.white);

        Buy.onClick.AddListener(() => dialog.BuyItem(item.Id) );
        Sell.onClick.AddListener(() => dialog.SellItem(item.Id) );
    }

    public void UpdateLanguage(int id)
    {
        Name.text = GameMenu.instance.languages.languages[id].ReturnName(item.Id).Title;
    }

}
