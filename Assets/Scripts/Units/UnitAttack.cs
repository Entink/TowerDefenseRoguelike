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

            if (stats.isAOE)
            {
                if (closest != null)
                {
                    Vector3 aoeCenter = closest.transform.position;
                    lastAOECenter = aoeCenter;

                    var aoeHits = Physics2D.OverlapCircleAll(aoeCenter, stats.aoeRadius, enemyLayer);

                    foreach(var h in aoeHits)
                    {
                        if (h == null) continue;
                        var uc = h.GetComponent<UnitController>();
                        var bc = h.GetComponent<BaseController>();
                        if (uc == null && bc == null) continue;
                        targets.Add(h);
                    }

                }
            }
            else
            {
                if (closest != null)
                    targets.Add(closest);
            }


            for (int i = 0; i < stats.multiStrikeCount; i++)
            {
                foreach (Collider2D target in targets)
                {
                    if (target == null)
                    {
                        continue;
                    }

                    UnitController uc = target.GetComponent<UnitController>();
                    if (uc != null)
                    {
                        UnitStats attackerStats = stats;
                        UnitStats targetStats = target.GetComponent<UnitStats>();

                        float typeMul = 1f;

                        if(attackerStats != null && targetStats != null)
                        {
                            typeMul = UnitTypeHelper.GetBonusMultiplier(attackerStats.unitype, targetStats.unitype);
                        }

                        float finalDamage = attackerStats.damage * typeMul * dmgMul;
                        uc.TakeDamage(finalDamage, effectiveKB);

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

                        var sc = uc.GetComponent<StatusController>();
                        if(sc != null)
                        {
                            if (hitSource != null)
                                hitSource.ApplyTo(uc);
                        }

                        continue;
                    }

                    BaseController bc = target.GetComponent<BaseController>();
                    if (bc != null)
                    {
                        float finalDamage = stats.damage * dmgMul;
                        bc.TakeDamage(finalDamage);
                        if (CombatStatsTracker.I != null)
                        {
                            if (controller != null && controller.IsAlly && !bc.isPlayerBase)
                                CombatStatsTracker.I.OnDamageDealt(stats.damage);
                            else if (controller != null && !controller.IsAlly && bc.isPlayerBase)
                                CombatStatsTracker.I.OnDamageTaken(stats.damage);
                        }
                    }
                }

                if (i < stats.multiStrikeCount - 1)
                {
                    yield return new WaitForSeconds(stats.multiStrikeDelay);
                }
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
