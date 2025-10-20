using UnityEngine;

[CreateAssetMenu(menuName ="StatusEffects/Regen Flat", fileName = "RegenFlatEffect")]
public class RegenFlatEffectAsset : StatusEffectAsset
{
    public float healPerSecond = 3f;
    public override StatusEffect Create()
    {
        var e = new RegenFlatEffect { healPerSecond = healPerSecond };
        Prime(e);
        return e;
    }
}
