using UnityEngine;

[System.Serializable]
public struct Area
{
    public Vector2 center;
    public Vector2 size;

    public Vector2 GetRandomPosition()
    {
        return new Vector2(
            center.x + Random.Range(-size.x / 2, size.x / 2),
            center.y + Random.Range(-size.y / 2, size.y / 2)
        );
    }
}

public class MapManager : MonoBehaviour
{
    [Header("地图设置")]
    [SerializeField] private SpriteRenderer mapRenderer;

    [Header("生成区域参数")]
    [SerializeField] private Vector2 minSize = new Vector2(5, 5);
    [SerializeField] private Vector2 maxSize = new Vector2(10, 10);

    private void Awake()
    {
        GameManager.Instance.RegisterMapController(this);
        mapRenderer = GameObject.Find("BackGround").GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterMapController();
    }

    public Bounds GetMapBounds()
    {
        return mapRenderer.bounds;
    }

    // 生成随机生成区域
    public Area GenerateRandomArea()
    {
        Bounds mapBounds = GetMapBounds();

        Vector2 areaSize = new Vector2(
            Random.Range(minSize.x, maxSize.x),
            Random.Range(minSize.y, maxSize.y)
        );

        Vector2 areaCenter;
        float minDistance = 5f;

        do
        {
            areaCenter = new Vector2(
                Random.Range(mapBounds.min.x + areaSize.x / 2,
                           mapBounds.max.x - areaSize.x / 2),
                Random.Range(mapBounds.min.y + areaSize.y / 2,
                           mapBounds.max.y - areaSize.y / 2)
            );
        } while (Vector2.Distance(areaCenter, GameManager.Instance.Player.transform.position) < minDistance);

        return new Area
        {
            center = areaCenter,
            size = areaSize
        };
    }
}
