using UnityEngine;
using System.Linq;

public class UnitSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableUnit
    {
        public GameObject prefab;
        public KeyCode hotkey;
    }

    public Transform spawnPoint;
    public SpawnableUnit[] units;

    private float[] nextAvailableTimes;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            BaseController[] bases = FindObjectsOfType<BaseController>();
            foreach (var b in bases)
            {
                if(b.isPlayerBase)
                {
                    spawnPoint = b.transform;
                    break;
                }
            }

            if(spawnPoint == null)
            {
                Debug.LogWarning("Nie znaleziono bazy gracza.");
            }
        }
    }

    private void Start()
    {
        if(RunData.selectedUnits != null && RunData.selectedUnits.Count > 0)
        {
            units = RunData.selectedUnits.Select(u => new SpawnableUnit { prefab = u }).ToArray();
        }
        nextAvailableTimes = new float[units.Length];
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < units.Length; i++)
        {
            if(Input.GetKeyDown(units[i].hotkey))
            {
                TrySpawnUnit(i);
            }
        }
    }

    public void TrySpawnUnit(int index)
    {
        if (index < 0 || index >= units.Length)
        {
            return;
        }

        UnitStats stats = units[index].prefab.GetComponent<UnitStats>();
        if(stats == null)
        {
            return;
        }

        foreach (var item in RunData.purchasedItems)
        {
            item.effect?.ApplyEffect(stats);
        }

        float currentTime = Time.time;
        if(currentTime < nextAvailableTimes[index])
        {
            Debug.Log("Jednostka jest na cooldownie");
            return;
        }

        if (CurrencyManager.instance.CanAfford(stats.cost) && GameManager.instance.CanSpawnUnit())
        {
            CurrencyManager.instance.SpendGold(stats.cost);
            GameObject unit = Instantiate(units[index].prefab, spawnPoint.position, Quaternion.identity);
            

            nextAvailableTimes[index] = currentTime + stats.cooldown;
        }
        else
        {
            Debug.Log("Nie sta� lub limit jednostek");
        }

        
    }

    public float GetCooldownRemaming(int index)
    {
        return Mathf.Max(0, nextAvailableTimes[index] - Time.time);
    }

    public float GetUnitCooldown(int index)
    {
        return units[index].prefab.GetComponent<UnitStats>().cooldown;
    }

    public int GetUnitCost(int index)
    {
        return units[index].prefab.GetComponent<UnitStats>().cost;
    }

    public string GetUnitName(int index)
    {
        return units[index].prefab.name;
    }
}
