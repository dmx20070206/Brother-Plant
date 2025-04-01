using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Transform target;                              // ����Ŀ��
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);    // ���ƫ��
    [SerializeField] private float smoothTime = 0.25f;                      // ƽ��ʱ��
    [SerializeField] private float lookAheadFactor = 0.1f;                  // �ƶ�Ԥ����

    [Header("�߽�����")]
    [SerializeField] private bool enableBounds = true; // ���ñ߽�����
    [SerializeField] private Rect levelBounds;         // �ؿ��߽�

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

        // �Զ��������
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
        // �����ƶ�����
        Vector3 movementDirection = (target.position - _lastTargetPosition).normalized;
        _lastTargetPosition = target.position;

        // ����Ԥ��ƫ��
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

        // ��������ӿڳߴ�
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
            Debug.LogWarning("���»�ȡ���Ŀ��");
        }
        else
        {
            Debug.LogError("����Ŀ�궪ʧ��");
            enabled = false;
        }
    }
}