using System.Collections.Generic;

public static class RunModifiers
{
    class NoOpModifier : IRunModifier
    {
        public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks) { }
    }


    static List<(IRunModifier effect, int stacks)> BuildEffects(List<RunModifierState> states)
    {
        var list = new List<(IRunModifier, int)>();
        if (states == null) return list;

        foreach (var s in states)
        {
            IRunModifier eff;
            switch (s.id)
            {
                case RunModifierId.RecruitmentDrive: eff = new RecruitmentDriveModifier(); break;
                case RunModifierId.RushOrders: eff = new RushOrdersModifier(); break;
                case RunModifierId.BulkDiscount: eff = new BulkDiscountModifier(); break;
                case RunModifierId.CommandLogistics: eff = new CommandLogisticsModifier(); break;
                case RunModifierId.GoldMine: eff = new GoldMineModifier(); break;
                case RunModifierId.BetterPlatings: eff = new BetterPlatingsModifier(); break;
                case RunModifierId.SharpenedBlades: eff = new SharpenedBladesModifier(); break;
                default: eff = new NoOpModifier(); break;
                
            }
            list.Add((eff, s.stacks));
        }
        return list;
    }

    public static void ApplyRecruitment(List<RunModifierState> states, ref float cost, ref float cooldown)
    {
        if (states == null || states.Count == 0) return;
        var pairs = BuildEffects(states);

        foreach (var p in pairs)
            p.effect.ApplyRecruitment(ref cost, ref cooldown, p.stacks);
    }

    public static void ApplyLimits(List<RunModifierState> states, ref int maxUnits)
    {
        if (states == null || states.Count == 0) return;
        var pairs = BuildEffects(states);
        foreach (var p in pairs)
            if (p.effect is IRunModifierLimits lim)
                lim.ApplyLimits(ref maxUnits, p.stacks);
    }

    public static void ApplyEconomy(List<RunModifierState> states, ref float passiveGoldPerInterval)
    {
        if (states == null || states.Count == 0) return;
        var pairs = BuildEffects(states);
        foreach (var p in pairs)
            if (p.effect is IRunModifierEconomy eco)
                eco.ApplyEconomy(ref passiveGoldPerInterval, p.stacks);
    }

    public static (float cost, float cooldown) GetAdjustedRecruitment(
        float baseCost, float baseCooldown, List<RunModifierState> states)
    {
        float c = baseCost, cd = baseCooldown;
        ApplyRecruitment(states, ref c, ref cd);
        return (c, cd);
    }

    public static void ApplyOnSpawn(
        List<RunModifierState> states,
        UnitStats stats,
        UnitController controller)
    {
        if (states == null || states.Count == 0 || stats == null) return;

        var pairs = BuildEffects(states);
        foreach(var p in pairs)
        {
            if (p.effect is IRunModifierOnSpawn onSpawn)
                onSpawn.ApplyOnSpawn(stats, controller, p.stacks);
        }
    }
}