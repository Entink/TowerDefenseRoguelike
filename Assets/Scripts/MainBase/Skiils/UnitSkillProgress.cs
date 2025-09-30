using System.Collections.Generic;
using UnityEngine;


public static class UnitSkillProgress
{
    const string Key = "UnitSkillProgressJson";

    [System.Serializable] class NodeLevel { public string nodeId; public int level; }
    [System.Serializable] class UnitEntry { public UnitId unitId; public List<NodeLevel> nodes = new(); }
    [System.Serializable] class SaveData { public List<UnitEntry> units = new(); }

    static SaveData cache;

    static void Load()
    {
        if (cache != null) return;
        if(!PlayerPrefs.HasKey(Key)) { cache = new SaveData(); return; }
        cache = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key)) ?? new SaveData();
    }
    
    static void Save()
    {
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(cache));
        PlayerPrefs.Save();
    }

    public static int GetLevel(UnitId unit, string nodeId)
    {
        Load();
        var u = cache.units.Find(x => x.unitId == unit);
        var n = u?.nodes.Find(x => x.nodeId == nodeId);
        return n?.level ?? 0;
    }

    public static bool TryBuy(UnitId unit, SkillNode node, System.Func<int> getMaterials, System.Action<int> addMaterials)
    {
        Load();

        if (node.prerequisites != null)
            foreach (var pre in node.prerequisites)
                if (GetLevel(unit, pre) < 1) return false;

        int cur = GetLevel(unit, node.nodeId);
        if (cur >= node.maxLevel) return false;

        if (getMaterials() < node.costMaterials) return false;

        addMaterials(-node.costMaterials);

        var u = cache.units.Find(x => x.unitId == unit);
        if (u == null) { u = new UnitEntry { unitId = unit }; cache.units.Add(u); }
        var n = u.nodes.Find(x => x.nodeId == node.nodeId);
        if(n == null) { n = new NodeLevel { nodeId = node.nodeId, level = 0 }; u.nodes.Add(n); }
        n.level = Mathf.Clamp(cur + 1, 0, node.maxLevel);
        Save();
        return true;


    }
}