using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffects/Poison %", fileName ="PoisonPercentEffect")]
public class PoisonPercentEffectAsset : StatusEffectAsset
{
    public float percentPerSecond = 0.02f;
    public override StatusEffect Create()
    {
        var e = new PoisonPercentEffect { percentPerSecond = percentPerSecond };
        Prime(e);
        return e;
    }
}
