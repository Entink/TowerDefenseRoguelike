using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Map/Event Database", fileName = "MapEventDatabase")]
public class MapEventDatabase : ScriptableObject
{
    public List<MapEventDef> events = new();

    public MapEventDef PickForColumn(System.Random rng, int columnIndex)
    {
        var pool = new List<MapEventDef>();
        foreach(var e in events)
        {
            if (e == null) continue;

            if (e.hideAfterResolve && !string.IsNullOrEmpty(e.eventId) && EventFlags.IsResolved(e.eventId))
                continue;

            if (columnIndex < e.minColumn || columnIndex > e.maxColumn) continue;

            int weight = Mathf.Max(1, Mathf.RoundToInt(e.commonness * 10f));
            for (int i = 0; i < weight; i++) pool.Add(e);
        }
        if (pool.Count == 0) return null;
        int idx = rng.Next(pool.Count);
        return pool[idx];
    }
}