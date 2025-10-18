using UnityEngine;

public static class Units
{
    private static UnitDatabase _db;
    public static UnitDatabase DB
    {
        get
        {
            if (_db == null)
                _db = Resources.Load<UnitDatabase>("UnitDatabase");
            return _db;
        }
    }
}
