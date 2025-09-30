public class BetterPlatingsModifier : IRunModifier, IRunModifierOnSpawn
{
    public void ApplyRecruitment(ref float cost, ref float cooldown, int stacks) { }

    public void ApplyOnSpawn(UnitStats stats, UnitController controller, int stacks)
    {
        if (stacks <= 0) return;

        float mult = 1f + 0.20f * stacks;
        stats.maxHP *= mult;

        if(controller != null)
        {
            controller.CurrentHP = stats.maxHP;
        }
    }
}