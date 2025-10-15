using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [SerializeField] private int startingGold;
    [SerializeField, ReadOnly] private int currentGold;

    [SerializeField] private float goldGainInterval;
    [SerializeField] private int baseGoldPerInterval;
    float currentGoldPerInterval;

    [SerializeField] RunData runData;


    public UnityEvent<int> OnGoldChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        if (runData == null) runData = RunData.I;
        if (runData != null) runData.OnModifiersChanged += RecalculatePassiveIncome;
        RecalculatePassiveIncome();
    }

    private void OnDisable()
    {
        if (runData != null) runData.OnModifiersChanged -= RecalculatePassiveIncome;
    }

    private void Start()
    {
        currentGold = startingGold;
        OnGoldChanged?.Invoke(currentGold);
        InvokeRepeating(nameof(PassiveIncome), goldGainInterval, goldGainInterval);
    }

    private void PassiveIncome()
    {
        AddGold(Mathf.RoundToInt(currentGoldPerInterval));
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
            CombatStatsTracker.I?.OnGoldSpent(amount);
        }
    }

    public int GetGold()
    {
        return currentGold;
    }

    void RecalculatePassiveIncome()
    {
        

        currentGoldPerInterval = baseGoldPerInterval;

        var mods = (runData != null) ? runData.activeModifiers : RunData.I.activeModifiers;
        RunModifiers.ApplyEconomy(mods, ref currentGoldPerInterval);
    }

}
