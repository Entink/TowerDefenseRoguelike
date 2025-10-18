using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttack : MonoBehaviour
{
    private SFXUnit sfx;

    private UnitStats stats;
    private UnitController controller;
    private Coroutine attackCoroutine;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

    private Vector3? lastAOECenter = null;

    public System.Action OnAttackFired;
    

    private void Start()
    {
        stats = GetComponent<UnitStats>();
        controller = GetComponent<UnitController>();

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
        yield return new WaitForSeconds((1f / stats.attackSpeed) / 2f);
        
        while(true)
        {
            if(controller != null && controller.IsStunned)
            {
                yield return null;
                continue;
            }


            List<Collider2D> targets = new List<Collider2D>();

            var closest = FindClosestEnemy(transform.position, stats.attackRange, enemyLayer);

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

                        float multiplier = 1f;

                        if(attackerStats != null && targetStats != null)
                        {
                            multiplier = UnitTypeHelper.GetBonusMultiplier(attackerStats.unitype, targetStats.unitype);
                        }

                        float finalDamage = attackerStats.damage * multiplier;
                        uc.TakeDamage(finalDamage, stats.kbForce);
                        if(CombatStatsTracker.I != null)
                        {
                            if (controller != null && controller.IsAlly)
                                CombatStatsTracker.I.OnDamageDealt(finalDamage);
                            else
                                CombatStatsTracker.I.OnDamageTaken(finalDamage);
                        }
                        if(controller != null && controller.lifeSteal > 0f)
                        {
                            controller.Heal(finalDamage * controller.lifeSteal);
                        }
                        continue;
                    }

                    BaseController bc = target.GetComponent<BaseController>();
                    if (bc != null)
                    {
                        bc.TakeDamage(stats.damage);
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

            yield return new WaitForSeconds(1f / stats.attackSpeed);
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
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);

        if(stats.isAOE && lastAOECenter.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastAOECenter.Value, stats.aoeRadius);
        }
    }
}
