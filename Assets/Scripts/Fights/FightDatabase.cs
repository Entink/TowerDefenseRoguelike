using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FightDatabase : MonoBehaviour
{
    public static FightDatabase instance;

    [Header("List of available fights")]
    public List<FightData> allFights;
    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public FightData GetRandomFight(int difficulty)
    {
        List<FightData> candidates = allFights.Where(f => f.difficulty == difficulty && !f.isBoss).ToList();

        if(candidates.Count == 0)
        {
            Debug.LogWarning($"Brak walk o trudnoœci {difficulty}");
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public FightData GetRandomBossFight()
    {
        List<FightData> bosses = allFights.Where(f => f.isBoss).ToList();

        if (bosses.Count == 0)
        {
            Debug.LogWarning("Brak walk typu boss");
            return null;
        }

        return bosses[Random.Range(0, bosses.Count)];
    }

    public FightData GetByTypeAndIndex(NodeType type, int index)
    {
        var list = (type == NodeType.Boss)
            ? allFights.Where(f => f != null && f.isBoss).ToList()
            : allFights.Where(f => f != null && !f.isBoss).ToList();

        if (list.Count == 0) return null;

        int i = Mathf.Abs(index) % list.Count;
        return list[i];
    }
}
