using System.Collections.Generic;

public interface IRunModifier
{
    void ApplyRecruitment(ref float cost, ref float cooldown, int stacks);
}