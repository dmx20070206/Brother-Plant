using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolSystem : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPool
    {
        public GameObject prefab;
        public int poolSize;
        public Queue<GameObject> availableObjects = new Queue<GameObject>();
        public Queue<GameObject> not_availableObjects = new Queue<GameObject>();
    }

    private Dictionary<string, EnemyPool> enemyPoolDict = new Dictionary<string, EnemyPool>();
    private GameObject EnemyContainer;

    void Awake()
    {
        GameManager.Instance.RegisterEnemyPoolSystem(this);

        EnemyContainer = new GameObject("EnemyContainer");
        InitializePools();
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterEnemyPoolSystem();
    }

    private void InitializePools()
    {
        enemyPoolDict.Clear();
        foreach (EnemySpawnData spawnData in GameManager.Instance.LevelController.curEnemySpawnDatas)
        {
            CreatePoolForEnemyType(spawnData, spawnData.TotalEnemies);
        }
    }

    private void CreatePoolForEnemyType(EnemySpawnData data, int poolSize)
    {
        if (!enemyPoolDict.ContainsKey(data.id.enemyName))
        {
            EnemyPool newPool = new EnemyPool
            {
                prefab = data.enemyPrefab,
                poolSize = poolSize
            };

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(data.enemyPrefab, EnemyContainer.transform);
                obj.SetActive(false);
                newPool.availableObjects.Enqueue(obj);
            }

            enemyPoolDict.Add(data.id.enemyName, newPool);
        }
    }

    // 获取可用敌人实例
    public GameObject GetEnemyFromPool(string enemyName)
    {
        if (enemyPoolDict.ContainsKey(enemyName))
        {
            EnemyPool pool = enemyPoolDict[enemyName];

            if (pool.availableObjects.Count == 0)
            {
                ExpandPool(pool, 5); // 池不足时自动扩展
            }

            GameObject obj = pool.availableObjects.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    // 回收敌人实例
    public void ReturnEnemyToPool(GameObject enemy)
    {
        EnemyPool pool = enemyPoolDict[enemy.GetComponent<Enemy>().Stats.name];
        enemy.SetActive(false);
        pool.not_availableObjects.Enqueue(enemy);
    }

    // 动态扩展对象池
    private void ExpandPool(EnemyPool pool, int expandAmount)
    {
        for (int i = 0; i < expandAmount; i++)
        {
            GameObject obj = Instantiate(pool.prefab, EnemyContainer.transform);
            obj.SetActive(false);
            pool.availableObjects.Enqueue(obj);
        }
        pool.poolSize += expandAmount;
    }

    public void DestroyAll()
    {
        foreach (var item in enemyPoolDict)
        {
            foreach (var enemy in item.Value.not_availableObjects)
                if (!enemy)
                    Destroy(enemy);
            foreach (var enemy in item.Value.availableObjects)
                if (!enemy)
                    Destroy(enemy);
            item.Value.not_availableObjects.Clear();
            item.Value.availableObjects.Clear();
        }
    }
}