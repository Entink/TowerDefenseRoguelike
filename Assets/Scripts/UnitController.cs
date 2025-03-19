using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UnitStats stats;

    [SerializeField, ReadOnly] private float currentHP;
    [SerializeField, ReadOnly] private HPBar hpBar;
    [SerializeField, ReadOnly] private SpriteRenderer spriteRenderer;

    [SerializeField, ReadOnly] private bool isAlly;
    private Vector3 moveDirection;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;



    public float CurrentHP
    {
        get { return currentHP;  }
        set
        { 
            currentHP = Mathf.Clamp(value,0,stats.maxHP);
            hpBar.SetHP(currentHP, stats.maxHP);
        }
    }

    public bool IsAlly
    {
        get { return isAlly; }
        set { isAlly = value;  }
    }

    private void Awake()
    {

        stats = GetComponent<UnitStats>();
        hpBar = GetComponentInChildren<HPBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        hpBar.Hide();

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
        StartCoroutine(HitEffect());

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        if(hpBar != null)
        {
            HPBarManager.instance.UnregisterHPBar(hpBar);
        }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        hpBar.Show();
        HPBarManager.instance.SetHoveredState(hpBar, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hpBar.Hide();
        HPBarManager.instance.SetHoveredState(hpBar, false);

    }
}
