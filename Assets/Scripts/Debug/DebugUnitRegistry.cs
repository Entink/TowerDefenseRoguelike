using UnityEngine;
using System;
using System.Collections.Generic;

public class DebugUnitRegistry : MonoBehaviour
{
    public static DebugUnitRegistry I { get; private set; }

    [Serializable]
    public class Entry
    {
        public string id;
        public GameObject prefab;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<string, GameObject> prefabById;

    private void Awake()
    {
        if(I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        BuildLookup();
       
    }

    private void BuildLookup()
    {
        prefabById = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < entries.Count; i++)
        {
            Entry entry = entries[i];

            if (entry == null)
                continue;

            if (string.IsNullOrWhiteSpace(entry.id))
                continue;

            if (entry.prefab == null)
                continue;

            prefabById[entry.id.Trim()] = entry.prefab;
        }
    }

    public GameObject GetPrefab(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (prefabById == null)
            BuildLookup();

        prefabById.TryGetValue(id.Trim(), out GameObject prefab);
        return prefab;
    }


    public string GetAllIds()
    {
        if (prefabById == null)
            BuildLookup();

        if (prefabById.Count == 0)
            return "none";

        return string.Join(", ", prefabById.Keys);
    }

}
