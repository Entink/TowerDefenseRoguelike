using UnityEngine;

public static class UnitSkillTreeRuntime
{
    public static void ApplyToRecruitment(UnitStats stats, ref int cost, ref float cooldown)
    {
        var db = Resources.Load<UnitSkillTreeDatabase>("UnitSkillTrees");
        if (db == null || stats == null) return;
        var tree = db.Get(stats.unitId);
        if (tree == null || tree.nodes == null) return;

        float costMult = 1f;
        int costFlat = 0;

        float cdMult = 1f;
        float cdFlat = 0f;


        foreach(var node in tree.nodes)
        {
            if (node == null) continue;
            int lvl = UnitSkillProgress.GetLevel(stats.unitId, node.nodeId);
            if (lvl <= 0) continue;

            for(int i = 0; i < lvl; i++)
            {
                if (node.reduceRecruitCostPercent > 0f) costMult *= (1f - node.reduceRecruitCostPercent);
                if (node.addRecruitCostFlat != 0) costFlat += node.addRecruitCostFlat;

                if (node.reduceRecruitCooldownPercent > 0f) cdMult *= (1f - node.reduceRecruitCooldownPercent);
                if (node.addRecruitCooldownFlat != 0f) cdFlat += node.addRecruitCooldownFlat;
            }
        }

        float newCost = (cost + costFlat) * costMult;
        float newCd = (cooldown + cdFlat) * cdMult;

        cost = Mathf.Max(0, Mathf.RoundToInt(newCost));
        cooldown = Mathf.Max(0.05f, newCd);
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
        float addDmgFlat = 0f;

        float rangeMult = 1f;
        float rangeFlat = 0f;

        float aoeMult = 1f;
        float aoeFlat = 0f;
        bool forceAOE = false;

        int addMulti = 0;

        float kbResist = 0f;
        float regenPS = 0f;
        float lifesteal = 0f;

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
                if (node.addFlatDMG != 0f) addDmgFlat += node.addFlatDMG;

                if (node.addPercentRange != 0f) rangeMult *= (1f + node.addPercentRange);
                if (node.addFlatRange != 0f) rangeFlat += node.addFlatRange;

                if (node.addPercentAoe != 0f) aoeMult *= (1f + node.addPercentAoe);
                if (node.addFlatAoe != 0f) aoeFlat += node.addFlatAoe;
                if (node.enableAOE) forceAOE = true;
    
                if (node.addMultiStrike != 0) addMulti += node.addMultiStrike;

                if (node.knockbackResist != 0f) kbResist += node.knockbackResist;
                if (node.regenPerSec != 0f) regenPS += node.regenPerSec;
                if (node.lifeSteal != 0f) lifesteal += node.lifeSteal;
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

        if(addDmgFlat != 0f)
        {
            stats.damage = Mathf.Max(0, stats.damage + addDmgFlat);
        }

        if (forceAOE) stats.isAOE = true;
        if (stats.isAOE && (aoeMult != 1f || aoeFlat != 0f))
        {
            stats.aoeRadius = Mathf.Max(0.05f, (stats.aoeRadius + aoeFlat) * aoeMult);
        }

        if (addMulti != 0)
        {
            stats.multiStrikeCount = Mathf.Max(1, stats.multiStrikeCount + addMulti);
            stats.multiStrikeDelay = stats.multiStrikeDelay + (1 / (2 * Mathf.Max(0.01f, stats.attackSpeed)));
        }

        if (kbResist != 0f)
        {
            stats.kbRes = Mathf.Clamp01(stats.kbRes + kbResist);
        }

        var sc = controller != null ? controller.GetComponent<StatusController>() : stats.GetComponent<StatusController>();
        if(sc != null)
        {
            float baseRange = stats.attackRange;
            float rangeAddTotal = baseRange * (rangeMult - 1f) + rangeFlat;

            var perm = new BuffStatModifierEffect
            {
                duration = 0f,
                damageMul = dmgMult,
                rangeAdd = rangeAddTotal,
                lifeStealAdd = lifesteal
            };
            sc.Apply(perm);

            if(regenPS > 0f)
            {
                sc.Apply(new RegenFlatEffect { duration = 0f, healPerSecond = regenPS });
            }
        }

        if(controller != null)
        {
            controller.regenPerSecond += 0f;
            controller.lifeSteal = controller.lifeSteal + 0f;
        }
            
    }
}