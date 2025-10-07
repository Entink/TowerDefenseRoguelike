using UnityEngine;
[System.Serializable]
public class UnitTemplate
{
    public GameObject prefab;
    public KeyCode hotkey;

    public int cost;
    public float cooldown;
    public string unitName;


    public UnitTemplate(GameObject prefab, KeyCode hotkey)
    {
        this.prefab = prefab;
        this.hotkey = hotkey;

        UnitStats stats = prefab.GetComponent<UnitStats>();
        unitName = stats.unitName;

        int baseCost = stats.cost;
        float baseCd = stats.cooldown;

        

        int costTmp = baseCost;
        float cdTmp = baseCd;

        
        UnitSkillTreeRuntime.ApplyToRecruitment(stats, ref costTmp, ref cdTmp);

        cost = Mathf.RoundToInt(costTmp * UnitStatsModifiers.discountMultiplier);
        cooldown = cdTmp * UnitStatsModifiers.cooldownMultiplier;
    }



}