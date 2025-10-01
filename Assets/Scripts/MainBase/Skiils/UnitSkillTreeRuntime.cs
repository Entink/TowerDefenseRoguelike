using UnityEngine;

public static class UnitSkillTreeRuntime
{
    public static void ApplyToRecruitment(UnitStats stats, ref int cost, ref float cooldown)
    {
        var db = Resources.Load<UnitSkillTreeDatabase>("UnitSKillTrees");
        if (db == null || stats == null) return;
        var tree = db.Get(stats.unitId);
        if (tree == null || tree.nodes == null) return;

        foreach(var node in tree.nodes)
        {
            if (node == null) continue;
            int lvl = UnitSkillProgress.GetLevel(stats.unitId, node.nodeId);
            if (lvl <= 0) continue;

            for(int i = 0; i < lvl; i++)
            {
                if(node.reduceRecruitCostPercent != 0f)
                {
                    float mult = 1f - node.reduceRecruitCostPercent;
                    cost = Mathf.RoundToInt(cost * mult);
                }
                if (node.reduceRecruitCooldownPercent != 0f)
                {
                    float mult = 1f - node.reduceRecruitCooldownPercent;
                    cooldown *= mult;
                }
            }
        }
    }

    public static void ApplyOnSpawn(UnitStats stats, UnitController controller)
    {
        var db = Resources.Load<UnitSkillTreeDatabase>("UnitSkillTrees");
        if (db == null || stats == null) return;
        var tree = db.Get(stats.unitId);
        if (tree == null || tree.nodes == null) return;

        float addHpFlat = 0f;
        float hpMult = 1f;
        float dmgMult = 1f;

        foreach(var node in tree.nodes)
        {
            if (node == null) continue;
            int lvl = UnitSkillProgress.GetLevel(stats.unitId, node.nodeId);
            if (lvl <= 0) continue;

            for (int i = 0; i < lvl; i++)
            {
                if (node.addPercentHP != 0f) hpMult *= (1f + node.addPercentHP);
                if (node.addFlatHP != 0f) addHpFlat += node.addFlatHP;
                if (node.addPercentDMG != 0f) dmgMult *= (1f + node.addPercentDMG);
            }
        }

        if(hpMult != 1f || addHpFlat != 0f)
        {
            if(stats.maxHP > 0)
            {
                stats.maxHP = Mathf.Max(1, (stats.maxHP + addHpFlat) * hpMult);
                if(controller != null)
                {
                    controller.CurrentHP = stats.maxHP;
                }
            }
        }

        if(dmgMult != 1f)
        {
            stats.damage = Mathf.Max(1, stats.damage * dmgMult);
        }
    }
}