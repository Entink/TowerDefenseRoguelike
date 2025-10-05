using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Units/Database",fileName ="UnitDatabase")]
public class UnitDatabase : ScriptableObject
{
    public List<UnitDef> units = new();

    public UnitDef Get(UnitId id) => units.FirstOrDefault(u => u && u.id == id);
    public IEnumerable<UnitDef> All => units.Where(u => u);
}