using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea] public string description;
    public int cost;

    public ShopItemEffectType effectType;
    public float effectValue;

    public void ApplyEffect()
    {
        switch(effectType)
        {
            case ShopItemEffectType.BonusHP:
                UnitStatsModifiers.hpMultiplier *= effectValue;
                break;

            case ShopItemEffectType.BonusDamage:
                UnitStatsModifiers.damageMultiplier *= effectValue;
                break;

            case ShopItemEffectType.Discount:
                UnitStatsModifiers.discountMultiplier *= effectValue;
                break;

            case ShopItemEffectType.Cooldown:
                UnitStatsModifiers.cooldownMultiplier *= effectValue;
                break;

            default:
                Debug.LogWarning($"Nieobs�ugiwany efekt przedmiotu: {effectType}");
                break;
        }
    }


}


public enum ShopItemEffectType
{
    BonusHP,
    BonusDamage,
    Discount,
    Cooldown,
    None
}


