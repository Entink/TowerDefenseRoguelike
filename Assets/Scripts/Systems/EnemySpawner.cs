using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private FightData fight;
    public FightData testFight;


    private void Start()
    {
        fight = BattleDataCarrier.selectedFight != null ? BattleDataCarrier.selectedFight : testFight;
        if(fight == null)
        {
            Debug.LogWarning("Brak danych o walce!");
            return;
        }

        StartSpawning();


    }


    private void StartSpawning()
    {
        foreach (var spawn in fight.enemies)
        {
            if (spawn.isInfinite)
            {
                StartCoroutine(SpawnLoop(spawn));
            }
            else
            {
                StartCoroutine(SpawnFixed(spawn));
            }
        }
        
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        //GameManager.instance.RegisterUnit();
    }

    private IEnumerator SpawnFixed(FightEnemySpawn spawn)
    {
        yield return new WaitForSeconds(spawn.delay);

        for (int i = 0; i < spawn.quantity; i++)
        {
            SpawnEnemy(spawn.enemyPrefab);

            if (i < spawn.quantity - 1)
            {
                yield return new WaitForSeconds(spawn.quantityInterval);
            }
        }
    }

    private IEnumerator SpawnLoop(FightEnemySpawn spawn)
    {
        yield return new WaitForSeconds(spawn.delay);

        while(true)
        {
            SpawnEnemy(spawn.enemyPrefab);
            yield return new WaitForSeconds(spawn.repeatInterval);
        }
    }
}
