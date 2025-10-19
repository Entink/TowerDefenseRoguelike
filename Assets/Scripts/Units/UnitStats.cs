using UnityEngine;

public class UnitStats : MonoBehaviour
{

    [Header("Unit Info")]
    public string unitName;
    public bool ally;
    public UnitId unitId;

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


    [Header("On-Hit Effects")]
    public bool onHitPoison;
    public float onHitPoisonDuration = 3f;
    public float onHitPoisonPercentPerSecond = 0.02f;
    public int onHitPoisonMaxStacks = 3;

    public bool onHitBurn;
    public float onHitBurnDuration = 3f;
    public float onHitBurnDps = 5f;
    

    

}
