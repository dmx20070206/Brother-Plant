using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [System.Serializable]
    public struct SunTypeConfig
    {
        public int value;           // 对应面值
        public float size;          // 对应体积
        public GameObject prefab;   // 对应预制体
    }

    [Header("阳光类型配置")]
    [SerializeField]
    private SunTypeConfig[] sunTypes = {
        new SunTypeConfig{value=100, size=4},
        new SunTypeConfig{value=50, size=3},
        new SunTypeConfig{value=10, size=2},
        new SunTypeConfig{value=5, size=1}
    };

    [Header("掉落参数")]
    [SerializeField] private float dropRadius = 1f;         // 掉落散布半径
    [SerializeField] private float verticalForce = 5f;      // 垂直弹跳力

    private float sunshineTotal;

    private void Awake()
    {
        GameManager.Instance.RegisterSunController(this);
        sunshineTotal = 0f;
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterSunController();
    }

    public void TryDropItems(Enemy enemy)
    {
        float dropRate = enemy.Stats.dropRate.Value;
        int experienceReward = (int)enemy.Stats.experienceReward.Value;

        if (Random.value > dropRate)
            return;

        Dictionary<int, int> breakdown = CalculateBreakdown(experienceReward);
        SpawnSuns(breakdown, enemy.transform);
    }

    private Dictionary<int, int> CalculateBreakdown(int total)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        int remaining = total;

        foreach (SunTypeConfig config in sunTypes)
        {
            int count = remaining / config.value;
            if (count > 0)
            {
                result.Add(config.value, count);
                remaining %= config.value;
            }
        }

        return result;
    }

    private void SpawnSuns(Dictionary<int, int> breakdown, Transform dropCenter)
    {
        foreach (var pair in breakdown)
        {
            SunTypeConfig config = System.Array.Find(sunTypes, x => x.value == pair.Key);

            for (int i = 0; i < pair.Value; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-dropRadius, dropRadius),
                    0,
                    Random.Range(-dropRadius, dropRadius)
                );

                GameObject sun = Instantiate(config.prefab,
                    dropCenter.position + randomOffset,
                    Quaternion.identity);

                // 添加物理效果
                Rigidbody rb = sun.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddForce(Vector3.up * verticalForce, ForceMode.Impulse);
                }

                // 设置阳光属性
                SunCollectible sunScript = sun.GetComponent<SunCollectible>();
                if (sunScript)
                {
                    sunScript.Initialize(config.value, config.size);
                }
            }
        }
    }

    public void AddSun(int amount)
    {
        sunshineTotal += amount;
    }

    public void SubtractSun(int amount)
    {
        sunshineTotal -= amount;
    }

    public float GetSunshineTotal()
    {
        return sunshineTotal;
    }
}
