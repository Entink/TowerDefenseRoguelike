public class BlackMarketModifier : IRunModifier, IRunModifierOnSpawn
{
    const float minCostMult = 0.10f;

    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {
        if (stacks <= 0) return;
        float cm = 1f - 0.50f * stacks;
        if (cm < minCostMult) cm = minCostMult;
        cost *= cm;
    }

    public void ApplyOnSpawn(UnitStats stats, UnitController controller, int stacks)
    {
        if (stacks <= 0) return;

        float mult = 1f - 0.10f * stacks;
        stats.maxHP *= mult;
        stats.damage *= mult;

        if (controller != null)
        {
            controller.CurrentHP = stats.maxHP;
        }
    }
}