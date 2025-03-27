using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttack : MonoBehaviour
{
    private UnitStats stats;
    private UnitController controller;
    private Coroutine attackCoroutine;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

    private Vector3? lastAOECenter = null;
    

    private void Start()
    {
        stats = GetComponent<UnitStats>();
        controller = GetComponent<UnitController>();

        enemyLayer = controller.GetEnemyLayer();

        if (stats.attackSpeed > 0)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds((1f / stats.attackSpeed) / 2f);
        
        while(true)
        {
            List<Collider2D> targets = new List<Collider2D>();

            if (stats.isAOE)
            {
                Collider2D closest = Physics2D.OverlapCircle(transform.position, stats.attackRange, enemyLayer);
                if (closest != null)
                {
                    Vector3 aoeCenter = closest.transform.position;
                    lastAOECenter = aoeCenter;
                    targets.AddRange(Physics2D.OverlapCircleAll(aoeCenter, stats.aoeRadius, enemyLayer));

                }
            }
            else
            {
                Collider2D single = Physics2D.OverlapCircle(transform.position, stats.attackRange, enemyLayer);
                if(single != null)
                {
                    targets.Add(single);
                }
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
                        uc.TakeDamage(stats.damage, stats.kbForce);
                        continue;
                    }

                    BaseController bc = target.GetComponent<BaseController>();
                    if (bc != null)
                    {
                        bc.TakeDamage(stats.damage);
                    }
                }

                if (i < stats.multiStrikeCount - 1)
                {
                    yield return new WaitForSeconds(stats.multiStrikeDelay);
                }
            }
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
