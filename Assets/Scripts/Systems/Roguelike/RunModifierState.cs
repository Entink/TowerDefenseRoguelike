using System;

[Serializable]
public class RunModifierState
{
    public RunModifierId id;
    public int stacks;

    public RunModifierState(RunModifierId id, int stacks = 1)
    {
        this.id = id;
        this.stacks = stacks;
    }
}