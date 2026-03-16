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

    [Header("AOE Feedback")]
    [SerializeField] private ParticleSystem stunAoeVfx;
    [SerializeField] private AudioClip stunAoeClip;
    [SerializeField, Range(0f, 1f)] private float stunAoeSfxVolume = 1f;

    [Header("Every Nth Attack")]
    [SerializeField] private int attackPerStun = 3;
    [SerializeField] private int attackCounter = 0;

    [Header("Threshold Summon")]
    [SerializeField] private float t1 = 0.66f;
    [SerializeField] private float t2 = 0.33f;
    [SerializeField] private float t3 = 0.10f;

    [SerializeField] private GameObject[] summonPrefabs;
    [SerializeField] private int summonCount = 2;

    [Header("Threshold SFX")]
    [SerializeField] private AudioClip threshold1Clip;
    [SerializeField] private AudioClip threshold2Clip;
    [SerializeField] private AudioClip threshold3Clip;
    [SerializeField, Range(0f, 1f)] private float thresholdSfxVolume = 1f;
    


    [Header("Retreat")]
    [SerializeField] private Transform enemyBase;
    [SerializeField] private float retreatStopDistance = 0.5f;
    [SerializeField] private bool isRetreating = false;


    [Header("Screen Shake")]
    [SerializeField] private float spawnShakeAmplitude = 0.35f;
    [SerializeField] private float spawnShakeFrequency = 22f;
    [SerializeField] private float spawnShakeDuration = 0.45f;
    private bool spawnShakePlayed;



    private bool trig1, trig2, trig3;
    private UnitController ctrl;
    private UnitAttack atk;
    private UnitStats stats;
    private float maxHPCached;

    private void Awake()
    {
        ctrl = GetComponent<UnitController>();
        atk = GetComponent<UnitAttack>();
        stats = GetComponent<UnitStats>();
    }


    private void Start()
    {
        PlaySpawnShakeOnce();
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

        if(!trig1 && hpratio <= t1) { TriggerThreshold(1); trig1 = true; }
        if(!trig2 && hpratio <= t2) { TriggerThreshold(2); trig2 = true; }
        if(!trig3 && hpratio <= t3) { TriggerThreshold(3); trig3 = true; }

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
        if (stunAoeVfx != null)
            stunAoeVfx.Play();

        if (SfxManager.I != null && stunAoeClip != null)
            SfxManager.I.PlayWorldOneShot(stunAoeClip, transform.position, stunAoeSfxVolume);

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

    private void TriggerThreshold(int thresholdIndex)
    {
        PlayThresholdSfx(thresholdIndex);


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

    private void PlaySpawnShakeOnce()
    {
        if (spawnShakePlayed)
            return;

        spawnShakePlayed = true;
        CameraShake2D.Instance?.TriggerShake(spawnShakeAmplitude, spawnShakeFrequency, spawnShakeDuration);
    }

    private void PlayThresholdSfx(int thresholdIndex)
    {
        if (SfxManager.I == null)
            return;

        AudioClip clipToPlay = null;

        switch(thresholdIndex)
        {
            case 1:
                clipToPlay = threshold1Clip;
                break;

            case 2:
                clipToPlay = threshold2Clip;
                break;

            case 3:
                clipToPlay = threshold3Clip;
                break;

            default:
                break;
        }

        if (clipToPlay != null)
            SfxManager.I.PlayWorldOneShot(clipToPlay, transform.position, thresholdSfxVolume);

    }
}