using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitStats stats;
    [SerializeField, ReadOnly] private float currentHP;
    [SerializeField, ReadOnly] private bool isAlly;
    private Vector3 moveDirection;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

    public float CurrentHP
    {
        get { return currentHP;  }
        set { currentHP = Mathf.Clamp(value,0,stats.maxHP); }
    }

    public bool IsAlly
    {
        get { return isAlly; }
        set { isAlly = value;  }
    }

    private void Awake()
    {
        stats = GetComponent<UnitStats>();
        IsAlly = stats.ally;
        enemyLayer = isAlly ? LayerMask.GetMask("Enemies") : LayerMask.GetMask("Allies");

    }
    private void Start()
    {

        if (!string.IsNullOrEmpty(stats.unitName))
        {
            gameObject.name = stats.unitName;
        }

        CurrentHP = stats.maxHP;

        moveDirection = isAlly ? Vector3.left : Vector3.right;

    }

    private void FixedUpdate()
    {
        if(!IsEnemyInFront())
        {
            Move();
        }
    }

    public void TakeDamage(float dmg)
    {
        CurrentHP -= dmg;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void Move()
    {
        transform.Translate(moveDirection * stats.speed * Time.deltaTime);
    }

    private float attackBuffer = 0.1f;

    private bool IsEnemyInFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, stats.attackRange-attackBuffer, enemyLayer);
        if(hit.collider != null)
        {
            UnitController unit = hit.collider.GetComponent<UnitController>();
            if (unit != null && unit.IsAlly != this.IsAlly)
            {
                return true;
            }
        }
        return false;
    }

    public LayerMask GetEnemyLayer()
    {
        return enemyLayer;
    }
}
