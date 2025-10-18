using System;
using System.Collections.Generic;
using UnityEngine;

public partial class RunData : MonoBehaviour
{
    
    public static RunData I { get; private set; }

    [Header("Runtime")]
    [SerializeField] private List<GameObject> _selectedUnits = new List<GameObject>();
    [SerializeField] public List<RunModifierState> activeModifiers = new List<RunModifierState>();

    
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

        RunResources.LoadMaterials();

        selectedUnits = _selectedUnits;
        LoadState();
    }


    

    public void AddModifier(RunModifierId id, int stacks = 1)
    {
        var existing = activeModifiers.Find(m => m.id == id);
        if (existing != null) existing.stacks += stacks;
        else activeModifiers.Add(new RunModifierState(id, stacks));

        SaveState();
        OnModifiersChanged?.Invoke();
    }

    public static void StaticAddModifier(RunModifierId id, int stacks = 1)
    {
        if(I == null) { Debug.LogWarning("[RunData] AddModifier called but RunData.I is null"); return; }
        I.AddModifier(id, stacks);
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

    public static List<string> GetSelectedUnitIds()
    {
        var result = new List<string>();

        foreach(var go in selectedUnits)
        {
            if (!go) continue;

            var stats = go.GetComponent<UnitStats>();
            if (stats != null)
            {
                result.Add(stats.unitId.ToString());
            }
            else
            {
                result.Add(go.name);
            }
        }

        return result;
    }

    public static void AddSelectedUnitById(UnitId id)
    {
        var db = Units.DB;
        var def = db != null ? db.Get(id) : null;
        if(def != null && def.unitPrefab != null)
        {
            selectedUnits.Add(def.unitPrefab);
        }
        else
        {
            Debug.LogWarning($"[RunData] Missing UnitDef/prefab of {id}");
        }
        
    }

    public static void AddSelectedUnitByName(string idString)
    {
        if(System.Enum.TryParse<UnitId>(idString, out var enumId))
        {
            AddSelectedUnitById(enumId);
        }
        else
        {
            Debug.LogWarning($"[RunData] Invalid UnitId in JSON: {idString}");
        }
    }

    public static List<RunModifierState> GetActiveModifiers()
    {
        return I != null ? I.activeModifiers : new List<RunModifierState>();
    }

    public static void ClearModifiers()
    {
        if (I != null) I.activeModifiers.Clear();
    }

    public static void ClearSelectedUnits() => selectedUnits.Clear();
    
}