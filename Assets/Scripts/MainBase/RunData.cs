using System.Collections.Generic;
using UnityEngine;

public static class RunData
{
    public static List<GameObject> selectedUnits = new List<GameObject>();
    public static List<ShopItemData> purchasedItems = new List<ShopItemData>();


    public static void AddShopItem(ShopItemData item)
    {
        purchasedItems.Add(item);
    }



    public static void ResetRun()
    {
        selectedUnits.Clear();
        purchasedItems.Clear();
        UnitStatsModifiers.Reset();
        
    }
}
