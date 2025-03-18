using UnityEngine;
using System.Collections;

public class UnitAttack : MonoBehaviour
{
    private UnitStats stats;
    private UnitController controller;
    private Coroutine attackCoroutine;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

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
        while(true)
        {
            yield return new WaitForSeconds(1f / stats.attackSpeed);

            Collider2D enemy = Physics2D.OverlapCircle(transform.position, stats.attackRange, enemyLayer);
            if (enemy != null)
            {
                UnitController enemyController = enemy.GetComponent<UnitController>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(stats.damage);
                }
            }
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
    }
}
