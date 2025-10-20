using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Shield",fileName ="ShieldEffect")]
public class ShieldEffectAsset : StatusEffectAsset
{
    public float shieldAmount = 30f;
    public override StatusEffect Create()
    {
        var e = new ShieldEffect { shieldAmount = shieldAmount };
        Prime(e);
        return e;
    }
}
