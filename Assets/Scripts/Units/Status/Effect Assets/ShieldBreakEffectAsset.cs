using UnityEngine;

[CreateAssetMenu(menuName ="StatusEffects/Shield Break",fileName ="ShieldBreakEffect")]
public class ShieldBreakEffectAsset : StatusEffectAsset
{
    public float breakAmount = 20f;
    public override StatusEffect Create()
    {
        var e = new ShieldBreakEffect { breakAmount = breakAmount };
        Prime(e);
        return e;
    }
}
