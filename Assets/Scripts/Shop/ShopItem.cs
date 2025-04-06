[System.Serializable]
public class ShopItem
{
    public string name;
    public string description;
    public int cost;
    public IShopItemEffect effect;
}