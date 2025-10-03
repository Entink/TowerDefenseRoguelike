using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Shop/Database", fileName = "ShopDatabase")]
public class ShopDatabase : ScriptableObject
{
    public List<ShopOfferDef> pool = new();

    public List<ShopOfferDef> Pick(System.Random rng, int columnIndex, int count, HashSet<string> excludeGuids = null)
    {
        var bag = new List<ShopOfferDef>();
        foreach(var o in pool)
        {
            if (!o) continue;
            if (columnIndex < o.minColumn || columnIndex > o.maxColumn) continue;
            if (excludeGuids != null && o.uniquePerRun && excludeGuids.Contains(o.name)) continue;

            int w = Mathf.Max(1, Mathf.RoundToInt(o.weight * 10f));
            for (int i = 0; i < w; i++) bag.Add(o);
        }

        var result = new List<ShopOfferDef>();
        for (int i = 0; i < count && bag.Count > 0; i++)
        {
            int idx = rng.Next(bag.Count);
            var pick = bag[idx];
            result.Add(pick);

            if (pick.uniquePerRun)
                bag.RemoveAll(x => x == pick);
        }
        return result;
    }
}