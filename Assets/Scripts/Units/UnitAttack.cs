using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttack : MonoBehaviour
{
    private SFXUnit sfx;

    private UnitStats stats;
    private UnitController controller;
    private StatusController status;
    private Coroutine attackCoroutine;
    private OnHitEffectSource hitSource;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

    private Vector3? lastAOECenter = null;

    public System.Action OnAttackFired;
    

    private void Start()
    {
        hitSource = GetComponent<OnHitEffectSource>();
        stats = GetComponent<UnitStats>();
        controller = GetComponent<UnitController>();
        status = GetComponent<StatusController>();

        enemyLayer = controller.GetEnemyLayer();
        sfx = GetComponent<SFXUnit>();
        OnAttackFired += () => sfx?.PlayAttack();
        
        if (stats.attackSpeed > 0)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }


    }

    private Collider2D FindClosestEnemy(Vector3 from, float range, LayerMask mask)
    {
        var hits = Physics2D.OverlapCircleAll(from, range, mask);

        Collider2D best = null;
        float bestSqr = float.MaxValue;

        foreach(var h in hits)
        {
            

            if (h == null) continue;

            var uc = h.GetComponent<UnitController>();
            var bc = h.GetComponent<BaseController>();

            

            if (uc == null && bc == null) continue;

            float dx = h.transform.position.x - from.x;
            

            if (controller != null)
            {
                if (controller.IsAlly && dx > 0f) continue;
                if (!controller.IsAlly && dx < 0f) continue;
            }

            float sqr = (h.transform.position - from).sqrMagnitude;
            

            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = h;
                
            }
        }

        return best;
    }

    private IEnumerator AttackCoroutine()
    {
        float atkMul0 = status != null ? status.GetAttackSpeedMul() : 1f;
        float firstDelay = Mathf.Max(0.01f, (1f / Mathf.Max(0.01f, stats.attackSpeed * atkMul0)) / 2f);
        yield return new WaitForSeconds(firstDelay);
        
        while(true)
        {
            if(controller != null && controller.IsStunned)
            {
                yield return null;
                continue;
            }

            float atkMul = status != null ? status.GetAttackSpeedMul() : 1f;
            float dmgMul = status != null ? status.GetDamageMul() : 1f;
            float rangeAdd = status != null ? status.GetRangeAdd() : 0f;
            float kbForceMul = status != null ? status.GetKBForceMul() : 1f;

            float effectiveAS = Mathf.Max(0.01f, stats.attackSpeed * atkMul);
            float effectiveRange = Mathf.Max(0f, stats.attackRange + rangeAdd);
            float effectiveKB = stats.kbForce * kbForceMul;


            List<Collider2D> targets = new List<Collider2D>();

            var closest = FindClosestEnemy(transform.position, effectiveRange, enemyLayer);

            if(stats.isAOE)
            {
                if(closest != null)
                {
                    Vector3 aoeCenter = closest.transform.position;
                    lastAOECenter = aoeCenter;

                    var aoeHits = Physics2D.OverlapCircleAll(aoeCenter, stats.aoeRadius, enemyLayer);

                    var seenUC = new HashSet<int>();
                    var seenBC = new HashSet<int>();

                    foreach(var h in aoeHits)
                    {
                        if (h == null) continue;

                        var ucH = h.GetComponent<UnitController>();
                        var bcH = h.GetComponent<BaseController>();
                        if (ucH == null && bcH == null) continue;

                        if(ucH != null)
                        {
                            if (!seenUC.Add(ucH.GetInstanceID())) continue;
                        }
                        else if(bcH != null)
                        {
                            if (!seenBC.Add(bcH.GetInstanceID())) continue;
                        }

                        targets.Add(h);
                    }
                }
            }
            else
            {
                Vector2 dir = (controller != null && controller.IsAlly) ? Vector2.left : Vector2.right;

                var hits = Physics2D.OverlapCircleAll(transform.position, effectiveRange, enemyLayer);

                var seenUC = new HashSet<int>();
                var seenBC = new HashSet<int>();

                var ordered = new List<(Collider2D col, float proj, float distSqr)>();

                foreach(var h in hits)
                {
                    if (h == null) continue;
                    var ucH = h.GetComponent<UnitController>();
                    var bcH = h.GetComponent<BaseController>();
                    if (ucH == null && bcH == null) continue;

                    float dx = h.transform.position.x - transform.position.x;
                    if(controller != null)
                    {
                        if (controller.IsAlly && dx > 0f) continue;
                        if (!controller.IsAlly && dx < 0f) continue;
                    }

                    Vector2 to = (Vector2)(h.transform.position - transform.position);
                    float proj = Vector2.Dot(to, dir);
                    if (proj <= 0f) continue;

                    if (ucH != null)
                    {
                        if (!seenUC.Add(ucH.GetInstanceID())) continue;
                    }
                    else if (bcH != null)
                    {
                        if (!seenBC.Add(bcH.GetInstanceID())) continue;
                    }

                    ordered.Add((h,proj,to.sqrMagnitude));
                }

                ordered.Sort((a, b) =>
                {
                    int cmp = a.proj.CompareTo(b.proj);
                    if (cmp != 0) return cmp;
                    return a.distSqr.CompareTo(b.distSqr);
                });

                int maxTargets = Mathf.Max(1, stats.piercingTargets);
                for (int iSel = 0; iSel < ordered.Count && iSel < maxTargets; iSel++)
                    targets.Add(ordered[iSel].col);

                if (targets.Count == 0 && closest != null)
                    targets.Add(closest);
            }


            for (int i = 0; i < stats.multiStrikeCount; i++)
            {
                for(int tIndex = 0; tIndex < targets.Count;tIndex++)
                {
                    var target = targets[tIndex];
                    if (target == null) continue;

                    float secondaryMul = (tIndex == 0) ? 1f : Mathf.Max(0f, stats.piercingSecondaryMul);

                    UnitController uc = target.GetComponent<UnitController>();
                    if(uc != null)
                    {
                        UnitStats attackerStats = stats;
                        UnitStats targetStats = target.GetComponent<UnitStats>();

                        float typeMul = 1f;
                        if (attackerStats != null && targetStats != null)
                            typeMul = UnitTypeHelper.GetBonusMultiplier(attackerStats.unitype, targetStats.unitype);

                        float finalDamage = attackerStats.damage * typeMul * dmgMul * secondaryMul;
                        uc.TakeDamage(finalDamage, effectiveKB);
                        Debug.Log($"[UnitAttack] {this.controller.name} attacked {uc.name} for {finalDamage} [i:{i},tIndex:{tIndex}]");

                        if(CombatStatsTracker.I != null)
                        {
                            if (controller != null && controller.IsAlly)
                                CombatStatsTracker.I.OnDamageDealt(finalDamage);
                            else
                                CombatStatsTracker.I.OnDamageTaken(finalDamage);
                        }

                        float lsBonus = status != null ? status.GetLifeStealAdd() : 0f;
                        float totalLS = Mathf.Max(0f, controller != null ? controller.lifeSteal + lsBonus : lsBonus);
                        if (totalLS > 0f && controller != null)
                            controller.Heal(finalDamage * totalLS);

                        var scT = uc.GetComponent<StatusController>();
                        if (scT != null && hitSource != null)
                            hitSource.ApplyTo(uc);

                        continue;
                    }

                    BaseController bc = target.GetComponent<BaseController>();
                    if (bc != null)
                    {
                        float finalDamage = stats.damage * dmgMul * secondaryMul;
                        bc.TakeDamage(finalDamage);

                        if (CombatStatsTracker.I != null)
                        {
                            if (controller != null && controller.IsAlly && !bc.isPlayerBase)
                                CombatStatsTracker.I.OnDamageDealt(finalDamage);
                            else if (controller != null && !controller.IsAlly && bc.isPlayerBase)
                                CombatStatsTracker.I.OnDamageTaken(finalDamage);
                        }
                    }

                    
                }
                if (i < stats.multiStrikeCount - 1)
                    yield return new WaitForSeconds(stats.multiStrikeDelay);
            }

            if (targets.Count > 0)
                OnAttackFired?.Invoke();

            yield return new WaitForSeconds(1f / effectiveAS);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(stats == null)
        {
            stats = GetComponent<UnitStats>();
            if (stats == null) return;
        }

        Gizmos.color = Color.red;
        float rangeAdd = 0f;
        var sc = GetComponent<StatusController>();
        if (sc != null) rangeAdd = sc.GetRangeAdd();
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, stats.attackRange + rangeAdd));

        if(stats.isAOE && lastAOECenter.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastAOECenter.Value, stats.aoeRadius);
        }
    }
}
