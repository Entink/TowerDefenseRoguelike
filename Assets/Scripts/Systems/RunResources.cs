using UnityEngine;

public static class RunResources
{
    public static int cash = 0;
    public static int materials = 0;

    public static void Reset()
    {
        cash = 0;
        
    }

    public static void FullReset()
    {
        cash = 0;
        materials = 0;
    }

    public static void AddCash(int amount)
    {
        cash += amount;
    }

    public static void AddMaterials(int amount)
    {
        materials += amount;
    }

    public static int GetCash() => cash;
    public static int GetMaterials() => materials;

}
