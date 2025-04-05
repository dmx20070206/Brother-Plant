using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("武器位置设置")]
    [SerializeField] private float weaponRadius = 3f;                   // 武器环绕半径
    [SerializeField] private Vector2[] rightPositions = new Vector2[3]; // 右侧3个位置点
    [SerializeField] private Vector2[] leftPositions = new Vector2[3];  // 左侧3个位置点

    private Transform _weaponContainer;

    public List<WeaponBase> _weapons = new List<WeaponBase>();

    private int maxWeapons;
    private PlayerStats _playerStats;

    private void Start()
    {
        GameManager.Instance.RegisterWeaponManager(this);
        Initialize();
    }

    public void Initialize()
    {
        _playerStats = GameManager.Instance.Player._stats;
        maxWeapons = _playerStats.maxWeapons;

        InitializePositionPoints();

        _weaponContainer = new GameObject("WeaponContainer").transform;
        _weaponContainer.SetParent(GameManager.Instance.Player.transform);

        // test
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("Jalapeno").prefab));
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("PeaShooter").prefab));
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("IceShooter").prefab));
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("DoublePeaShooter").prefab));
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("GatlingPeaShooter").prefab));
        AddWeapon(Instantiate(GameManager.Instance.WeaponDatabase.GetWeapon("Jalapeno").prefab));
    }

    private void InitializePositionPoints()
    {
        rightPositions[0] = new Vector2(weaponRadius, 0);                     
        rightPositions[1] = new Vector2(weaponRadius, weaponRadius * 1f);
        rightPositions[2] = new Vector2(weaponRadius, -weaponRadius * 1f);

        leftPositions[0] = new Vector2(-weaponRadius, 0);
        leftPositions[1] = new Vector2(-weaponRadius, weaponRadius * 1f);
        leftPositions[2] = new Vector2(-weaponRadius, -weaponRadius * 1f);
    }

    public bool AddWeapon(WeaponBase newWeapon)
    {
        if (_weapons.Count >= maxWeapons) return false;

        newWeapon.Initialize(transform);
        _weapons.Add(newWeapon);

        UpdateWeaponPositions();
        UpdatePlayerStats();
        return true;
    }

    public void RemoveWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Count) return;

        _weapons[index].Deactivate();
        _weapons.RemoveAt(index);

        UpdateWeaponPositions();
        UpdatePlayerStats();
    }
    public void UpdateWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Count) return;

        _weapons[index].UpgradeWeapon();

        UpdatePlayerStats();
    }

    private void UpdatePlayerStats()
    {
        // 根据当前武器更新玩家属性
    }

    private void UpdateWeaponPositions()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            WeaponBase weapon = _weapons[i];
            bool isRightSide = GetWeaponSide(i + 1);
            int positionIndex = GetPositionIndex(i + 1);

            Vector2 localPos = isRightSide ?
                rightPositions[positionIndex] :
                leftPositions[positionIndex];

            weapon.transform.SetParent(_weaponContainer);
            weapon.transform.localPosition = localPos;
            weapon.transform.localRotation = Quaternion.identity;

            Vector3 scale = weapon.transform.localScale;
            scale.x = isRightSide ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            weapon.transform.localScale = scale;
        }
    }

    private bool GetWeaponSide(int weaponIndex)
    {
        return weaponIndex switch
        {
            1 => true,   // 第1把在右
            2 => false,  // 第2把在左
            3 => true,   // 第3把在右
            4 => false,  // 第4把在左
            5 => true,   // 第5把在右
            6 => false,  // 第6把在左
            _ => true
        };
    }

    private int GetPositionIndex(int weaponIndex)
    {
        int countOnSide = GetWeaponsOnSideCount(weaponIndex);
        int positionIndex = (countOnSide - 1) switch
        {
            0 => 0, // 第一个武器
            1 => 1, // 第二个武器（上）
            2 => 2, // 第三个武器（下）
            _ => 0
        };
        return positionIndex;
    }

    private int GetWeaponsOnSideCount(int weaponIndex)
    {
        bool currentSide = GetWeaponSide(weaponIndex);
        int count = 0;

        for (int i = 0; i < weaponIndex; i++)
        {
            if (GetWeaponSide(i + 1) == currentSide)
            {
                count++;
            }
        }
        return count;
    }

    public void Update()
    {
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterWeaponManger();
    }
}