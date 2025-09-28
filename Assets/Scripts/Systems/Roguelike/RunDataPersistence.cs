using System.Collections.Generic;
using UnityEngine;

public static class RunDataPersistence
{
    const string ModsKey = "RunModifiersJson";

    [System.Serializable]
    class ModsWrapper { public List<RunModifierState> list; }

    public static void SaveActiveModifiers(List<RunModifierState> mods)
    {
        var w = new ModsWrapper { list = mods ?? new List<RunModifierState>() };
        var json = JsonUtility.ToJson(w);
        PlayerPrefs.SetString(ModsKey, json);
        PlayerPrefs.Save();
    }

    public static List<RunModifierState> LoadActiveModifiers()
    {
        if (!PlayerPrefs.HasKey(ModsKey)) return new List<RunModifierState>();
        var json = PlayerPrefs.GetString(ModsKey);
        var w = JsonUtility.FromJson<ModsWrapper>(json);
        return w?.list ?? new List<RunModifierState>();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(ModsKey);
        PlayerPrefs.Save();
    }
}
