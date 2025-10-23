using UnityEngine;

public class ShieldBreakEffect : StatusEffect
{
    public float breakAmount = 20f;

    public override void OnApply(StatusController target)
    {
        if (target == null || breakAmount <= 0f) return;
        target.AddShield(-breakAmount);
        target.RebuildAggregates();
        target.EffectsChanged?.Invoke();
    }
}
