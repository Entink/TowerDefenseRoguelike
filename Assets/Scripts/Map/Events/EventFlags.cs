using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventFlagSave
{
    public List<string> resolved = new();
}

public static class EventFlags
{
    private const string KEY = "EVENT_FLAGS_V1";
    private static HashSet<string> _resolved = new();
    private static bool _loaded;

    public static void Load()
    {
        if (_loaded) return;
        _resolved.Clear();
        if(PlayerPrefs.HasKey(KEY))
        {
            var json = PlayerPrefs.GetString(KEY);
            var data = JsonUtility.FromJson<EventFlagSave>(json) ?? new EventFlagSave();
            foreach (var id in data.resolved) _resolved.Add(id);
        }
        _loaded = true;
    }

    public static void Save()
    {
        var data = new EventFlagSave();
        foreach (var id in _resolved) data.resolved.Add(id);
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static bool IsResolved(string eventId) => !string.IsNullOrEmpty(eventId) && _resolved.Contains(eventId);
    public static void MarkResolved(string eventId)
    {
        if (string.IsNullOrEmpty(eventId)) return;
        _resolved.Add(eventId);
        Save();
    }
}