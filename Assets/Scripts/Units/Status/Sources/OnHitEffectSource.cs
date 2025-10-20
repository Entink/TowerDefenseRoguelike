using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class OnHitEffectSource : MonoBehaviour
{
    public List<StatusEffectAsset> Effects = new();

    UnitController ownerUC;
    int enemySign;

    private void Awake()
    {
        ownerUC = GetComponent<UnitController>();
        if(ownerUC != null)
        {
            enemySign = ownerUC.IsAlly ? -1 : 1;
        }
    }

    public void ApplyTo(UnitController target)
    {
        if (target == null) return;
        int sign = target.IsAlly ? 1 : -1;
        var sc = target.GetComponent<StatusController>();
        if (sc == null) return;

        if (sign == enemySign)
        {
            for(int i = 0; i < Effects.Count; i++)
            {
                var a = Effects[i];
                if (a == null) continue;
                sc.Apply(a.Create());
            }
        }
    }
}
