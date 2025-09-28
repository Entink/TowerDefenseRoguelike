using UnityEngine;

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

    [Header("Meta")]
    public RunModifierRarity rarity = RunModifierRarity.Common;
    public bool stackable = true;
    public int maxStacks = 99;

    [Header("Defaults")]
    public int defaultStacksOnPick = 1;
}