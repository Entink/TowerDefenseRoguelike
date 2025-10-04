using UnityEngine;

public enum ModifierRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Flags]
public enum ModifierPool
{
    Default = 1 << 0,
    ShopOnly = 1 << 1,
    RunOnly = 1 << 2,
    Special = 1 << 3
}


[CreateAssetMenu(menuName = "Modifiers/Modifier Def", fileName = "RunModifierDef_")]
public class RunModifierDef : ScriptableObject
{
    [Header("Id")]
    public RunModifierId id;

    [Header("Name")]
    public string displayName;

    [Header("Description")]
    [TextArea(2, 4)] public string shortDescription;

    [Header("Icon")]
    public Sprite icon;

    [Header("Drop Setup")]
    public ModifierPool pools = ModifierPool.Default;
    public ModifierRarity rarity = ModifierRarity.Common;

    [Range(0.1f, 5f)] public float rarityWeightMultiplier = 1f;

    

    [Header("Defaults")]
    public int defaultStacksOnPick = 1;
}