using UnityEngine;

public abstract class StatusEffectAsset : ScriptableObject
{
    public bool hideIcon = false;
    public string effectKey = "";
    public float duration = 3f;
    public int stacks = 1;
    public int maxStacks = 1;
    public abstract StatusEffect Create();
    protected void Prime(StatusEffect e)
    {
        e.duration = duration;
        e.stacks = stacks;
        e.maxStacks = maxStacks < 1 ? 1 : maxStacks;
        e.uiHidden = hideIcon;
        e.effectKey = string.IsNullOrEmpty(effectKey) ? e.GetType().Name : effectKey;
    }
}
