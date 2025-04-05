using UnityEngine;
using System;

[System.Serializable]
public class CharacterStats
{
    [Header("Ψһ��ʶ")]
    public string name;

    [Header("��������")]
    public Attribute maxHealth;             // �������ֵ
    public Attribute healthRegenRate;       // ��������

    [Header("ս������")]
    public Attribute critChance;            // ������
    public Attribute critMultiplier;        // �����˺�
    public Attribute armor;                 // ����ֵ
    public Attribute dodgeChance;           // ����
    public Attribute speed;                 // �ٶ�

    // ����ʱ״̬
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

    // �˺�����ϵͳ
    public struct DamageResult
    {
        public bool isCrit;
        public float finalDamage;
        public EnemyStatusSystem.StatusType type;
    }

    /// <summary>
    /// incomingDamage �ڱ���ɫ�����Լӳ������յ��˺�ֵ
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
    /// damage �ڱ���ɫ���Լӳ�����ɵ��˺���Ч��
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