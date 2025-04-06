public class BonusHPItem : IShopItemEffect
{
    private float bonusMultiplayer;

    public BonusHPItem(float multiplayer)
    {
        bonusMultiplayer = multiplayer;
    }

    public void ApplyEffect(UnitStats stats)
    {
        stats.maxHP *= bonusMultiplayer;
    }
}