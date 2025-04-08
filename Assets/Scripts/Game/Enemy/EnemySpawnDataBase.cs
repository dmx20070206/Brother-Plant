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
        [Tooltip("������������")]
        public int spawnCount;

        [Tooltip("���ɼ�����룩")]
        public float spawnInterval;

        [Tooltip("���ν�����ȴ�ʱ��")]
        public float postWaveDelay;
    }

    [System.Serializable]
    public struct SpawnId
    {
        [Tooltip("�ؿ����")]
        public int level;

        [Tooltip("��������")]
        public string enemyName;
    }

    [Tooltip("ID")]
    public SpawnId id;

    [Tooltip("ʹ�õĵ���Ԥ����")]
    public GameObject enemyPrefab;

    [Header("��ʼ�ӳ�")]
    public float initialDelay;

    [Header("��������")]
    public SpawnWave[] waves;

    [Header("ȫ������")]
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
