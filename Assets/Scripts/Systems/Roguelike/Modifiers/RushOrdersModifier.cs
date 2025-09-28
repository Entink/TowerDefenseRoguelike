public class RushOrdersModifier : IRunModifier
{
    const float minCostMult = 0.10f;
    const float minCooldown = 0.01f;

    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {
        if (stacks <= 0) return;
        float cm = 1f + 0.10f * stacks;
        float dm = 1f - 0.20f * stacks;

        if (cm < minCostMult) cm = minCostMult;
        float nextCd = cooldown * dm;
        if (nextCd < minCooldown) dm = minCooldown / (cooldown <= 0f ? 1f : cooldown);

        cost *= cm;
        cooldown *= dm;
    }
}