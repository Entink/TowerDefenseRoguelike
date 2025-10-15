using UnityEngine;
using System;

[Serializable]
public class FightEnemySpawn
{
    public GameObject enemyPrefab;
    public float delay;

    [Min(1)]
    public int quantity = 1;

    [Min(0f)]
    public float quantityInterval = 0.5f;

    public bool isInfinite = false;
    public float repeatInterval = 5f;
}

[CreateAssetMenu(fileName = "NewFightData", menuName = "Combat/Fight Data")]
public class FightData : ScriptableObject
{
    public string fightName;
    public bool isBoss;

    [Header("Tutorial")]
    public bool isTutorialOnly = false;

    [Header("Level settings")]
    [Min(5)]
    public float levelLenght = 20f;

    [Header("Enemies to spawn")]
    public FightEnemySpawn[] enemies;

    [Header("Difficulty")]
    public int difficulty = 1;
}
