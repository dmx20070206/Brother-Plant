using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttributeData
{
    [Tooltip("唯一标识")]
    public string playerName;

    [Tooltip("最大生命值")]
    public float maxHealth;

    [Tooltip("生命再生")]
    public float healthRegenRate;

    [Tooltip("生命窃取")]
    [Range(0, 1)] public float lifeSteal;

    [Tooltip("伤害")]
    public float damage;

    [Tooltip("暴击率")]
    [Range(0, 1)] public float critChance;

    [Tooltip("暴击伤害")]
    public float critMultiplier;

    [Tooltip("护甲")]
    public float armor;

    [Tooltip("闪避")]
    [Range(0, 1)] public float dodgeChance;

    [Tooltip("速度")]
    public float speed;

    [Tooltip("范围")]
    public float attackRange;

    [Tooltip("收获")]
    public float harvestEfficiency;

    [Tooltip("幸运")]
    public float luck;

    [Tooltip("最大武器数")]
    public int maxWeapons;
}

[CreateAssetMenu(menuName = "Attribute System/Player Attribute", fileName = "PlayerAttributeDataBase")]
public class PlayerAttributeDataBase : ScriptableObject
{
    [SerializeField] private List<PlayerAttributeData> attributes = new List<PlayerAttributeData>();

    private Dictionary<string, PlayerAttributeData> _attributeDictionary;

    private void Initialize()
    {
        if (_attributeDictionary != null) return;

        _attributeDictionary = new Dictionary<string, PlayerAttributeData>();
        foreach (var atrribute in attributes)
        {
            if (!_attributeDictionary.ContainsKey(atrribute.playerName))
            {
                _attributeDictionary.Add(atrribute.playerName, atrribute);
            }
        }
    }

    public PlayerAttributeData GetPlayerAttribute(string playerName)
    {
        Initialize();
        return _attributeDictionary.TryGetValue(playerName, out var data) ? data : null;
    }

    public PlayerAttributeData GetPlayerAttribute(int index)
    {
        if (index < 0 || index >= attributes.Count)
        {
            return null;
        }
        return attributes[index];
    }

    public List<PlayerAttributeData> GetAllPlayerAtrribute()
    {
        return new List<PlayerAttributeData>(attributes);
    }
}