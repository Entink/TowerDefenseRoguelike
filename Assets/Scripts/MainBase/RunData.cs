using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RunData : MonoBehaviour
{
    //SINGLETON
    public static RunData I { get; private set; }

    [Header("Runtime")]
    [SerializeField] private List<GameObject> _selectedUnits = new List<GameObject>();
    [SerializeField] public List<RunModifierState> activeModifiers = new List<RunModifierState>();

    //TYMCZASOWY BACKWARD-COMPAT
    [Obsolete("purchasedItems jest wygaszane. Uzywac systemu modyfikatorow (activeModifiers) i Shopv2.")]
    public static List<ShopItemData> purchasedItems = new List<ShopItemData>();
    public static List<GameObject> selectedUnits = new List<GameObject>();

    public Action OnModifiersChanged;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        selectedUnits = _selectedUnits;
        LoadState();
    }


    // Public API
    [Obsolete("AddShopItem jest wygaszane. Nie uzywac w nowych miejscach.")]
    public static void AddShopItem(ShopItemData item)
    {
        purchasedItems.Add(item);
    }

    public void AddModifier(RunModifierId id, int stacks = 1)
    {
        var existing = activeModifiers.Find(m => m.id == id);
        if (existing != null) existing.stacks += stacks;
        else activeModifiers.Add(new RunModifierState(id, stacks));

        SaveState();
        OnModifiersChanged?.Invoke();
    }

    public void RemoveAllModifiers()
    {
        activeModifiers.Clear();
        SaveState();
        OnModifiersChanged?.Invoke();
    }

    public static void ResetRun()
    {
        I.RemoveAllModifiers();
        RunDataPersistence.Clear();
        selectedUnits.Clear();
        purchasedItems.Clear();
        UnitStatsModifiers.Reset();
        RunResources.Reset();
        MapRunData.Reset();


    }

    public void LoadState()
    {
        activeModifiers = RunDataPersistence.LoadActiveModifiers();
        if (activeModifiers == null) activeModifiers = new List<RunModifierState>();
    }

    public void SaveState()
    {
        RunDataPersistence.SaveActiveModifiers(activeModifiers);
    }
    
}