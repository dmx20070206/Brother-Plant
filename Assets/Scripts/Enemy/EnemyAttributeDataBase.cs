using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttributeData
{
    [Tooltip("Ψһ��ʶ")]
    public string enemyName;

    [Tooltip("Ԥ����")]
    public GameObject enemyPrefab;

    [Tooltip("�������ֵ")]
    public float maxHealth;

    [Tooltip("��������")]
    public float healthRegenRate;

    [Tooltip("�˺�")]
    public float damage;

    [Tooltip("������")]
    [Range(0, 1)] public float critChance;

    [Tooltip("�����˺�")]
    public float critMultiplier;

    [Tooltip("����")]
    public float armor;

    [Tooltip("����")]
    [Range(0, 1)] public float dodgeChance;

    [Tooltip("��Χ")]
    public float attackRange;

    [Tooltip("����Ǯ��")]
    public float experienceReward;

    [Tooltip("�������")]
    public float dropRate;

    [Tooltip("����")]
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