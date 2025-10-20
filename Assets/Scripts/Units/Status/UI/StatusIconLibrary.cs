using UnityEngine;

[System.Serializable]
public class StatusIconEntry
{
    public string typeName;
    public Sprite icon;
}

[CreateAssetMenu(menuName ="StatusEffects/Icon Library",fileName ="StatusIconLibrary")]
public class StatusIconLibrary : ScriptableObject
{
    public StatusIconEntry[] entries;

    public Sprite Get(string typeName)
    {
        if (entries == null) return null;
        for(int i = 0; i < entries.Length; i++)
        {
            var e = entries[i];
            if (e != null && e.icon != null && e.typeName == typeName) return e.icon;
        }
        return null;
    }
}
