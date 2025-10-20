public class BuffStatModifierEffect : StatusEffect, IAggregatedModifier
{
    public float damageMul = 1f;
    public float attackSpeedMul = 1f;
    public float moveMul = 1f;
    public float kbForceMul = 1f;
    public float kbResMul = 1f;
    public float armorAdd = 0f;
    public float rangeAdd = 0f;
    public float lifeStealAdd = 0f;

    public override bool CanStack(StatusEffect other) => maxStacks > 1;

    public void ApplyTo(StatusController target)
    {
        if (damageMul != 1f) target.AddDamageMul(damageMul);
        if (attackSpeedMul != 1f) target.AddAttackSpeedMul(attackSpeedMul);
        if (moveMul != 1f) target.AddMoveMul(moveMul);
        if (kbForceMul != 1f) target.AddKBForceMul(kbForceMul);
        if (kbResMul != 1f) target.AddKBResMul(kbResMul);
        if (armorAdd != 0f) target.AddArmor(armorAdd);
        if (rangeAdd != 0f) target.AddRange(rangeAdd);
        if (lifeStealAdd != 0f) target.AddLifeSteal(lifeStealAdd);
    }
}