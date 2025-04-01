using System.Collections.Generic;
using UnityEngine;

// ÎäÆ÷Êı¾İÀà
[System.Serializable]
public class WeaponData
{
    public string weaponName;               // ÎäÆ÷Î¨Ò»±êÊ¶Ãû
    public WeaponBase prefab;               // ÎäÆ÷Ô¤ÖÆÌå
    public Sprite weaponIcon;               // ÎäÆ÷Í¼±ê
    [TextArea] public string description;   // ÎäÆ÷ÃèÊö

    public float damage;                    // ÎäÆ÷ÉËº¦
    public float attackInterval;            // ÎäÆ÷ÀäÈ´
    public float attackRange = 50f;         // ÎäÆ÷·¶Î§
    public float rotationSpeed = 20f;       // Ğı×ªËÙ¶È
}

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Weapon System/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();

    private Dictionary<string, WeaponData> _weaponDictionary;

    private void Initialize()
    {
        if (_weaponDictionary != null) return;

        _weaponDictionary = new Dictionary<string, WeaponData>();
        foreach (var weapon in weapons)
        {
            if (!_weaponDictionary.ContainsKey(weapon.weaponName))
            {
                _weaponDictionary.Add(weapon.weaponName, weapon);
            }
        }
    }

    public WeaponData GetWeapon(string weaponName)
    {
        Initialize();
        return _weaponDictionary.TryGetValue(weaponName, out var data) ? data : null;
    }

    public WeaponData GetWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
        {
            return null;
        }
        return weapons[index];
    }

    public List<WeaponData> GetAllWeapons()
    {
        return new List<WeaponData>(weapons);
    }
}