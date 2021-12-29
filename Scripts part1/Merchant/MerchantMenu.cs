using Dialog;
using Inventory;
using System.Collections.Generic;
using UnityEngine;

public class MerchantMenu : MonoBehaviour, UILanguage
{
    [SerializeField] private NpcType npcState;
    [SerializeField] private GameObject SlotPrefab;
    
    [SerializeField]
    private List<MerchantSlot> merchantSlots = new List<MerchantSlot>();

    private void Start()
    {
        ItemClass firstClass = ItemClass.trade ;
        ItemClass SecondClass = ItemClass.Eating ;

        switch (npcState)
        {
            case NpcType.smith:
                firstClass = ItemClass.Armor;
                SecondClass = ItemClass.HandTool;
                break;
            case NpcType.Healer:
                firstClass = ItemClass.potion;
                SecondClass = ItemClass.rock;
                break;
        }

        for (int i = 0; i < ItemDatabase.instance.itemSCs.Count; i++)
        {
            var currentClass = ItemDatabase.instance.itemSCs[i].currentClass;
            if (currentClass == firstClass || currentClass == SecondClass && ItemDatabase.instance.itemSCs[i].DefaultTrade)
            {
                var slot = Instantiate(SlotPrefab);
                slot.transform.SetParent(transform);
                var slotcomponent = slot.GetComponent<MerchantSlot>();
                slotcomponent.Build(ItemDatabase.instance.itemSCs[i]);
                merchantSlots.Add(slotcomponent);
            }
        }
    }

    public void UpdateLanguage(int id)
    {
        for (int i = 0; i < merchantSlots.Count; i++)
        {
            merchantSlots[i].UpdateLanguage(id);
        }
    }

}
