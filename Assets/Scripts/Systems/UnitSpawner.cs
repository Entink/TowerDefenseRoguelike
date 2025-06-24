using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    
    public List<UnitTemplate> units = new List<UnitTemplate>();
    private float[] nextAvailableTimes;

    private void Awake()
    {
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



        float currentTime = Time.time;
        if (currentTime < nextAvailableTimes[index])
        {
            Debug.Log("Jednostka jest na cooldownie");
            return;
        }

        if (CurrencyManager.instance.CanAfford(template.cost) && GameManager.instance.CanSpawnUnit())
        {
            CurrencyManager.instance.SpendGold(template.cost);
            GameObject unit = Instantiate(template.prefab, spawnPoint.position, Quaternion.identity);


            nextAvailableTimes[index] = currentTime + template.cooldown;
        }
        else
        {
            Debug.Log("Nie staæ lub limit jednostek");
        }


    }

    public float GetCooldownRemaming(int index)
    {
        return Mathf.Max(0, nextAvailableTimes[index] - Time.time);
    }

    public float GetUnitCooldown(int index)
    {
        return units[index].cooldown;
    }

    public int GetUnitCost(int index)
    {
        return units[index].cost;
    }

    public string GetUnitName(int index)
    {
        return units[index].unitName;
    }
}
