using UnityEngine;

[CreateAssetMenu(menuName ="StatusEffects/Burn Flat", fileName = "BurnFlatEffect")]
public class BurnFlatEffectAsset : StatusEffectAsset
{
    public float dps = 5f;
    public override StatusEffect Create()
    {
        var e = new BurnFlatEffect { damagePerSecond = dps };
        Prime(e);
        return e;
    }
}
