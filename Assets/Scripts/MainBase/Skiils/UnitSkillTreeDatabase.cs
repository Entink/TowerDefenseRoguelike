using UnityEngine;

[CreateAssetMenu(menuName = "Unit Skill Tree/Database", fileName = "UnitSkillTrees")]
public class UnitSkillTreeDatabase : ScriptableObject
{
    public UnitSkillTreeDef[] trees;

    public UnitSkillTreeDef Get(UnitId id)
    {
        if (trees == null) return null;
        foreach (var t in trees)
            if (t != null && t.unitId == id) return t;
        return null;
    }
}