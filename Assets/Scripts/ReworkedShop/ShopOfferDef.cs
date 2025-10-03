using UnityEngine;

public enum ShopCurrency
{
    Cash,
    Materials,
    Both
}

[CreateAssetMenu(menuName = "Shop/Offer",fileName = "ShopOffer_")]
public class ShopOfferDef : ScriptableObject
{
    [Header("Display")]
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    [Range(0, 1)] public float weight = 1f;
    public Color tint = Color.white;

    [Header("Cost")]
    public ShopCurrency currency = ShopCurrency.Cash;
    public int priceCash;
    public int priceMaterials;

    [Header("Requirements")]
    public int minColumn = 0;
    public int maxColumn = 999;

    [Header("Effects")]
    public int deltaCash;
    public int deltaMaterials;
    public bool addModifier;
    public RunModifierId modifier;
    public int modifierStacks = 1;

    [Header("One-time per run")]
    public bool uniquePerRun = false;
}