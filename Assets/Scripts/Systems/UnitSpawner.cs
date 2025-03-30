using UnityEngine;

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
            //GameManager.instance.RegisterUnit();

            nextAvailableTimes[index] = currentTime + stats.cooldown;
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
        return units[index].prefab.GetComponent<UnitStats>().cooldown;
    }

    public int GetUnitCost(int index)
    {
        return units[index].prefab.GetComponent<UnitStats>().cost;
    }
}
