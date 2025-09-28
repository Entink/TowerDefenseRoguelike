using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Modifiers/Modifier Database", fileName = "RunModifierDatabase")]
public class RunModifierDatabase : ScriptableObject
{
    [SerializeField] private List<RunModifierDef> all = new List<RunModifierDef>();
    private Dictionary<RunModifierId, RunModifierDef> byId;

    private void OnEnable()
    {
        RebuildIndex();
    }


    public void RebuildIndex()
    {
        byId = new Dictionary<RunModifierId, RunModifierDef>();
        foreach (var def in all)
        {
            if (def == null) continue;
            if (!byId.ContainsKey(def.id))
                byId.Add(def.id, def);
            else
                Debug.LogWarning($"Duplicate RunModifierId in database: {def.id}");
        }
    }

    public RunModifierDef Get(RunModifierId id)
    {
        if (byId == null || byId.Count == 0) RebuildIndex();
        return byId != null && byId.TryGetValue(id, out var def) ? def : null;
    }

    public IReadOnlyList<RunModifierDef> All => all;

    public bool Contains(RunModifierId id) => Get(id) != null;
    
}