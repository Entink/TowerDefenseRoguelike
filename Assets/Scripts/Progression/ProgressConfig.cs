using UnityEngine;

[CreateAssetMenu(menuName ="Progression Config")]
public class ProgressConfig : ScriptableObject
{
    [Header("Difficulty")]
    public int minDifficulty = 1;
    public int maxDifficulty = 10;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public int variance = 1;

    [Header("Acts scaling")]
    public float actDifficultyMul = 1.0f;
}
