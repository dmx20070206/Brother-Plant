using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

[System.Serializable]
public struct SpawnArea
{
    public Vector2 center;
    public Vector2 size;
}

public class EnemySpawn : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private SpawnArea[] spawnAreas;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int spawnCountPerWave = 3;

    [Header("Ԥ������")]
    [SerializeField] private GameObject warningPrefab;

    private int currentEnemyCount = 0;
    private Coroutine spawnCoroutine;

    public GameObject EnemyContainer;

    private void OnEnable()
    {
        GameManager.Instance.RegisterEnemySpawner(this);
        EnemyContainer = new GameObject("EnemyContainer");
        StartSpawning();
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterEnemySpawner();
        StopSpawning();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemyCount < maxEnemies)
            {
                int remaining = maxEnemies - currentEnemyCount;
                int spawnCount = Mathf.Min(spawnCountPerWave, remaining);

                SpawnEnemies(spawnCount);
            }
        }
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnArea area = spawnAreas[Random.Range(0, spawnAreas.Length)];
            Vector2 spawnPos = GetRandomPositionInArea(area);

            StartCoroutine(SpawnEnemyWithWarning(spawnPos));
        }
    }

    private IEnumerator SpawnEnemyWithWarning(Vector2 spawnPos)
    {
        // ����Ԥ��Ȧ
        GameObject warningInstance = Instantiate(warningPrefab, spawnPos, Quaternion.identity);
        warningInstance.transform.SetParent(EnemyContainer.transform);

        // ��ȡԤ���������
        Animator warningAnimator = warningInstance.GetComponent<Animator>();
        ParticleSystem warningParticles = warningInstance.GetComponent<ParticleSystem>();

        // �ȴ�Ԥ������
        bool useDestroyEvent = warningAnimator != null;

        // ͨ�������¼�����
        while (warningInstance != null)
        {
            yield return null;
        }

        // ��������������
        if (currentEnemyCount >= maxEnemies)
            yield break;

        if(Random.value < 0.7f) enemyPrefab = GameManager.Instance.EnemyAttributeDataBase.GetEnemyAttribute("OrdinaryZombie").enemyPrefab;
        else if(Random.value < 0.7f) enemyPrefab = GameManager.Instance.EnemyAttributeDataBase.GetEnemyAttribute("BackUpDancer").enemyPrefab;
        else enemyPrefab = GameManager.Instance.EnemyAttributeDataBase.GetEnemyAttribute("FootBallZombie").enemyPrefab;

        // ���ɵ���
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemyObj.transform.SetParent(EnemyContainer.transform);
        enemyObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        currentEnemyCount++;

        SortingGroup sortingGroup = enemyObj.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = currentEnemyCount;
    }

    private Vector2 GetRandomPositionInArea(SpawnArea area)
    {
        return area.center + new Vector2(
            Random.Range(-area.size.x / 2, area.size.x / 2),
            Random.Range(-area.size.y / 2, area.size.y / 2)
        );
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        currentEnemyCount--;
    }
}