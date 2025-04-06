using System.Collections.Generic;
using UnityEngine;

public static class RunData
{
    public static List<GameObject> selectedUnits = new List<GameObject>();

    public static List<ShopItem> purchasedItems = new List<ShopItem>();

    public static void AddShopItem(ShopItem item)
    {
        purchasedItems.Add(item);
    }

    public static void ResetRun()
    {
        selectedUnits.Clear();
        purchasedItems.Clear();
    }
}