using UnityEngine;

[System.Serializable]
public class RunStats
{
    public int fightsWon, fightsLost;
    public float timeInFights, totalDamageDealt, totalDamageTaken;
    public int unitsSpawned, enemiesKilled, goldSpent;

    public int materialsEarned;
}

public static class RunStatsCollector
{
    public static RunStats S = new RunStats();

    public static void Reset() => S = new RunStats();

    public static void AddFromFight(CombatStatsTracker c)
    {
        if (c == null) return;
        S.timeInFights += c.fightTime;
        S.totalDamageDealt += c.damageDealt;
        S.totalDamageTaken += c.damageTaken;
        S.unitsSpawned += c.unitsSpawned;
        S.enemiesKilled += c.enemiesKilled;
        S.goldSpent += c.goldSpent;
    }

    public static void OnFightResult(bool win)
    {
        if (win) S.fightsWon++; else S.fightsLost++;
    }

    public static void AddMaterials(int amount)
    {
        S.materialsEarned += Mathf.Max(0, amount);
    }
}
