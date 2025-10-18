using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class RunSaveDTO
{
    public int version = 1;

    public int seed;
    public int currentNodeId;
    public string scene;

    public List<string> selectedUnitIds = new();
    public List<ModStackDTO> modifiers = new();

    public int cash;
    public int materialsBanked;

    public int fightsWon, fightsLost;
    public float timeInFights, totalDamageDealt, totalDamageTaken;
    public int unitsSpawned, enemiesKilled, goldSpent;

    public int fightIndex = -1;
    public int pendingNodeId = -1;
}

[System.Serializable]
public class ModStackDTO
{
    public string id;
    public int stacks;
}

public static class RunSaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "current_run.json");

    public static bool Exists() => File.Exists(SavePath);
    public static void Delete()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }

    public static void Save()
    {
        var dto = BuildDTO();
        var json = JsonUtility.ToJson(dto, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[RunSaveManager] Saved run to {SavePath}");
    }

    public static bool TryLoad(out RunSaveDTO dto)
    {
        dto = null;
        if (!File.Exists(SavePath)) return false;
        try
        {
            var json = File.ReadAllText(SavePath);
            dto = JsonUtility.FromJson<RunSaveDTO>(json);
            return dto != null;
        }
        catch { return false; }
    }

    public static RunSaveDTO BuildDTO()
    {
        var dto = new RunSaveDTO();
        dto.seed = MapRunData.currentSeed;
        dto.currentNodeId = MapRunData.currentNode.id;
        dto.scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        dto.selectedUnitIds = RunData.GetSelectedUnitIds();
        foreach (var m in RunData.GetActiveModifiers())
            dto.modifiers.Add(new ModStackDTO { id = m.id.ToString(), stacks = m.stacks });

        dto.cash = RunResources.GetCash();
        dto.materialsBanked = RunStatsCollector.S.materialsEarned;

        var s = RunStatsCollector.S;
        dto.fightsWon = s.fightsWon;
        dto.fightsLost = s.fightsLost;
        dto.timeInFights = s.timeInFights;
        dto.totalDamageDealt = s.totalDamageDealt;
        dto.totalDamageTaken = s.totalDamageTaken;
        dto.unitsSpawned = s.unitsSpawned;
        dto.enemiesKilled = s.enemiesKilled;
        dto.goldSpent = s.goldSpent;

        dto.fightIndex = MapRunData.currentNode.fightIndex;
        dto.pendingNodeId = MapRunData.pendingNodeId;
        return dto;
    }

    public static void ApplyDTO(RunSaveDTO dto)
    {
        MapRunData.currentSeed = dto.seed;
        MapRunData.currentNode = MapManager.FindNodeById(dto.currentNodeId);
        MapRunData.pendingNodeId = dto.pendingNodeId;

        RunData.ClearSelectedUnits();
        foreach (var name in dto.selectedUnitIds)
            RunData.AddSelectedUnitByName(name);

        RunData.ClearModifiers();
        foreach (var m in dto.modifiers)
        {
            if (System.Enum.TryParse<RunModifierId>(m.id, out var enumId))
                RunData.StaticAddModifier(enumId, m.stacks);
            else
                Debug.LogWarning($"[RunData] Unknown modifier id '{m.id}'");
        }
            

        RunResources.SetCash(dto.cash);

        var s = RunStatsCollector.S;
        s.materialsEarned = dto.materialsBanked;
        s.fightsWon = dto.fightsWon;
        s.fightsLost = dto.fightsLost;
        s.timeInFights = dto.timeInFights;
        s.totalDamageDealt = dto.totalDamageDealt;
        s.totalDamageTaken = dto.totalDamageTaken;
        s.unitsSpawned = dto.unitsSpawned;
        s.enemiesKilled = dto.enemiesKilled;
        s.goldSpent = dto.goldSpent;
    }

    public static string GetSavePath() => SavePath;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Clear Run Save")]
    private static void EditorClearRunSave()
    {
        Delete();
        Debug.LogWarning($"[RunSaveManager] Save removed: {SavePath}");
    }
#endif

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Reset All (Prefs + Run Save)")]
    private static void EditorResetAll()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        Delete();
        Debug.LogWarning("[RunSaveManager] Cleared PlayerPrefs and run save.");
    }
#endif
}

