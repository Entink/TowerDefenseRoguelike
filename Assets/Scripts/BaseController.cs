using UnityEngine;

public class BaseController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP;
    [SerializeField, ReadOnly] private float currentHP;
    [SerializeField] private BaseHPBarUI hPBarUI;

    [SerializeField] public bool isPlayerBase = false;

    private bool died = false;

    private void Start()
    {
        
        currentHP = maxHP;
        hPBarUI?.SetHP(currentHP, maxHP);
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{gameObject.name} take {dmg} damage. {currentHP}/{maxHP}");
        hPBarUI.SetHP(currentHP, maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (died) return;
        died = true;
        Debug.Log($"{gameObject.name} was destroyed!");
        GameManager.instance.OnBaseDestroyed(isPlayerBase);

        //GameManager.instance.EndFight();
        Destroy(gameObject);
    }
}
