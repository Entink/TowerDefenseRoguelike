using UnityEngine;


public static class RewardCalculator
{
    public struct RewardResult
    {
        public int cash;
        public int materials;
    }

    public static RewardResult Compute(bool isBoss, float difficulty, bool won, RewardConfig cfg)
    {
        var d = difficulty <= 0f ? 1f : difficulty;
        var baseCash = cfg.normalCash;
        var baseMat = cfg.normalMaterials;

        var cash = baseCash * (cfg.difficultyMultiplier <= 0f ? 1f : cfg.difficultyMultiplier) * d;
        var mat = baseMat * (cfg.difficultyMultiplier <= 0f ? 1f : cfg.difficultyMultiplier) * d;

        if(isBoss)
        {
            cash = 0f;
            mat *= cfg.bossMaterialsMul <= 0f ? 1f : cfg.bossMaterialsMul;

        }

        if(won)
        {
            cash *= cfg.winCashMul <= 0f ? 1f : cfg.winCashMul;
            mat *= cfg.winMaterialsMul <= 0f ? 1f : cfg.winMaterialsMul;
            return new RewardResult { cash = (int)cash, materials = (int)mat };
        }

        return new RewardResult { cash = 0, materials = 0 };
    }

    public static int ComputeDefeatMaterials(bool isBoss, RewardConfig cfg)
    {
        var baseVal = cfg.defeatBaseMaterials;
        var mul = cfg.defeatBaseMaterials <= 0f ? 0f : cfg.defeatMaterialsMul;
        return (int)(baseVal * mul);
    }
}