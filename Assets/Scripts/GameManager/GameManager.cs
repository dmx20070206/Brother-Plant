using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 单例实例
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
    public GameObject map_controller;
    public GameObject ui_controller;
    public GameObject music_controller;
    public GameObject sun_controller;
    public GameObject level_controller;
    public GameObject scene_controller;
    public GameObject game_data_controller;

    [Header("数据库")]
    public PlayerAttributeDataBase _playerAttribute;
    public EnemyAttributeDataBase _enemyAttribute;
    public EnemySpawnDataBase _enemySpawnData;
    public WeaponDataBase _weaponDatabase;

    #region Game
    [Header("玩家")]
    private Player _player;

    [Header("敌人")]
    private HashSet<Enemy> _enemies = new HashSet<Enemy>();
    private List<string> _enemyNames = new List<string>();

    [Header("敌人出生点")]
    private EnemySpawnController _spawner;
    private EnemyPoolSystem _enemyPoolSystem;

    [Header("武器")]
    private WeaponManager _weaponManager;

    [Header("地图")]
    private MapManager _mapManager;

    [Header("UI 控制器")]
    private UIController _ui_controller;

    [Header("Music 控制器")]
    private MusicController _music_controller;

    [Header("阳光系统控制器")]
    private SunController _sun_controller;
    #endregion

    #region Always
    [Header("场景控制器")]
    private SceneController _scene_controller;

    [Header("关卡控制器")]
    private LevelController _level_controller;

    [Header("数据控制器")]
    private GameDataController _game_data_controller;
    #endregion


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void Init()
    {
        Instantiate(scene_controller, transform);
        Instantiate(level_controller, transform);
        Instantiate(game_data_controller, transform);
    }

    #region Scene Switch
    public void EnterGame()
    {
        Instantiate(player);
        Instantiate(map_controller);
        Instantiate(ui_controller);
        Instantiate(enemySpawn);
        Instantiate(weaponManager);
        Instantiate(music_controller);
        Instantiate(sun_controller);
    }

    public void ExitGame()
    {
        GameDataController.SaveData();
    }

    public void EnterMenu()
    {
        GameDataController.ClearData();
    }

    public void ExitMenu()
    {

    }

    public void EnterShop()
    {

    }

    public void ExitShop()
    {

    }
    #endregion

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
            Debug.LogWarning($"敌人 {enemy.name} 已注册，跳过重复注册");
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (!_enemies.Remove(enemy))
        {
            Debug.LogWarning($"尝试移除未注册的敌人: {enemy.name}");
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
    public EnemySpawnController EnemySpawn
    {
        get
        {
            return _spawner;
        }
    }
    public void RegisterEnemySpawner(EnemySpawnController spawner)
    {
        if (_enemyPoolSystem != null)
        {
            UnregisterEnemySpawner();
        }
        _spawner = spawner;
        _spawner.name = "EnemySpawnController";
    }

    public void UnregisterEnemySpawner()
    {
        if (_spawner != null)
        {
            _spawner = null;
        }
    }
    public EnemyPoolSystem EnemyPoolSystem
    {
        get
        {
            return _enemyPoolSystem;
        }
    }
    public void RegisterEnemyPoolSystem(EnemyPoolSystem enemyPoolSystem)
    {
        if (_enemyPoolSystem != null)
        {
            UnregisterEnemyPoolSystem();
        }
        _enemyPoolSystem = enemyPoolSystem;
    }

    public void UnregisterEnemyPoolSystem()
    {
        if (_enemyPoolSystem != null)
        {
            _enemyPoolSystem = null;
        }
    }
    #endregion

    #region EnemySpawn DataBase
    public EnemySpawnDataBase EnemySpawnDataBase
    {
        get
        {
            return _enemySpawnData;
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
    public WeaponDataBase WeaponDatabase
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

    #region Map Management
    public MapManager MapManager
    {
        get
        {
            return _mapManager;
        }
    }

    public void RegisterMapController(MapManager mapManager)
    {
        if (_mapManager != null)
        {
            UnregisterMapController();
        }
        _mapManager = mapManager;
        _mapManager.name = "MapController";
    }

    public void UnregisterMapController()
    {
        if (_mapManager != null)
        {
            _mapManager = null;
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
        _music_controller.name = "MusicController";
    }

    public void UnregisterMusicController()
    {
        if (_music_controller != null)
        {
            _music_controller = null;
        }
    }
    #endregion

    #region Sun Controller
    public SunController SunController
    {
        get
        {
            return _sun_controller;
        }
    }

    public void RegisterSunController(SunController sun_controller)
    {
        if (_sun_controller != null)
        {
            UnregisterSunController();
        }
        _sun_controller = sun_controller;
        _sun_controller.name = "SunController";
    }

    public void UnregisterSunController()
    {
        if (_sun_controller != null)
        {
            _sun_controller = null;
        }
    }
    #endregion

    #region Level Controller
    public LevelController LevelController
    {
        get
        {
            return _level_controller;
        }
    }
    public void RegisterLevelController(LevelController level_controller)
    {
        if (_level_controller != null)
        {
            UnregisterLevelController();
        }
        _level_controller = level_controller;
        _level_controller.name = "LevelController";
    }

    public void UnregisterLevelController()
    {
        if (_level_controller != null)
        {
            _level_controller = null;
        }
    }
    #endregion

    #region Scene Controller
    public SceneController SceneController
    {
        get
        {
            return _scene_controller;
        }
    }

    public void RegisterSceneController(SceneController scene_controller)
    {
        if (_scene_controller != null)
        {
            UnregisterSceneController();
        }
        _scene_controller = scene_controller;
        _scene_controller.name = "SceneController";
    }

    public void UnregisterSceneController()
    {
        if (_scene_controller != null)
        {
            _scene_controller = null;
        }
    }
    #endregion

    #region GameData Controller
    public GameDataController GameDataController
    {
        get
        {
            return _game_data_controller;
        }
    }

    public void RegisterGameDataController(GameDataController game_data_controller)
    {
        if (_game_data_controller != null)
        {
            UnregisterGameDataController();
        }
        _game_data_controller = game_data_controller;
        _game_data_controller.name = "GameDataController";
    }

    public void UnregisterGameDataController()
    {
        if (_game_data_controller != null)
        {
            _game_data_controller = null;
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
            if (enemy == null || !enemy.isActiveAndEnabled || enemy.GetComponent<Enemy>()._isDie) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    public List<string> GetAllEnemyName()
    {
        return _enemyNames;
    }
    #endregion
}