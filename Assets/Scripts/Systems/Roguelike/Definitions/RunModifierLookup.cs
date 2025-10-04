using UnityEngine;

public static class RunModifierLookup
{
    static RunModifierDatabase _db;
    public static void SetDB(RunModifierDatabase db) => _db = db;

    public static RunModifierDatabase DB
    {
        get
        {
            if(_db == null)
            {
                _db = Resources.Load<RunModifierDatabase>("RunModifierDatabase");
                if(_db == null)
                {
                    Debug.LogError("RunModifierDatabase not found.");
                }
            }
            return _db;
        }
    }

    public static RunModifierDef Def(RunModifierId id)
        => DB != null ? DB.GetById(id) : null;
}