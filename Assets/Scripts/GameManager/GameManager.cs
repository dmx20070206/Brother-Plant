using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // µ¥ÀýÊµÀý
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Prefabs")]
    public GameObject player;
    public GameObject enemySpawn;
    public GameObject weaponManager;
    public GameObject ui_controller;
    public GameObject music_controller;

    [Header("Íæ¼Ò")]
    public Player _player;
    public PlayerAttributeDataBase _playerAttribute;

    [Header("µÐÈË")]
    public HashSet<Enemy> _enemies = new HashSet<Enemy>();
    public EnemyAttributeDataBase _enemyAttribute;

    [Header("µÐÈË³öÉúµã")]
    public EnemySpawn _spawner;

    [Header("ÎäÆ÷")]
    public WeaponManager _weaponManager;
    public WeaponDatabase _weaponDatabase;

    [Header("UI ¿ØÖÆÆ÷")]
    public UIController _ui_controller;

    [Header("Music ¿ØÖÆÆ÷")]
    public MusicController _music_controller;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitialAll();
    }

    public void InitialAll()
    {
        Instantiate(player);
        Instantiate(enemySpawn);
        Instantiate(weaponManager);
        Instantiate(ui_controller);
        Instantiate(music_controller);
    }

    #region Player Management
    public Player Player
    {
        get
        {
            return _player;
        }
    }

    public void RegisterPlayer(Player player)
    {
        if (_player != null)
        {
            UnregisterPlayer();
        }
        _player = player;
        _player.name = "Player";
    }

    public void UnregisterPlayer()
    {
        if (_player != null)
        {
            _player = null;
        }
    }
    #endregion

    #region Enemy Management
    public IReadOnlyCollection<Enemy> Enemies => _enemies;

    public void RegisterEnemy(Enemy enemy)
    {
        if (!_enemies.Add(enemy))
        {
            Debug.LogWarning($"µÐÈË {enemy.name} ÒÑ×¢²á£¬Ìø¹ýÖØ¸´×¢²á");
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (!_enemies.Remove(enemy))
        {
            Debug.LogWarning($"³¢ÊÔÒÆ³ýÎ´×¢²áµÄµÐÈË: {enemy.name}");
        }
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        _enemies.Clear();
    }
    #endregion

    #region Enemy Spawner
    public EnemySpawn EnemySpawn
    {
        get
        {
            return _spawner;
        }
    }
    public void RegisterEnemySpawner(EnemySpawn spawner)
    {
        if(_spawner != null)
        {
            UnregisterEnemySpawner();
        }    
        _spawner = spawner;
        _spawner.name = "EnemySpawn";
    }

    public void UnregisterEnemySpawner()
    {
        if(_spawner != null)
        {
            _spawner = null;
        }
    }
    #endregion

    #region Weapon Management
    public WeaponManager WeaponManager
    {
        get
        {
            return _weaponManager;
        }
    }

    public void RegisterWeaponManager(WeaponManager weaponManager)
    {
        if (weaponManager != null)
        {
            UnregisterWeaponManger();
        }
        _weaponManager = weaponManager;
        _weaponManager.name = "WeaponManager";
    }

    public void UnregisterWeaponManger()
    {
        if (_weaponManager != null)
        {
            _weaponManager = null;
        }
    }
    #endregion

    #region Weapon DataBase
    public WeaponDatabase WeaponDatabase
    {
        get
        {
            return _weaponDatabase;
        }
    }
    #endregion

    #region PlayerAttribute DataBase
    public PlayerAttributeDataBase PlayerAttributeDataBase
    {
        get
        {
            return _playerAttribute;
        }
    }
    #endregion

    #region Enemy DataBase
    public EnemyAttributeDataBase EnemyAttributeDataBase
    {
        get
        {
            return _enemyAttribute;
        }
    }
    #endregion

    #region UI Controller
    public UIController UIController
    {
        get
        {
            return _ui_controller;
        }
    }

    public void RegisterUIController(UIController uicontroller)
    {
        if (_ui_controller != null)
        {
            UnregisterUIController();
        }
        _ui_controller = uicontroller;
        _ui_controller.name = "UIController";
    }

    public void UnregisterUIController()
    {
        if (_ui_controller != null)
        {
            _ui_controller = null;
        }
    }
    #endregion

    #region Music Controller
    public MusicController MusicController
    {
        get
        {
            return _music_controller;
        }
    }

    public void RegisterMusicController(MusicController music_controller)
    {
        if (_music_controller != null)
        {
            UnregisterMusicController();
        }
        _music_controller = music_controller;
        _music_controller.name = "UIController";
    }

    public void UnregisterMusicController()
    {
        if (_music_controller != null)
        {
            _music_controller = null;
        }
    }
    #endregion

    #region Utils
    public Enemy FindNearestEnemy(Vector3 position)
    {
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in _enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }
    #endregion
}