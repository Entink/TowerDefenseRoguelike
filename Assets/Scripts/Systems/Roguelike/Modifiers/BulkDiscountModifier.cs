public class BulkDiscountModifier : IRunModifier
{
    const float minCostMult = 0.10f;

    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {
        if (stacks <= 0) return;
        float cm = 1f - 0.10f * stacks;
        if (cm < minCostMult) cm = minCostMult;
        cost *= cm;
    }
}