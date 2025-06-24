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
        cost = Mathf.RoundToInt(stats.cost * UnitStatsModifiers.discountMultiplier);
        cooldown = stats.cooldown * UnitStatsModifiers.cooldownMultiplier;
    }



}