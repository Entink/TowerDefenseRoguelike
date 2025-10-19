public class ShieldEffect : StatusEffect, IAggregatedModifier
{
    public float shieldAmount = 0f;

    public override void OnApply(StatusController target)
    {
        target.AddShield(shieldAmount);
    }

    public void ApplyTo(StatusController target) { }
}