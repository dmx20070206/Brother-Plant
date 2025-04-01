using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed;                                       // 速度
    [SerializeField] private float acceleration;                                    // 加速响应速度
    [SerializeField] private float deceleration;                                    // 减速响应速度
    [SerializeField][Range(0, 0.9f)] private float diagonalSpeedModifier;           // 对角线速度修正

    private Animator animator;                                    
    private SpriteRenderer spriteRenderer;                        
    private Rigidbody2D _rb;

    private Vector2 _moveInput;
    private Vector2 _smoothedVelocity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        moveSpeed = GetComponent<Player>().stats.speed.Value;
        acceleration = 2 * moveSpeed;
        deceleration = 3 * moveSpeed;
        diagonalSpeedModifier = 0.95f;
    }

    private void Update()
    {
        GetPlayerInput();
        UpdateVisuals();
    }

    private void GetPlayerInput()
    {
        // 获取原始输入
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 标准化输入向量
        _moveInput = new Vector2(horizontal, vertical).normalized;

        // 对角线移动速度修正
        if (Mathf.Abs(horizontal) > 0 && Mathf.Abs(vertical) > 0)
        {
            _moveInput *= diagonalSpeedModifier;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        Vector2 targetVelocity = _moveInput * moveSpeed;

        // 使用平滑阻尼实现加速度效果
        _smoothedVelocity = Vector2.Lerp(
            _smoothedVelocity,
            targetVelocity,
            (_moveInput.magnitude > 0.1f ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        // 应用速度
        _rb.velocity = _smoothedVelocity;
    }

    private void UpdateVisuals()
    {
        // 方向翻转（仅水平）
        if (_moveInput.x != 0)
        {
            spriteRenderer.flipX = _moveInput.x < 0;
        }
    }
}