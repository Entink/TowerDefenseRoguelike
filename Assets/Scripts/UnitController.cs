using UnityEngine;

public class UnitController : MonoBehaviour
{
    private UnitStats stats;
    [SerializeField, ReadOnly] private float currentHP;
    [SerializeField, ReadOnly] private bool isAlly;
    private Vector3 moveDirection;

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
    private void Start()
    {
        stats = GetComponent<UnitStats>();

        if (!string.IsNullOrEmpty(stats.unitName))
        {
            gameObject.name = stats.unitName;
        }

        CurrentHP = stats.maxHP;
        IsAlly = stats.ally;

        moveDirection = isAlly ? Vector3.left : Vector3.right;
    }

    private void FixedUpdate()
    {
        Move();
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

    
}
