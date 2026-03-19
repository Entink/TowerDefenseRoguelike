public static class DebugCombatSession
{
    public static bool StartEmptyTestFight { get; private set; }
    public static bool ReturnToBaseAfterFight { get; private set; }

    public static void RequestEmptyTestFight()
    {
        StartEmptyTestFight = true;
        ReturnToBaseAfterFight = true;
    }

    public static void Clear()
    {
        StartEmptyTestFight = false;
        ReturnToBaseAfterFight = false;
    }

}