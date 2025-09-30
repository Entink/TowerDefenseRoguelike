using UnityEngine;

[CreateAssetMenu(menuName = "Unit Skill Tree/Definition", fileName = "UnitSkiilTree_")]
public class UnitSkillTreeDef : ScriptableObject
{
    public UnitId unitId;
    public SkillNode[] nodes;
}

[System.Serializable]
public class SkillNode
{
    public string nodeId;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    public int costMaterials = 0;
    public string[] prerequisites;
    public int maxLevel = 1;

    [Header("Effects (to UnitTemplate)")]
    public float addPercentHP;
    public float addPercentDMG;
    public float reduceCooldown;
}
