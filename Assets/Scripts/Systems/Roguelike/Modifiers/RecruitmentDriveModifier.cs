public class RecruitmentDriveModifier : IRunModifier
{
    const float minCostMultiplier = 0.10f;
    const float minCooldown = 0.01f;

    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {
        if (stacks <= 0) return;

        float costMult = 1f - 0.15f * stacks;
        if (costMult < minCostMultiplier) costMult = minCostMultiplier;

        float cdMult = 1f + 0.20f * stacks;
        if (cooldown * cdMult < minCooldown) cdMult = minCooldown / (cooldown <= 0f ? 1f : cooldown);

        cost *= costMult;
        cooldown *= cdMult;
    }
}
