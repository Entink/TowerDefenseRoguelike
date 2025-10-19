using UnityEngine;

public class BurnFlatEffect : StatusEffect
{
    public float damagePerSecond = 5f;
    float carry;

    public override void OnTick(StatusController target, float dt)
    {
        carry += damagePerSecond * stacks * dt;
        if(carry > 0)
        {
            float delta = carry;
            carry = 0f;
            target.OnRequestDamage?.Invoke(delta);
        }
    }
}
