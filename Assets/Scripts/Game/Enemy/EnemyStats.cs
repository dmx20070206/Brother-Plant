using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class EnemyStats : CharacterStats
{
    public Attribute experienceReward;
    public Attribute dropRate;

    public Attribute damage;
    public Attribute attackRange;

    public override void Initialize(string name)
    {
        EnemyStats data = Attribute_to_EnemyStats(name);

        foreach (var field in typeof(EnemyStats).GetFields())
        {
            field.SetValue(this, field.GetValue(data));
        }

        base.Initialize(name);

        currentHealth = maxHealth.Value;
    }
}