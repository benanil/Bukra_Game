
using Inventory;
using Rock;
using UnityEngine;

public interface IItem
{
    byte id();
    RockPack RockPack();
    Sprite icon();
    void ChangeLanguage(ItemStrings strings);
}

