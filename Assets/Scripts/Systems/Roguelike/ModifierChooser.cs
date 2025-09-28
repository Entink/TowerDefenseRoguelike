using UnityEngine;
using System.Collections.Generic;

public static class ModifierChooser
{
    public static List<RunModifierDef> ChooseOptions(
        RunModifierDatabase db,
        int count,
        int seedBase,
        int fightIndex)
    {
        var options = new List<RunModifierDef>();
        if (db == null || db.All == null || db.All.Count == 0) return options;

        Random.InitState(seedBase + fightIndex * 997);
        var pool = new List<RunModifierDef>(db.All);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            options.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        return options;
    }
}