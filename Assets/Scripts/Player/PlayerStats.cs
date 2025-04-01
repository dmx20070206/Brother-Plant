using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public Attribute lifeSteal;
    public Attribute damage;
    public Attribute attackRange;
    public Attribute harvestEfficiency;
    public Attribute luck;
    public int maxWeapons;

    public override void Initialize(string name)
    {
        base.Initialize(name);

        PlayerAttributeData data = GameManager.Instance.PlayerAttributeDataBase.GetPlayerAttribute(name);
        if (data != null)
        {
            maxHealth = new Attribute(data.maxHealth);
            healthRegenRate = new Attribute(data.healthRegenRate);
            lifeSteal = new Attribute(data.lifeSteal);
            damage = new Attribute(data.damage);
            critChance = new Attribute(data.critChance);
            critMultiplier = new Attribute(data.critMultiplier);
            armor = new Attribute(data.armor);
            dodgeChance = new Attribute(data.dodgeChance);
            speed = new Attribute(data.speed);
            attackRange = new Attribute(data.attackRange);
            harvestEfficiency = new Attribute(data.harvestEfficiency);
            luck = new Attribute(data.luck);
            maxWeapons = data.maxWeapons;
        }

        currentHealth = maxHealth.Value;
    }
}
