using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum ModifierContext
{
    Any,
    Shop,
    RunReward
}

[CreateAssetMenu(menuName = "Modifiers/Modifier Database", fileName = "RunModifierDatabase")]
public class RunModifierDatabase : ScriptableObject
{
    public List<RunModifierDef> mods = new();


    [Header("Rarity Weights")]
    public int commonW = 100;
    public int uncommonW = 50;
    public int rareW = 20;
    public int epicW = 7;
    public int legendaryW = 2;

    public RunModifierDef GetById(RunModifierId id) => mods.FirstOrDefault(m => m && m.id == id);

    public IEnumerable<RunModifierDef> FilterByContext(ModifierContext ctx)
    {
        foreach(var m in mods)
        {
            if (!m) continue;
            var p = m.pools;

            bool allowed = ctx switch
            {
                ModifierContext.Shop => (p & ModifierPool.ShopOnly) != 0 || (p & ModifierPool.Default) != 0,
                ModifierContext.RunReward => (p & ModifierPool.RunOnly) != 0 || (p & ModifierPool.Default) != 0,
                ModifierContext.Any => (p & ModifierPool.Special) == 0,
                _ => true
            };

            if (allowed) yield return m;
        }
    }

    public RunModifierDef PickOne(System.Random rng, ModifierContext ctx, HashSet<RunModifierId> exclude = null)
    {
        var list = FilterByContext(ctx).Where(m => exclude == null || !exclude.Contains(m.id)).ToList();
        if (list.Count == 0) return null;

        int WeightOf(RunModifierDef m)
        {
            int baseW = m.rarity switch
            {
                ModifierRarity.Common => commonW,
                ModifierRarity.Uncommon => uncommonW,
                ModifierRarity.Rare => rareW,
                ModifierRarity.Epic => epicW,
                ModifierRarity.Legendary => legendaryW,
                _ => commonW
            };
            float w = baseW * Mathf.Max(0.1f, m.rarityWeightMultiplier);
            return Mathf.Max(1, Mathf.RoundToInt(w));
        }

        int total = 0;
        var weights = new int[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            weights[i] = WeightOf(list[i]);
            total += weights[i];
        }

        int roll = rng.Next(total);
        int acc = 0;
        for (int i = 0; i < list.Count; i++)
        {
            acc += weights[i];
            if (roll < acc) return list[i];
        }
        return list[list.Count - 1];
    }

    public List<RunModifierDef> PickMany(System.Random rng, ModifierContext ctx, int count, bool allowDuplicates, HashSet<RunModifierId> exclude = null)
    {
        var result = new List<RunModifierDef>();
        var ex = exclude != null
            ? new HashSet<RunModifierId>(exclude)
            : new HashSet<RunModifierId>();

        for (int i = 0; i < count; i++)
        {
            var pick = PickOne(rng, ctx, ex);
            if (pick == null) break;
            result.Add(pick);
            if (!allowDuplicates) ex.Add(pick.id);
        }
        return result;
    }
}