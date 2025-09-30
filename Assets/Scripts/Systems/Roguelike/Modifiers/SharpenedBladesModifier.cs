public class SharpenedBladesModifier : IRunModifier, IRunModifierOnSpawn
{
    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks) { }

    public void ApplyOnSpawn(UnitStats stats, UnitController controller, int stacks)
    {
        if (stacks <= 0) return;

        float mult = 1f + 0.10f * stacks;
        stats.damage *= mult;
    }
}