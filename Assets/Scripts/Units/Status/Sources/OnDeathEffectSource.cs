using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class OnDeathEffectSource : MonoBehaviour
{
    public float radius = 3f;
    public List<StatusEffectAsset> allyEffects = new();
    public List<StatusEffectAsset> enemyEffects = new();

    UnitController owner;

    private void Awake()
    {
        owner = GetComponent<UnitController>();
    }

    public void Trigger()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, Mathf.Max(0.01f, radius));
        if (hits == null) return;

        int mySign = (owner != null && owner.IsAlly) ? 1 : -1;
        for(int i = 0; i < hits.Length; i++)
        {
            var uc = hits[i].GetComponent<UnitController>();
            if (uc == null) continue;
            var sc = uc.GetComponent<StatusController>();
            if (sc == null) continue;

            int hisSign = uc.IsAlly ? 1 : -1;
            var list = (hisSign == mySign) ? allyEffects : enemyEffects;
            for(int e = 0; e<list.Count; e++)
            {
                var a = list[e];
                if (a == null) continue;
                sc.Apply(a.Create());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.01f, radius));
    }
}
