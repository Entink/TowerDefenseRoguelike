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
    

    public static void Load(UnitDatabase db)
    {
        
        //_unlocked.Clear();

        if(PlayerPrefs.HasKey(KEY))
        {
            var json = PlayerPrefs.GetString(KEY);
            var data = JsonUtility.FromJson<UnitUnlockSave>(json) ?? new UnitUnlockSave();
            foreach (var id in data.unlocked) _unlocked.Add((UnitId)id);


            
        }

        if(db != null)
        {
            foreach (var def in db.All)
                if (def.unlockedByDefault) _unlocked.Add(def.id);
        }

        
    }

    public static void Save()
    {
        var data = new UnitUnlockSave();
        foreach (var id in _unlocked) data.unlocked.Add((int)id);
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static bool IsUnlocked(UnitId id) => _unlocked.Contains(id);

   

    public static bool TryUnlockWithMaterials(UnitDef def)
    {
        if (def == null) return false;
        if (IsUnlocked(def.id)) return true;

        if (def.unlockMethod != UnitUnlockMethod.Materials)
            return false;

        if (!HasRequirements(def))
            return false;

        int cost = Mathf.Max(0, def.costMaterials);
        if (RunResources.GetMaterials() < cost)
            return false;

        if (cost > 0) RunResources.AddMaterials(-cost);

        _unlocked.Add(def.id);
        Save();
        PlayerPrefs.Save();
        return true;
    }

    public static bool HasRequirements(UnitDef def)
    {
        if (def.requiresUnlocked == null || def.requiresUnlocked.Length == 0) return true;
        foreach(var req in def.requiresUnlocked)
        {
            if (!IsUnlocked(req)) return false;
        }
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