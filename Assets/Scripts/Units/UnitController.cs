using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SFXUnit sfx;
    private UnitStats stats;

    [SerializeField, ReadOnly] private float currentHP;
    [SerializeField, ReadOnly] private HPBar hpBar;

    [SerializeField, ReadOnly] private SpriteRenderer spriteRenderer;
    [ReadOnly] private ParticleSystem deathEffect;

    [SerializeField, ReadOnly] private bool isAlly;
    private Vector3 moveDirection;

    [SerializeField, ReadOnly] private LayerMask enemyLayer;

    [SerializeField] private bool isStunned = false;
    [SerializeField] private float stunEndTime = 0f;

    [SerializeField] public bool IsStunned => isStunned;

    [Header("Tree runtime bonuses")]
    public float regenPerSecond = 0f;
    public float lifeSteal = 0f;


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
        deathEffect = GetComponentInChildren<ParticleSystem>();
        sfx = GetComponent<SFXUnit>();

        IsAlly = stats.ally;

        enemyLayer = isAlly ? LayerMask.GetMask("Enemies") : LayerMask.GetMask("Allies");

    }
    private void Start()
    {



        if (!string.IsNullOrEmpty(stats.unitName))
        {
            gameObject.name = stats.unitName;
        }
        if (isAlly)
        {
            if (GameManager.instance.CanSpawnUnit())
            {
                GameManager.instance.RegisterUnit();
            }
            else
            {
                Debug.LogWarning($"Nie mo�na pojawi� wi�cej jednostek. Osi�gni�to limit {GameManager.instance.currentUnits}/{GameManager.instance.GetMaxUnits()}");
                Destroy(gameObject);
            }
        }
        

        CurrentHP = stats.maxHP;
        hpBar.Hide();

        moveDirection = isAlly ? Vector3.left : Vector3.right;

    }

    private void FixedUpdate()
    {
        if(isStunned && Time.time >= stunEndTime)
            isStunned = false;

        if (IsStunned) return;

        if(regenPerSecond > 0f && currentHP > 0 && currentHP < stats.maxHP)
        {
            Heal(regenPerSecond * Time.deltaTime);
        }

        if(!IsEnemyInFront())
        {
            Move();
        }
    }


    //Health subsystem
    public void TakeDamage(float dmg, float kbPower)
    {
        CurrentHP -= dmg;
        sfx?.PlayHit();
        StartCoroutine(HitEffect());

        float finalKB = kbPower * (1f - stats.kbRes);
        if(kbPower > 0f)
        {
            StartCoroutine(Knockback(finalKB));
        }

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator Knockback(float force)
    {
        float duration = 0.1f;
        float time = 0;

        Vector3 startPosition = transform.position;
        Vector3 knockbackPosition = startPosition - moveDirection * force;

        while (time<duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, knockbackPosition, time / duration);
            yield return null;
        }

        transform.position = knockbackPosition;
    }

    private IEnumerator HitEffect()
    {
        Color hitColor = new Color(1f, 0.5f, 0.5f, 1f);
        Color originalColor = Color.white;

        float duration = 0.1f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(hitColor, originalColor, time / duration);
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if(CombatStatsTracker.I != null)
        {
            if (isAlly)
                CombatStatsTracker.I.OnPlayerUnitKilled();
            else
                CombatStatsTracker.I.OnEnemyKilled();
        }
        sfx?.PlayDeath();
        if(isAlly)
        {
            if (hpBar != null)
            {
                HPBarManager.instance.UnregisterHPBar(hpBar);
            }

            GameManager.instance.UnregisterUnit();
        }
        

        if(deathEffect != null)
        {
            deathEffect.transform.parent = null;
            deathEffect.Play();
            Destroy(deathEffect.gameObject, 1f);
        }

        Destroy(gameObject);
    }


    //Movement subsystem
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

            BaseController baseController = hit.collider.GetComponent<BaseController>();
            if (baseController != null && baseController.isPlayerBase != this.IsAlly)
            {
                return true;
            }
        }
        return false;
    }


    //Misc 

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

    public void ApplyStun(float duration)
    {
        if (duration <= 0f) return;
        isStunned = true;
        stunEndTime = Time.time + duration;
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || currentHP <= 0f) return;
        currentHP = Mathf.Min(stats.maxHP, currentHP + amount);
    }
}
