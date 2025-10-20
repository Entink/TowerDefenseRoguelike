using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class AuraEffectSource : MonoBehaviour
{
    public float radius = 3f;
    public float tickInterval = 1f;
    public bool affectSelf = false;

    public List<StatusEffectAsset> allyEffects = new();
    public List<StatusEffectAsset> enemyEffects = new();

    [Header("Debug")]
    public bool debugLogs = false;

    UnitController ownerUC;
    int allySign;
    int enemySign;
    Coroutine loop;

    private void Awake()
    {
        ownerUC = GetComponentInParent<UnitController>();
        if(ownerUC != null)
        {
            allySign = ownerUC.IsAlly ? 1 : -1;
            enemySign = ownerUC.IsAlly ? -1 : 1;
        }
        else
        {
            allySign = 1;
            enemySign = -1;
        }
    }

    private void OnEnable()
    {
        if (loop == null) loop = StartCoroutine(Ticker());
    }

    private void OnDisable()
    {
        if(loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    IEnumerator Ticker()
    {
        var wait = new WaitForSeconds(Mathf.Max(0.05f, tickInterval));
        while(true)
        {
            TickOnce();
            yield return wait;
        }
    }

    void TickOnce()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, Mathf.Max(0.01f, radius));
        if (hits == null || hits.Length == 0) return;

        for(int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (h == null) continue;

            var uc = h.GetComponent<UnitController>();
            if (uc == null) continue;

            if (!affectSelf && ownerUC != null && uc == ownerUC) continue;

            int sign = uc.IsAlly ? 1 : -1;
            var sc = uc.GetComponent<StatusController>();
            if (sc == null) continue;

            if(sign == allySign)
            {
                for(int e = 0; e < allyEffects.Count; e++)
                {
                    var asset = allyEffects[e];
                    if (asset == null) continue;
                    var eff = asset.Create();
                    sc.Apply(eff);
                }
                if (debugLogs) Debug.Log($"[Aura] {name} -> ally {uc.name} ({allyEffects.Count} effects)"); 
            }
            else if (sign == enemySign)
            {
                for(int e = 0; e <enemyEffects.Count; e++)
                {
                    var asset = enemyEffects[e];
                    if (asset == null) continue;
                    var eff = asset.Create();
                    sc.Apply(eff);

                }
                if (debugLogs) Debug.Log($"[Aura] {name} -> enemy {uc.name} ({enemyEffects.Count} effects)");

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.01f, radius));
    }
}
