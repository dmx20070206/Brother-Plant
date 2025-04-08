using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;



public class EnemySpawnController : MonoBehaviour
{
    [Header("预警设置")]
    [SerializeField] private GameObject warningPrefab;

    [SerializeField] private int curLevel;
    [SerializeField] private int currentEnemyCount;

    [SerializeField] private List<EnemySpawnData> spawnData = new List<EnemySpawnData>();
    [SerializeField] private List<Coroutine> spawnCoroutines = new List<Coroutine>();

    #region data, start and stop
    public void RefreshConfig()
    {
        currentEnemyCount = 0;
        curLevel = GameManager.Instance.LevelController.CurLevel;
        spawnData = GameManager.Instance.LevelController.curEnemySpawnDatas;
        StartAllSpawning();
    }

    public void StartAllSpawning()
    {
        spawnCoroutines.Clear();
        foreach (var data in spawnData)
            spawnCoroutines.Add(StartCoroutine(SpawnRoutine(data)));
    }

    public void StopAllSpawning()
    {
        foreach (var coroutine in spawnCoroutines)
            StopCoroutine(coroutine);
    }
    #endregion

    #region enable and disable
    private void OnEnable()
    {
        GameManager.Instance.RegisterEnemySpawner(this);
        RefreshConfig();
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterEnemySpawner();
        StopAllSpawning();
    }
    #endregion

    private IEnumerator SpawnRoutine(EnemySpawnData spawnData)
    {
        int totalSpawned = 0;

        yield return new WaitForSeconds(spawnData.initialDelay);

        // 遍历所有波次配置
        foreach (var wave in spawnData.waves)
        {
            // 波次生成循环
            while (totalSpawned < spawnData.TotalEnemies)
            {
                // 计算剩余可生成数量
                int remaining = spawnData.TotalEnemies - totalSpawned;
                if (remaining <= 0) yield break;

                // 生成当前批次敌人
                int currentBatch = Mathf.Min(wave.spawnCount, remaining);
                SpawnEnemies(currentBatch, spawnData);
                totalSpawned += currentBatch;

                // 等待生成间隔
                if (wave.spawnInterval > 0)
                    yield return new WaitForSeconds(wave.spawnInterval);

                // 如果当前批次未消耗完波次数量则继续
                if (currentBatch < wave.spawnCount) break;
            }

            // 等待波次结束延迟
            if (wave.postWaveDelay > 0)
                yield return new WaitForSeconds(wave.postWaveDelay);
        }
    }

    private void SpawnEnemies(int count, EnemySpawnData spawnData)
    {
        for (int i = 0; i < count; i++)
        {
            Area area = GameManager.Instance.MapManager.GenerateRandomArea();
            Vector2 spawnPos = area.GetRandomPosition();

            StartCoroutine(SpawnEnemyWithWarning(spawnPos, spawnData));
        }
    }

    private IEnumerator SpawnEnemyWithWarning(Vector2 spawnPos, EnemySpawnData spawnData)
    {
        // 生成预警圈
        GameObject warningInstance = Instantiate(warningPrefab, spawnPos, Quaternion.identity);

        // 通过动画事件销毁
        while (warningInstance != null)
        {
            yield return null;
        }

        // 生成敌人
        GameObject enemyObj = GameManager.Instance.EnemyPoolSystem.GetEnemyFromPool(spawnData.id.enemyName);
        enemyObj.transform.position = spawnPos;
        enemyObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        currentEnemyCount++;

        SortingGroup sortingGroup = enemyObj.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = currentEnemyCount;
    }

    public void EnemyDistory()
    {
        currentEnemyCount--;
    }
}