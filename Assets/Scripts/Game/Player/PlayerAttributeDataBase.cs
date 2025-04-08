using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttributeData
{
    [Tooltip("Ψһ��ʶ")]
    public string playerName;

    [Tooltip("�������ֵ")]
    public float maxHealth;

    [Tooltip("��������")]
    public float healthRegenRate;

    [Tooltip("������ȡ")]
    [Range(0, 1)] public float lifeSteal;

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

    [Tooltip("�ٶ�")]
    public float speed;

    [Tooltip("��Χ")]
    public float attackRange;

    [Tooltip("�ջ�")]
    public float harvestEfficiency;

    [Tooltip("����")]
    public float luck;

    [Tooltip("���������")]
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