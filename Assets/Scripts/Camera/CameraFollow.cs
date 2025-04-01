using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private Transform target;                              // 跟随目标
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);    // 相机偏移
    [SerializeField] private float smoothTime = 0.25f;                      // 平滑时间
    [SerializeField] private float lookAheadFactor = 0.1f;                  // 移动预判量

    [Header("边界限制")]
    [SerializeField] private bool enableBounds = true; // 启用边界限制
    [SerializeField] private Rect levelBounds;         // 关卡边界

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastTargetPosition;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        InitializeTarget();
        _lastTargetPosition = target.position;
    }

    private void InitializeTarget()
    {
        if (target != null) return;

        // 自动查找玩家
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
        }
        else
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        GameObject background = GameObject.FindGameObjectWithTag("BackGround");
        if (background != null)
        {
            SpriteRenderer renderer = background.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                levelBounds = new Rect(
                    renderer.bounds.min.x,
                    renderer.bounds.min.y,
                    renderer.bounds.size.x,
                    renderer.bounds.size.y
                );
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            HandleMissingTarget();
            return;
        }

        Vector3 targetPosition = CalculateTargetPosition();
        ApplyCameraBounds(ref targetPosition);
        SmoothFollow(targetPosition);
    }

    private Vector3 CalculateTargetPosition()
    {
        // 计算移动方向
        Vector3 movementDirection = (target.position - _lastTargetPosition).normalized;
        _lastTargetPosition = target.position;

        // 计算预判偏移
        Vector3 lookAheadOffset = movementDirection * lookAheadFactor;

        return target.position + lookAheadOffset + offset;
    }

    private void SmoothFollow(Vector3 targetPosition)
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            smoothTime
        );
    }

    private void ApplyCameraBounds(ref Vector3 targetPos)
    {
        if (!enableBounds) return;

        // 计算相机视口尺寸
        float vertExtent = _camera.orthographicSize;
        float horizExtent = vertExtent * _camera.aspect;

        targetPos.x = Mathf.Clamp(
            targetPos.x,
            levelBounds.xMin + horizExtent,
            levelBounds.xMax - horizExtent
        );

        targetPos.y = Mathf.Clamp(
            targetPos.y,
            levelBounds.yMin + vertExtent,
            levelBounds.yMax - vertExtent
        );
    }

    private void HandleMissingTarget()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
            Debug.LogWarning("重新获取玩家目标");
        }
        else
        {
            Debug.LogError("跟随目标丢失！");
            enabled = false;
        }
    }
}