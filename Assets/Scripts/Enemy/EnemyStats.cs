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
        base.Initialize(name);

        EnemyAttributeData data = GameManager.Instance.EnemyAttributeDataBase.GetEnemyAttribute(name);
        if (data != null)
        {
            maxHealth = new Attribute(data.maxHealth);
            healthRegenRate = new Attribute(data.healthRegenRate);
            damage = new Attribute(data.damage);
            critChance = new Attribute(data.critChance);
            critMultiplier = new Attribute(data.critMultiplier);
            armor = new Attribute(data.armor);
            dodgeChance = new Attribute(data.dodgeChance);
            attackRange = new Attribute(data.attackRange);
            experienceReward = new Attribute(data.experienceReward);
            dropRate = new Attribute(data.dropRate);
            speed = new Attribute(data.speed);
        }
           
        currentHealth = maxHealth.Value;
    }

    // 敌人死亡处理
    public void HandleDeath(Enemy enemy)
    {
        SpawnLoot();
        GrantExperience();
    }

    private void SpawnLoot()
    {
        if (Random.value <= dropRate.Value)
        {
            // 调用掉落系统
        }
    }

    private void GrantExperience()
    {

    }
}