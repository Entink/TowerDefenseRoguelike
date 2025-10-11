using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{

    [SerializeField] RunData runData;
    public Transform spawnPoint;
    
    public List<UnitTemplate> units = new List<UnitTemplate>();
    private float[] nextAvailableTimes;

    private void Awake()
    {
        runData = FindAnyObjectByType<RunData>();

        if (spawnPoint == null)
        {
            BaseController[] bases = FindObjectsOfType<BaseController>();
            foreach (var b in bases)
            {
                if (b.isPlayerBase)
                {
                    spawnPoint = b.transform;
                    break;
                }
            }

            if (spawnPoint == null)
            {
                Debug.LogWarning("Nie znaleziono bazy gracza.");
            }
        }
    }

    private void Start()
    {
        units.Clear();

        for(int i = 0; i < RunData.selectedUnits.Count; i++)
        {
            GameObject prefab = RunData.selectedUnits[i];
            KeyCode hotkey = GetHotkeyForIndex(i);
            units.Add(new UnitTemplate(prefab, hotkey));
        }

        nextAvailableTimes = new float[units.Count];
    }

    private KeyCode GetHotkeyForIndex(int index)
    {
        if(index >= 0 && index <= 8)
        {
            return KeyCode.Alpha1 + index;
        }
        else if(index == 9)
        {
            return KeyCode.Alpha0;
        }
        else
        {
            return KeyCode.None;
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.IsInputAllowed()) return;

        for (int i = 0; i < units.Count; i++)
        {
            if (Input.GetKeyDown(units[i].hotkey))
            {
                TrySpawnUnit(i);
            }
        }
    }

    public void TrySpawnUnit(int index)
    {
        if (!GameManager.instance.IsInputAllowed()) return;


        if (index < 0 || index >= units.Count)
        {
            return;
        }


        UnitTemplate template = units[index];
        UnitStats stats = template.prefab.GetComponent<UnitStats>();
        if (stats == null)
        {
            return;
        }

        var (finalCost, finalCd) = GetAdjustedForIndex(index);

        float currentTime = Time.time;
        if (currentTime < nextAvailableTimes[index])
        {
            Debug.Log("Jednostka jest na cooldownie");
            return;
        }

        if (CurrencyManager.instance.CanAfford(finalCost) && GameManager.instance.CanSpawnUnit())
        {
            CurrencyManager.instance.SpendGold(finalCost);
            GameObject unit = Instantiate(template.prefab, spawnPoint.position, Quaternion.identity);
            var sts = unit.GetComponent<UnitStats>();
            var controller = unit.GetComponent<UnitController>();

            UnitSkillTreeRuntime.ApplyOnSpawn(sts, controller);

            var mods = (runData != null) ? runData.activeModifiers : RunData.I?.activeModifiers;
            RunModifiers.ApplyOnSpawn(mods, sts, controller);
            


            nextAvailableTimes[index] = currentTime + finalCd;
        }
        else
        {
            Debug.Log("Nie staæ lub limit jednostek");
        }

        //Debug.Log($"IDX {index} base {units[index].cost}/{units[index].cooldown} -> final {finalCost}/{finalCd}");

    }

    public float GetCooldownRemaming(int index)
    {
        return Mathf.Max(0, nextAvailableTimes[index] - Time.time);
    }

    public float GetUnitCooldown(int index)
    {
        return GetAdjustedForIndex(index).cd;
    }

    public int GetUnitCost(int index)
    {
        return GetAdjustedForIndex(index).cost;
    }

    public string GetUnitName(int index)
    {
        return units[index].unitName;
    }

    private (int cost, float cd) GetAdjustedForIndex(int index)
    {
        var t = units[index];
        float baseCost = t.cost;
        float baseCd = t.cooldown;

        var mods = (runData != null) ? runData.activeModifiers : RunData.I?.activeModifiers;
        var adj = RunModifiers.GetAdjustedRecruitment(baseCost, baseCd, mods);

        int finalCost = Mathf.CeilToInt(adj.cost);
        float finalCd = adj.cooldown;
        return (finalCost, finalCd);
    }
}
