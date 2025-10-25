using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class StatusController : MonoBehaviour
{
    public Action<float> OnRequestDamage;
    public Action<float> OnRequestHeal;
    public System.Action<float> OnRequestDamageSilent;
    public System.Action EffectsChanged;

    readonly List<StatusEffect> effects = new();

    float dmgMul = 1f;
    float atkSpeedMul = 1f;
    float moveMul = 1f;
    float kbForceMul = 1f;
    float kbResMul = 1f;
    float armorFlat = 0f;
    float rangeAdd = 0f;
    float lifeStealAdd = 0f;
    float shieldHP = 0f;
    float regenFlat = 0f;

    int pierceTargetsAdd = 0;
    float pierceSecondaryMulMul = 1f;

    [SerializeField] public bool applyBaselinesFromStats = true;
    bool baselinesApplied;

    public struct StatusIconData { public string typeName; public int stacks; }

    public List<StatusIconData> GetIconData()
    {
        var dict = new System.Collections.Generic.Dictionary<string, int>();
        for(int i = 0; i < effects.Count; i++)
        {
            var eff = effects[i];
            if (eff.uiHidden && !(eff is ShieldEffect)) continue;
            if (eff is ShieldEffect && GetShield() <= 0f) continue;

            var t = effects[i].GetType().Name;
            if (!dict.TryGetValue(t, out var s)) s = 0;
            s += Mathf.Max(1, effects[i].stacks);
            dict[t] = s;
        }
        var list = new System.Collections.Generic.List<StatusIconData>(dict.Count);
        foreach (var kv in dict) list.Add(new StatusIconData { typeName = kv.Key, stacks = kv.Value });
        return list;
    }
    private void Awake()
    {
        if (applyBaselinesFromStats) ApplyBaselinesFromStats();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for(int i = effects.Count - 1; i >= 0; i--)
        {
            var e = effects[i];
            e.Tick(this, dt);
            if(e.IsExpired)
            {
                e.OnExpire(this);
                effects.RemoveAt(i);
                RebuildAggregates();
                EffectsChanged?.Invoke();
            }
        }

        if(regenFlat > 0f)
        {
            OnRequestHeal?.Invoke(regenFlat * Time.deltaTime);
        }
    }

    public void Apply(StatusEffect effect)
    {
        if(effect.IsPermament)
        {
            effects.Add(effect);
            effect.OnApply(this);
            RebuildAggregates();
            EffectsChanged?.Invoke();
            return;
        }

        int sameKeyIndex = -1;
        for(int i = 0; i < effects.Count; i++)
        {
            var e = effects[i];
            if (e.IsPermament) continue;
            if (e.effectKey != effect.effectKey) continue;

            if (e.uiHidden != effect.uiHidden) continue;

            sameKeyIndex = i;
            break;
        }

        if(sameKeyIndex >= 0)
        {
            var current = effects[sameKeyIndex];

            if(current.CanStack(effect) && current.stacks < current.maxStacks)
            {
                current.AddStack();
                current.ResetTimer();
                EffectsChanged?.Invoke();
                return;
            }

            effects[sameKeyIndex] = effect;
            effect.OnApply(this);
            RebuildAggregates();
            EffectsChanged?.Invoke();
            return;
        }

        effects.Add(effect);
        effect.OnApply(this);
        RebuildAggregates();
        EffectsChanged?.Invoke();
    }


    void ApplyBaselinesFromStats()
    {
        if (baselinesApplied) return;
        var us = GetComponent<UnitStats>();
        if (us == null) return;

        if(us.hasBaseArmor && us.baseArmorFlat != 0f)
        {
            var permArmor = new BuffStatModifierEffect { duration = 0f, armorAdd = us.baseArmorFlat, uiHidden = true };
            Apply(permArmor);
        }

        if(us.hasBaseShield && us.baseShieldAmount > 0f)
        {
            var permShield = new ShieldEffect { duration = 0f, shieldAmount = us.baseShieldAmount };
            Apply(permShield);
        }

        baselinesApplied = true;
    }

    public float ModifyIncomingDamage(float dmg)
    {
        float baseDamage = Mathf.Max(0f, dmg);

        float armor = GetArmorAdd();
        float armorSoftCap = 0.6f * baseDamage;
        float effectiveArmor = Mathf.Min(armor, armorSoftCap);
        float afterArmor = Mathf.Max(0f, baseDamage - effectiveArmor);

        float minFloor = Mathf.Max(0f, 0.1f * baseDamage);
        float mitigated = Mathf.Max(afterArmor, minFloor);



        
        if(shieldHP > 0f)
        {
            float absorb = Mathf.Min(shieldHP, mitigated);
            shieldHP -= absorb;
            mitigated -= absorb;
            if (absorb > 0f && shieldHP <= 0f) EffectsChanged?.Invoke();
        }


        return Mathf.Max(0f, mitigated);
    }

    public float GetDamageMul() => dmgMul;
    public float GetAttackSpeedMul() => atkSpeedMul;
    public float GetMoveMul() => moveMul;
    public float GetKBForceMul() => kbForceMul;
    public float GetKBResMul() => kbResMul;
    public float GetRangeAdd() => rangeAdd;
    public float GetLifeStealAdd() => lifeStealAdd;
    public float GetArmorAdd() => armorFlat;

    public void AddShield(float amount) { shieldHP += amount; }
    public void SetShield(float amount) { shieldHP = Mathf.Max(0f, amount); }
    public float GetShield() => shieldHP;

    public float GetRegenFlat() => regenFlat;
    public void AddRegenFlat(float add) { regenFlat += add; }

    public void AddArmor(float amount) { armorFlat += amount; }
    public void AddDamageMul(float mul) { dmgMul *= mul; }
    public void AddAttackSpeedMul(float mul) { atkSpeedMul *= mul; }
    public void AddMoveMul(float mul) { moveMul *= mul; }
    public void AddKBForceMul(float mul) { kbForceMul *= mul; }
    public void AddKBResMul(float mul) { kbResMul *= mul; }
    public void AddRange(float add) { rangeAdd += add; }
    public void AddLifeSteal(float add) { lifeStealAdd += add; }

    public int GetPierceTargetsAdd() => pierceTargetsAdd;
    public float GetPierceSecondaryMulMul() => pierceSecondaryMulMul;
    public void AddPierceTargets(int add) { pierceTargetsAdd += add; }
    public void MulPierceSecondary(float mul) { pierceSecondaryMulMul *= Mathf.Max(0f, mul); }

    public void RebuildAggregates()
    {
        dmgMul = 1f;
        atkSpeedMul = 1f;
        moveMul = 1f;
        kbForceMul = 1f;
        kbResMul = 1f;
        armorFlat = 0f;
        rangeAdd = 0f;
        lifeStealAdd = 0f;
        regenFlat = 0f;
        pierceTargetsAdd = 0;
        pierceSecondaryMulMul = 1f;

        for (int i = 0; i < effects.Count; i++)
            if (effects[i] is IAggregatedModifier a) a.ApplyTo(this);

        var caps = Resources.Load<BalancingCapsConfig>("Balance/BalancingCapsConfig");
        if(caps != null)
        {
            armorFlat = Mathf.Min(armorFlat, caps.armorCap_Act1);
            regenFlat = Mathf.Min(regenFlat, caps.regenCap_Act1);
        }
    }

    public List<StatusEffect> GetActiveEffectsSnapshot()
    {
        var list = new List<StatusEffect>(effects.Count);
        for (int i = 0; i < effects.Count; i++)
            list.Add(effects[i]);
        return list;
    }

    
}

public interface IAggregatedModifier
{
    void ApplyTo(StatusController target);
}
