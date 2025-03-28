using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [SerializeField] private int startingGold;
    [SerializeField, ReadOnly] private int currentGold;

    [SerializeField] private float goldGainInterval;
    [SerializeField] private int goldPerInterval;

    public UnityEvent<int> OnGoldChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        currentGold = startingGold;
        OnGoldChanged?.Invoke(currentGold);
        InvokeRepeating(nameof(PassiveIncome), goldGainInterval, goldGainInterval);
    }

    private void PassiveIncome()
    {
        AddGold(goldPerInterval);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool CanAfford(int amount)
    {
        return currentGold >= amount;
    }

    public void SpendGold(int amount)
    {
        if(CanAfford(amount))
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
        }
    }

    public int GetGold()
    {
        return currentGold;
    }

}
