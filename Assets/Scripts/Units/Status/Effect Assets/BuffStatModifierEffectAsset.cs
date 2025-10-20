using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Buff Stat Modifier", fileName = "BuffStatModifierEffect")]
public class BuffStatModifierEffectAsset : StatusEffectAsset
{
    public float damageMul = 1f;
    public float attackSpeedMul = 1f;
    public float moveMul = 1f;
    public float kbForceMul = 1f;
    public float kbResMul = 1f;
    public float armorAdd = 0f;
    public float rangeAdd = 0f;
    public float lifeStealAdd = 0f;

    public override StatusEffect Create()
    {
        var e = new BuffStatModifierEffect
        {
            damageMul = damageMul,
            attackSpeedMul = attackSpeedMul,
            moveMul = moveMul,
            kbForceMul = kbForceMul,
            kbResMul = kbResMul,
            armorAdd = armorAdd,
            rangeAdd = rangeAdd,
            lifeStealAdd = lifeStealAdd
        };
        Prime(e);
        return e;
    }
}
