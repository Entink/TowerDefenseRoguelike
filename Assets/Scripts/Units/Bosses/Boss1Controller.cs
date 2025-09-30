using UnityEngine;

[RequireComponent(typeof(UnitController))]
[RequireComponent(typeof(UnitAttack))]
[RequireComponent(typeof(UnitStats))]

public class Boss1Controller : MonoBehaviour
{
    [Header("Stun AOE")]
    [SerializeField] private float stunRadius = 4.5f;
    [SerializeField] private float stunDuration = 2.0f;
    [SerializeField] private LayerMask playerUnitMask;

    [Header("Every Nth Attack")]
    [SerializeField] private int attackPerStun = 3;
    [SerializeField] private int attackCounter = 0;

    [Header("Threshold Summon")]
    [SerializeField] private float t1 = 0.66f;
    [SerializeField] private float t2 = 0.33f;
    [SerializeField] private float t3 = 0.10f;

    [SerializeField] private GameObject[] summonPrefabs;
    [SerializeField] private int summonCount = 2;

    [Header("Retreat")]
    [SerializeField] private Transform enemyBase;
    [SerializeField] private float retreatStopDistance = 0.5f;

    private bool trig1, trig2, trig3;
    private UnitController ctrl;
    private UnitAttack atk;
    private UnitStats stats;
    private float maxHPCached;
    [SerializeField] private bool isRetreating = false;

    private void Awake()
    {
        ctrl = GetComponent<UnitController>();
        atk = GetComponent<UnitAttack>();
        stats = GetComponent<UnitStats>();
    }


    private void Start()
    {
        maxHPCached = stats.maxHP;
        if (atk != null) atk.OnAttackFired += HandleAttackFired;

        if(enemyBase == null)
        {
            var baseObj = GameObject.FindGameObjectWithTag("EnemyBase");
            if (baseObj != null) enemyBase = baseObj.transform;
        }
    }

    private void OnDestroy()
    {
        if (atk != null) atk.OnAttackFired -= HandleAttackFired;
    }

    private void Update()
    {
        float hpratio = ctrl.CurrentHP / Mathf.Max(1, maxHPCached);

        if(!trig1 && hpratio <= t1) { TriggerThreshold(); trig1 = true; }
        if(!trig2 && hpratio <= t2) { TriggerThreshold(); trig2 = true; }
        if(!trig3 && hpratio <= t3) { TriggerThreshold(); trig3 = true; }

        if(isRetreating && enemyBase != null)
        {
            float dist = Mathf.Abs(transform.position.x - enemyBase.position.x);
            if(dist <= retreatStopDistance)
            {
                isRetreating = false;
            }
        }
    }

    private void HandleAttackFired()
    {
        attackCounter++;
        if(attackCounter % attackPerStun == 0)
        {
            Debug.Log("STUN");
            DoAoeStun();
        }
    }

    private void DoAoeStun()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, stunRadius, playerUnitMask);
        for(int i = 0; i < hits.Length; i++)
        {
            var unit = hits[i].GetComponent<UnitController>();
            if(unit != null && unit.IsAlly)
            {
                Debug.Log($"Has stunned {unit.name} for {stunDuration}");
                unit.ApplyStun(stunDuration);
            }
        }
    }

    private void TriggerThreshold()
    {
        for (int i = 0; i < summonCount; i++)
        {
            var prefab = summonPrefabs != null && summonPrefabs.Length > 0
                ? summonPrefabs[Random.Range(0, summonPrefabs.Length)]
                : null;

            if(prefab != null)
            {
                Vector3 pos = transform.position + new Vector3(0.3f * (i - (summonCount - 1) * 0.5f), 0f, 0f);
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }

        isRetreating = true;

        var ai = GetComponent<Boss1SimpleAI>();
        if (ai != null && enemyBase != null) ai.RetreatTo(enemyBase.position.x);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}