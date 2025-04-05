using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttributeData
{
    [Tooltip("唯一标识")]
    public string enemyName;

    [Tooltip("预制体")]
    public GameObject enemyPrefab;

    [Tooltip("最大生命值")]
    public float maxHealth;

    [Tooltip("生命再生")]
    public float healthRegenRate;

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

    [Tooltip("范围")]
    public float attackRange;

    [Tooltip("掉落钱数")]
    public float experienceReward;

    [Tooltip("掉落概率")]
    public float dropRate;

    [Tooltip("移速")]
    public float speed;
}

[CreateAssetMenu(menuName = "Attribute System/Enemy Attribute", fileName = "EnemyAttributeDataBase")]
public class EnemyAttributeDataBase : ScriptableObject
{
    [SerializeField] private List<EnemyAttributeData> attributes = new List<EnemyAttributeData>();

    private Dictionary<string, EnemyAttributeData> _attributeDictionary;

    private void Initialize()
    {
        if (_attributeDictionary != null) return;

        _attributeDictionary = new Dictionary<string, EnemyAttributeData>();
        foreach (var atrribute in attributes)
        {
            if (!_attributeDictionary.ContainsKey(atrribute.enemyName))
            {
                _attributeDictionary.Add(atrribute.enemyName, atrribute);
            }
        }
    }

    public EnemyAttributeData GetEnemyAttribute(string enemyName)
    {
        Initialize();
        return _attributeDictionary.TryGetValue(enemyName, out var data) ? data : null;
    }

    public EnemyAttributeData GetEnemyAttribute(int index)
    {
        if (index < 0 || index >= attributes.Count)
        {
            return null;
        }
        return attributes[index];
    }

    public List<EnemyAttributeData> GetAllEnemyAtrribute()
    {
        return new List<EnemyAttributeData>(attributes);
    }
}