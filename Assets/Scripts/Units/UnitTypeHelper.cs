using UnityEngine;
using System.Collections.Generic;

public class UnitTypeHelper : MonoBehaviour
{
    private static Dictionary<UnitType, Dictionary<UnitType, float>> typeBonuses = new()
    {
        {
            UnitType.Biological, new Dictionary<UnitType, float>
            {
                { UnitType.Ethereal, 0.25f },
                { UnitType.Mechanical, 0.75f },
                { UnitType.Electrical, 1.25f }
            }

        },
        {
            UnitType.Electrical, new Dictionary<UnitType, float>
            {
                { UnitType.Biological, 0.7f },
                { UnitType.Mechanical, 1.25f },
                { UnitType.Ethereal, 1.75f }
            }
        },
        {
            UnitType.Ethereal, new Dictionary<UnitType, float>
            {
                { UnitType.Biological, 1.1f },
                { UnitType.Ethereal, 2f },
                { UnitType.Mechanical, 1.1f }
            }
        },
        {
            UnitType.Mechanical, new Dictionary<UnitType, float>
            {
                { UnitType.Biological, 1.25f },
                { UnitType.Ethereal, 0f }
            }
        }
    };

    public static float GetBonusMultiplier(UnitType attacker, UnitType target)
    {
        if (typeBonuses.TryGetValue(attacker, out var targetDict))
        {
            if(targetDict.TryGetValue(target, out var multiplier))
            {
                return multiplier;
            }
        }
        return 1f;
    }
}
