using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class UnitEffectiveStatsHUD : MonoBehaviour
{
    public static bool GlobalEnabled = true;

    public TextMeshProUGUI label;
    public float refreshInterval = 0.25f;
    public Vector3 worldOffset = new Vector3(0, 1.6f, 0);

    UnitStats stats;
    StatusController sc;
    UnitController uc;
    float t;

    private void Awake()
    {
        stats = GetComponentInParent<UnitStats>();
        sc = GetComponentInParent<StatusController>();
        uc = GetComponentInParent<UnitController>();
        if (label != null) label.enabled = GlobalEnabled;
        if (sc != null) sc.EffectsChanged += ForceRefresh;
    }

    private void OnEnable()
    {
        ForceRefresh();
    }

    private void OnDisable()
    {
        if (sc != null) sc.EffectsChanged -= ForceRefresh;
    }

    private void Update()
    {
        if(!GlobalEnabled || label == null)
        {
            if (label != null) label.enabled = false;
            return;
        }
        label.enabled = true;
        //Transform target = (uc != null) ? uc.transform : transform.parent;
        //if(target != null)
        //{
        //    var cam = Camera.main;
        //    if(cam != null)
        //    {
        //        var screen = cam.WorldToScreenPoint(target.position);
        //        label.transform.position = screen;

        //    }
        //}

        t += Time.deltaTime;
        if(t >= refreshInterval)
        {
            t = 0f;
            RefreshText();
        }
    }
    void ForceRefresh()
    {
        t = 0f;
        RefreshText();
    }

    void RefreshText()
    {
        if (label == null || stats == null) return;

        float dmgMul = sc != null ? sc.GetDamageMul() : 1f;
        float asMul = sc != null ? sc.GetAttackSpeedMul() : 1f;
        float mvMul = sc != null ? sc.GetMoveMul() : 1f;
        float rngAdd = sc != null ? sc.GetRangeAdd() : 0f;
        float kbRes = sc != null ? sc.GetKBResMul() : 1f;
        float armorA = sc != null ? sc.GetArmorAdd() : 0f;
        float lsAdd = sc != null ? sc.GetLifeStealAdd() : 0f;
        float shield = sc != null ? sc.GetShield() : 0f;

        float effDmg = stats.damage * dmgMul;
        float effAS = Mathf.Max(0.01f, stats.attackSpeed * asMul);
        float effRng = Mathf.Max(0f, stats.attackRange + rngAdd);
        float effLS = (uc != null ? uc.lifeSteal : 0f) + lsAdd;

        label.text =
            $"DMG {effDmg:0.##}  AS {effAS:0.##}\n" +
            $"RNG {effRng:0.##}  MV {mvMul:0.##}x\n" +
            $"ARM {armorA:0.##}  KBres {kbRes:0.##}x\n" +
            $"LS {effLS:0.##}  SH {shield:0.#}";
    }
}
