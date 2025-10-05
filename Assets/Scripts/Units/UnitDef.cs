using UnityEngine;

public enum UnitUnlockMethod
{
    Materials,
    EventOnly,
    Special
}

[CreateAssetMenu(menuName = "Units/Unit Definition", fileName = "Unit_")]
public class UnitDef : ScriptableObject
{
    public UnitId id;
    public string displayName;
    public GameObject unitPrefab;

    [Header("Unlocking")]
    public bool unlockedByDefault = false;

    public UnitUnlockMethod unlockMethod = UnitUnlockMethod.Materials;
    public int costMaterials = 0;
    public UnitId[] requiresUnlocked;
}