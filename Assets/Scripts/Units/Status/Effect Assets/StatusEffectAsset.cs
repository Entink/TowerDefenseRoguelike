using UnityEngine;

public abstract class StatusEffectAsset : ScriptableObject
{
    public float duration = 3f;
    public int stacks = 1;
    public int maxStacks = 1;
    public abstract StatusEffect Create();
    protected void Prime(StatusEffect e)
    {
        e.duration = duration;
        e.stacks = stacks;
        e.maxStacks = maxStacks < 1 ? 1 : maxStacks;
    }
}
