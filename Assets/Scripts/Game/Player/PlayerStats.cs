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
        PlayerStats data = GameManager.Instance.GameDataController.PlayerData;

        foreach (var field in typeof(PlayerStats).GetFields())
        {
            field.SetValue(this, field.GetValue(data));
        }

        base.Initialize(name);

        currentHealth = maxHealth.Value;
    }
}
