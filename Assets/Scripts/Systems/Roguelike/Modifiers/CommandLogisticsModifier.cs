public class CommandLogisticsModifier : IRunModifier, IRunModifierLimits
{
    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {

    }

    public void ApplyLimits(ref int maxUnits, int stacks)
    {
        if (stacks <= 0) return;
        maxUnits += stacks;
    }
}