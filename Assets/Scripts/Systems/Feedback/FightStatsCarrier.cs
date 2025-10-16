using UnityEngine;

[System.Serializable]
public class CombatStatsSnapshot
{
    public float fightTime;
    public int unitsSpawned;
    public int enemiesKilled;
    public int playerUnitsKilled;
    public int goldSpent;
    public float damageDealt;
    public float damageTaken;
}

public static class FightStatsCarrier
{
    public static CombatStatsSnapshot Last;
    public static void Set(CombatStatsSnapshot snap) => Last = snap;
    public static void Clear() => Last = null;
}