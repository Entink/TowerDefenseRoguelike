using UnityEngine;

[CreateAssetMenu(menuName ="StatusEffects/Ranger Aura1", fileName ="RangerAura1Effect")]
public class RangerAura1EffectAsset : StatusEffectAsset
{
    public float damageMul = 1.05f;
    public float attackSpeedMul = 1.05f;
    public float rangeAdd = 0.2f;

    public override StatusEffect Create()
    {
        var e = new RangerAura1Effect
        {
            damageMul = damageMul,
            attackSpeedMul = attackSpeedMul,
            rangeAdd = rangeAdd
        };
        Prime(e);
        return e;
    }
}
