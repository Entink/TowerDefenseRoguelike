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

    [Header("Recruitment (per level)")]
    public float reduceRecruitCostPercent;
    public int addRecruitCostFlat;
    public float reduceRecruitCooldownPercent;
    public float addRecruitCooldownFlat;

    [Header("Combat (per level)")]
    public float addPercentHP;
    public float addFlatHP;
    public float addPercentDMG;
    public float addFlatDMG;

    [Header("Range & AOE (per level)")]
    public float addPercentRange;
    public float addFlatRange;
    public float addPercentAoe;
    public float addFlatAoe;
    public bool enableAOE;

    [Header("Multistrike (per level)")]
    public int addMultiStrike;

    [Header("Survivability (per level)")]
    [Range(0f, 1f)] public float knockbackResist;
    public float regenPerSec;

    [Header("Sustain (per level)")]
    public float lifeSteal;
    
}
