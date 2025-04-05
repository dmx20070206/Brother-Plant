using UnityEngine;
using System;

[System.Serializable]
public class CharacterStats
{
    [Header("唯一标识")]
    public string name;

    [Header("生命属性")]
    public Attribute maxHealth;             // 最大生命值
    public Attribute healthRegenRate;       // 生命再生

    [Header("战斗属性")]
    public Attribute critChance;            // 暴击率
    public Attribute critMultiplier;        // 暴击伤害
    public Attribute armor;                 // 护甲值
    public Attribute dodgeChance;           // 闪避
    public Attribute speed;                 // 速度

    // 运行时状态
    [NonSerialized] public float currentHealth;

    public virtual void Initialize(string name)
    {
        this.name = name;
    }

    public virtual void UpdateStats(float deltaTime)
    {
        HealthRegeneration(deltaTime);
    }

    public virtual void HealthRegeneration(float deltaTime)
    {
        currentHealth = Mathf.Clamp(
            currentHealth + healthRegenRate.Value * deltaTime,
            0,
            maxHealth.Value
        );
    }

    // 伤害计算系统
    public struct DamageResult
    {
        public bool isCrit;
        public float finalDamage;
        public EnemyStatusSystem.StatusType type;
    }

    /// <summary>
    /// incomingDamage 在本角色的属性加成下最终的伤害值
    /// </summary>
    /// <param name="incomingDamage"></param>
    /// <returns></returns>
    public virtual DamageResult CalculateDamage(float incomingDamage)
    {
        DamageResult result = new DamageResult();

        if (UnityEngine.Random.value < critChance.Value)
        {
            result.isCrit = true;
            incomingDamage *= critMultiplier.Value;
        }

        result.finalDamage = incomingDamage;
        return result;
    }


    /// <summary>
    /// damage 在本角色属性加成下造成的伤害和效果
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(DamageResult damage)
    {
        var result = damage.isCrit ? 2 * damage.finalDamage : damage.finalDamage;

        currentHealth = Mathf.Clamp(currentHealth - result, 0, maxHealth.Value);
    }
}

[System.Serializable]
public class Attribute
{
    public float _baseValue;
    private float _additiveModifier;
    private float _multiplier = 1f;

    public Attribute(float baseValue)
    {
        _baseValue = baseValue;
    }

    public float Value => (_baseValue + _additiveModifier) * _multiplier;

    public void AddModifier(float additive = 0, float multiplicative = 0)
    {
        _additiveModifier += additive;
        _multiplier += multiplicative;
    }

    public void RemoveModifier(float additive = 0, float multiplicative = 0)
    {
        _additiveModifier -= additive;
        _multiplier -= multiplicative;
    }
}