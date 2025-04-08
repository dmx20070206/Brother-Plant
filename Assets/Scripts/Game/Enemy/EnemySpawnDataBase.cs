using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static EnemySpawnData;

[System.Serializable]
public class EnemySpawnData
{
    [System.Serializable]
    public class SpawnWave
    {
        [Tooltip("单次生成数量")]
        public int spawnCount;

        [Tooltip("生成间隔（秒）")]
        public float spawnInterval;

        [Tooltip("波次结束后等待时间")]
        public float postWaveDelay;
    }

    [System.Serializable]
    public struct SpawnId
    {
        [Tooltip("关卡序号")]
        public int level;

        [Tooltip("敌人名称")]
        public string enemyName;
    }

    [Tooltip("ID")]
    public SpawnId id;

    [Tooltip("使用的敌人预制体")]
    public GameObject enemyPrefab;

    [Header("初始延迟")]
    public float initialDelay;

    [Header("波次设置")]
    public SpawnWave[] waves;

    [Header("全局设置")]
    public int TotalEnemies;
}

[CreateAssetMenu(menuName = "Level System/EnemySpawn Config")]
public class EnemySpawnDataBase : ScriptableObject
{
    [SerializeField] private List<EnemySpawnData> spawnData = new List<EnemySpawnData>();

    private Dictionary<SpawnId, EnemySpawnData> _enemySpawnDataDic;

    private void Initialize()
    {
        if (_enemySpawnDataDic != null) return;

        _enemySpawnDataDic = new Dictionary<SpawnId, EnemySpawnData>();
        foreach (var atrribute in spawnData)
        {
            if (!_enemySpawnDataDic.ContainsKey(atrribute.id))
            {
                _enemySpawnDataDic.Add(atrribute.id, atrribute);
            }
        }
    }

    public EnemySpawnData GetEnemySpawnData(SpawnId id)
    {
        Initialize();
        return _enemySpawnDataDic.TryGetValue(id, out var data) ? data : null;
    }

    public EnemySpawnData GetEnemySpawnData(int index)
    {
        if (index < 0 || index >= spawnData.Count)
        {
            return null;
        }

        return spawnData[index];
    }

    public List<EnemySpawnData> GetEnemySpawnDataByLevel(int level)
    {
        List<EnemySpawnData> enemySpawnDatas = new List<EnemySpawnData>();

        foreach (var item in spawnData)
        {
            if (item.id.level == level)
                enemySpawnDatas.Add(item);
        }

        return enemySpawnDatas;
    }

    public List<EnemySpawnData> GetAllEnemySpawnData()
    {
        return new List<EnemySpawnData>(spawnData);
    }
}
