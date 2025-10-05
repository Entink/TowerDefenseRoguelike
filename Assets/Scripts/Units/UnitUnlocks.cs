using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class UnitUnlockSave
{
    public List<int> unlocked = new();
}

public static class UnitUnlocks
{
    private const string KEY = "UNIT_UNLOCKS_V1";
    private static HashSet<UnitId> _unlocked = new();
    private static bool _loaded = false;

    public static void Load(UnitDatabase db)
    {
        if (_loaded) return;
        _unlocked.Clear();

        if(PlayerPrefs.HasKey("KEY"))
        {
            var json = PlayerPrefs.GetString(KEY);
            try
            {
                var data = JsonUtility.FromJson<UnitUnlockSave>(json) ?? new UnitUnlockSave();
                foreach (var id in data.unlocked) _unlocked.Add((UnitId)id);
            }
            catch { }


        }

        if(db != null)
        {
            foreach (var def in db.All)
                if (def.unlockedByDefault) _unlocked.Add(def.id);
        }

        _loaded = true;
    }

    public static void Save()
    {
        var data = new UnitUnlockSave();
        foreach (var id in _unlocked) data.unlocked.Add((int)id);
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static bool IsUnlocked(UnitId id) => _unlocked.Contains(id);

    public static bool CanUnlockWithMaterials(UnitDef def)
    {
        if (def == null) return false;
        if (IsUnlocked(def.id)) return false;
        if (def.unlockMethod != UnitUnlockMethod.Materials) return false;
        if (def.requiresUnlocked != null)
            foreach (var req in def.requiresUnlocked)
                if (!IsUnlocked(req)) return false;

        return RunResources.GetMaterials() >= def.costMaterials;
    }

    public static bool TryUnlockWithMaterials(UnitDef def)
    {
        if (!CanUnlockWithMaterials(def)) return false;
        if (def.costMaterials > 0) RunResources.AddMaterials(-def.costMaterials);
        _unlocked.Add(def.id);
        Save();
        return true;
    }

    public static bool UnlockByEvent(UnitId id)
    {
        if (IsUnlocked(id)) return false;
        _unlocked.Add(id);
        Save();
        return true;
    }

    public static void ForceUnlock(UnitId id)
    {
        _unlocked.Add(id);
        Save();
    }
}