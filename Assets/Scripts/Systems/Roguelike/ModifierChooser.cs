using UnityEngine;
using System.Collections.Generic;

public static class ModifierChooser
{
    public static List<RunModifierDef> ChooseOptions(RunModifierDatabase db, int count, int seed, int nodeId)
    {
        if (db == null) return new List<RunModifierDef>();
        var rng = new System.Random(unchecked(seed ^ (nodeId * 4561213)));
        return db.PickMany(rng, ModifierContext.RunReward, count, allowDuplicates: false);
    }
}