using UnityEngine;

public static class UnitStatsModifiers
{
    public static float hpMultiplier = 1f;
    public static float damageMultiplier = 1f;
    public static float discountMultiplier = 1f;
    public static float cooldownMultiplier = 1f;

    public static void Reset()
    {
        hpMultiplier = 1f;
        damageMultiplier = 1f;
        discountMultiplier = 1f;
        cooldownMultiplier = 1f;
    }
}