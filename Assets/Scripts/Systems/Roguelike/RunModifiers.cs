using System.Collections.Generic;

public static class RunModifiers
{

    static List<IRunModifier> BuildEffects(List<RunModifierState> states)
    {
        var effects = new List<IRunModifier>();
        if (states == null) return effects;

        foreach (var s in states)
        {
            switch (s.id)
            {
                case RunModifierId.RecruitmentDrive: effects.Add(new RecruitmentDriveModifier()); break;
                case RunModifierId.RushOrders:  effects.Add(new RushOrdersModifier()); break;
                case RunModifierId.BulkDiscount: effects.Add(new BulkDiscountModifier()); break;
            }
        }
        return effects;
    }

    public static void ApplyRecruitment(List<RunModifierState> states, ref float cost, ref float cooldown)
    {
        if (states == null || states.Count == 0) return;

        var effects = BuildEffects(states);

        for (int i = 0; i < states.Count; i++)
        {
            var st = states[i];
            var ef = effects[i];
            ef.ApplyRecruitment(ref cost, ref cooldown, st.stacks);
        }
    }

    public static (float cost, float cooldown) GetAdjustedRecruitment(float baseCost, float baseCooldown, List<RunModifierState> states)
    {
        float cost = baseCost;
        float cd = baseCooldown;
        ApplyRecruitment(states, ref cost, ref cd);
        return (cost, cd);
    }
}