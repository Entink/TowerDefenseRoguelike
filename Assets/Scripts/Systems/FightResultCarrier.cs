public static class FightResultCarrier
{
    public static bool fightWon = true;
    public static bool recoverableDefeat = false;

    public static float cashMultiplier = 1f;
    public static float materialsMultiplier = 1f;

    public static string resultTitle = "VICTORY";
    public static string integrityText = "";

    public static void SetVictory()
    {
        fightWon = true;
        recoverableDefeat = false;
        cashMultiplier = 1f;
        materialsMultiplier = 1f;
        resultTitle = "VICTORY";
        integrityText = "";
    }

    public static void SetRecoverableDefeat(string newIntegrityText)
    {
        fightWon = false;
        recoverableDefeat = true;

        cashMultiplier = 0.5f;
        materialsMultiplier = 0.5f;

        resultTitle = "DEFEAT";
        integrityText = newIntegrityText;
    }

    public static void Clear()
    {
        SetVictory();
    }
}