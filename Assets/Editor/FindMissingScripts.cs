using UnityEditor;
using UnityEngine;

public static class FindMissingScripts
{
    [MenuItem("Tools/Diagnostics/Find Missing Scripts In Scene")]
    public static void FindInScene()
    {
        int n = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var comps = go.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
                if (comps[i] == null) { Debug.LogError($"[Missing Script] {GetPath(go)} (idx {i})", go); n++; }
        }
        Debug.Log($"Missing scripts in scene: {n}");
    }

    private static string GetPath(GameObject obj)
    {
        string p = obj.name; var t = obj.transform;
        while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
        return p;
    }
}
