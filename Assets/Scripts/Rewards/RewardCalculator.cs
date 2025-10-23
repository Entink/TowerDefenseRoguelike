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
        float diffMul = 0.5f + 0.5f * Mathf.Clamp01(difficulty);
        float diffCfg = (cfg.difficultyMultiplier <= 0f ? 1f : cfg.difficultyMultiplier);

        float baseCash = cfg.normalCash;
        float baseMat = cfg.normalMaterials;

        float cash = baseCash * diffCfg * diffMul;
        float mat = baseMat * diffCfg * diffMul;

        if(isBoss)
        {
            cash = 0f;
            float bossMul = (cfg.bossMaterialsMul <= 0f ? 1f : cfg.bossMaterialsMul);
            mat *= bossMul;
        }

        if(!won)
        {
            return new RewardResult { cash = 0, materials = 0 };
        }

        float winCashMul = (cfg.winCashMul <= 0f ? 1f : cfg.winCashMul);
        float winMatMul = (cfg.winMaterialsMul <= 0f ? 1f : cfg.winMaterialsMul);

        cash *= winCashMul;
        mat *= winMatMul;

        return new RewardResult { cash = Mathf.FloorToInt(cash), materials = Mathf.FloorToInt(mat) };
    }

    public static int ComputeDefeatMaterials(bool isBoss, RewardConfig cfg)
    {
        var baseVal = cfg.defeatBaseMaterials;
        var mul = cfg.defeatBaseMaterials <= 0f ? 0f : cfg.defeatMaterialsMul;
        return (int)(baseVal * mul);
    }
}