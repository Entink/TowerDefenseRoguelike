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
    }

    public void Apply(StatusEffect effect)
    {
        int visibleSameTypeIndex = -1;
        for(int i = 0; i < effects.Count; i++)
        {
            var e = effects[i];
            if (e.GetType() != effect.GetType()) continue;

            if (e.uiHidden && !effect.uiHidden) continue;

            if (!e.uiHidden && effect.uiHidden) continue;

            if(e.CanStack(effect))
            {
                e.AddStack();
                e.ResetTimer();
                EffectsChanged?.Invoke();
                return;
            }

            visibleSameTypeIndex = i;
            break;

            
        }

        if(visibleSameTypeIndex >= 0)
        {
            effects[visibleSameTypeIndex] = effect;
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
        float before = shieldHP;

        float reduced = Mathf.Max(0f, dmg - armorFlat);
        if(shieldHP > 0f)
        {
            float absorb = Mathf.Min(shieldHP, reduced);
            shieldHP -= absorb;
            reduced -= absorb;
        }

        if (before > 0f && shieldHP <= 0f) EffectsChanged?.Invoke();

        return reduced;
    }

    public float GetDamageMul() => dmgMul;
    public float GetAttackSpeedMul() => atkSpeedMul;
    public float GetMoveMul() => moveMul;
    public float GetKBForceMul() => kbForceMul;
    public float GetKBResMul() => kbResMul;
    public float GetRangeAdd() => rangeAdd;
    public float GetLifeStealAdd() => lifeStealAdd;

    public void AddShield(float amount) { shieldHP += amount; }
    public void SetShield(float amount) { shieldHP = Mathf.Max(0f, amount); }
    public float GetShield() => shieldHP;

    public void AddArmor(float amount) { armorFlat += amount; }
    public void AddDamageMul(float mul) { dmgMul *= mul; }
    public void AddAttackSpeedMul(float mul) { atkSpeedMul *= mul; }
    public void AddMoveMul(float mul) { moveMul *= mul; }
    public void AddKBForceMul(float mul) { kbForceMul *= mul; }
    public void AddKBResMul(float mul) { kbResMul *= mul; }
    public void AddRange(float add) { rangeAdd += add; }
    public void AddLifeSteal(float add) { lifeStealAdd += add; }


    void RebuildAggregates()
    {
        dmgMul = 1f;
        atkSpeedMul = 1f;
        moveMul = 1f;
        kbForceMul = 1f;
        kbResMul = 1f;
        armorFlat = 0f;
        rangeAdd = 0f;
        lifeStealAdd = 0f;

        for (int i = 0; i < effects.Count; i++)
            if (effects[i] is IAggregatedModifier a) a.ApplyTo(this);
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
