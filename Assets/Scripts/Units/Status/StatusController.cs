using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class StatusController : MonoBehaviour
{
    public Action<float> OnRequestDamage;
    public Action<float> OnRequestHeal;

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
            }
        }
    }

    public void Apply(StatusEffect effect)
    {
        for(int i = 0; i < effects.Count; i++)
        {
            var e = effects[i];
            if(e.GetType() == effect.GetType() && e.CanStack(effect))
            {
                e.AddStack();
                return;
            }
        }
        effects.Add(effect);
        effect.OnApply(this);
        RebuildAggregates();
    }

    public float ModifyIncomingDamage(float dmg)
    {
        float reduced = Mathf.Max(0f, dmg - armorFlat);
        if(shieldHP > 0f)
        {
            float absorb = Mathf.Min(shieldHP, reduced);
            shieldHP -= absorb;
            reduced -= absorb;
        }
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

    
}

public interface IAggregatedModifier
{
    void ApplyTo(StatusController target);
}
