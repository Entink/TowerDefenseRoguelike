using UnityEngine;

public class UnitStats : MonoBehaviour
{

    [Header("Unit Info")]
    public string unitName;
    public bool ally;

    [Header("Type")]
    public UnitType unitype;

    [Header("Health")]
    public float maxHP;

    [Header("Movement")]
    public float speed;

    [Header("Combat")]
    public float attackSpeed;
    public float damage;
    public float attackRange;
    public int multiStrikeCount;
    public float multiStrikeDelay;

    [Header("AOE")]
    public bool isAOE;
    public float aoeRadius;

    [Header("Knockback")]
    public float kbForce;
    [Range(0f, 1f)] public float kbRes;

    [Header("Economy")]
    public int cost;
    public float cooldown;


    private void Awake()
    {
        maxHP *= UnitStatsModifiers.hpMultiplier;
        damage *= UnitStatsModifiers.damageMultiplier;
        cooldown *= UnitStatsModifiers.cooldownMultiplier;
    }

}
