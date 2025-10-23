using UnityEngine;

public class RegenFlatEffect : StatusEffect, IAggregatedModifier
{
    public float healPerSecond = 3f;
    

    public void ApplyTo(StatusController target)
    {
        if (target == null) return;
        target.AddRegenFlat(healPerSecond * Mathf.Max(1, stacks));
    }
}
