using UnityEngine;

public class PoisonPercentEffect : StatusEffect
{
    public float percentPerSecond = 0.02f;
    float carry;
    UnitStats stats;

    public override void OnApply(StatusController target)
    {
        if (stats == null) stats = target.GetComponent<UnitStats>();
    }

    public override void OnTick(StatusController target, float dt)
    {
        if (stats == null) return;
        float dmg = Mathf.Max(0f, stats.maxHP) * percentPerSecond * stacks * dt;
        carry += dmg;
        if(carry > 0)
        {
            float delta = carry;
            carry = 0f;
            target.OnRequestDamageSilent?.Invoke(delta);
        }
    }
}
