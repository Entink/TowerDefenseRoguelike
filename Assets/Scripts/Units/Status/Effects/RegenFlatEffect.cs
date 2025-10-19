using UnityEngine;

public class RegenFlatEffect : StatusEffect
{
    public float healPerSecond = 3f;
    float carry;

    public override void OnTick(StatusController target, float dt)
    {
        carry += healPerSecond * stacks * dt;
        if(carry > 0)
        {
            float delta = carry;
            carry = 0f;
            target.OnRequestHeal?.Invoke(delta);
        }
    }
}
