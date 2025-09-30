public class GoldMineModifier : IRunModifier, IRunModifierEconomy
{
    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks)
    {

    }

    public void ApplyEconomy(ref float passiveGoldPerSecond, int stacks)
    {
        if (stacks <= 0) return;

        float mult = 1f + 0.50f * stacks;
        passiveGoldPerSecond *= mult;
    }
}