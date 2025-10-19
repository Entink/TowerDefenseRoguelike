using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    public int stacks = 1;
    public int maxStacks = 1;

    protected float elapsed;
    public bool IsExpired => elapsed >= duration && duration > 0f;

    public virtual void OnApply(StatusController target) { }
    public virtual void OnExpire(StatusController target) { }
    public virtual void OnTick(StatusController target, float dt) { }

    public virtual bool CanStack(StatusEffect other) => other.GetType() == GetType() && stacks < maxStacks;

    public void AddStack() { stacks = Mathf.Clamp(stacks + 1, 1, maxStacks); }

    public void Tick(StatusController target, float dt)
    {
        elapsed += dt;
        OnTick(target, dt);
    }
}
