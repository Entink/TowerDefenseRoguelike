using UnityEngine;

[CreateAssetMenu(menuName = "Config/Reward Config",fileName ="RewardConfing")]
public class RewardConfig : ScriptableObject
{
    [Header("Base")]
    public int normalCash = 12;
    public int normalMaterials = 1;


    [Header("Multipliers")]
    public float difficultyMultiplier = 1f;
    public float winCashMul = 1f;
    public float winMaterialsMul = 1;
    public float bossMaterialsMul = 3f;

    [Header("Defeat")]
    public int defeatBaseMaterials = 1;
    public float defeatMaterialsMul = 0.7f;
}
