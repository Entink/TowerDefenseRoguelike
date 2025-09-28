using UnityEngine;

public static class RunModifierLookup
{
    static RunModifierDatabase _db;
    public static RunModifierDatabase DB
    {
        get
        {
            if (_db == null)
            {
                _db = Resources.Load<RunModifierDatabase>("RunModifierDatabase");
                if (_db == null)
                    Debug.LogError("RunModifierDatabase cannot be found in Resources. Please, place it in Resources/RunModifierDatabase.asset or assign using Inspector in UI code.");
            }
            return _db;
        }
    }

    public static RunModifierDef Def(RunModifierId id) => DB != null ? DB.Get(id) : null;
}