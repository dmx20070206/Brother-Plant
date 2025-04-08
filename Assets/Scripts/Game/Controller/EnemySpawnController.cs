using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;



public class EnemySpawnController : MonoBehaviour
{
    [Header("Ԥ������")]
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

        // �������в�������
        foreach (var wave in spawnData.waves)
        {
            // ��������ѭ��
            while (totalSpawned < spawnData.TotalEnemies)
            {
                // ����ʣ�����������
                int remaining = spawnData.TotalEnemies - totalSpawned;
                if (remaining <= 0) yield break;

                // ���ɵ�ǰ���ε���
                int currentBatch = Mathf.Min(wave.spawnCount, remaining);
                SpawnEnemies(currentBatch, spawnData);
                totalSpawned += currentBatch;

                // �ȴ����ɼ��
                if (wave.spawnInterval > 0)
                    yield return new WaitForSeconds(wave.spawnInterval);

                // �����ǰ����δ�����겨�����������
                if (currentBatch < wave.spawnCount) break;
            }

            // �ȴ����ν����ӳ�
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
        // ����Ԥ��Ȧ
        GameObject warningInstance = Instantiate(warningPrefab, spawnPos, Quaternion.identity);

        // ͨ�������¼�����
        while (warningInstance != null)
        {
            yield return null;
        }

        // ���ɵ���
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