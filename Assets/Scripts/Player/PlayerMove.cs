using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("�ƶ�����")]
    [SerializeField] private float moveSpeed;                                       // �ٶ�
    [SerializeField] private float acceleration;                                    // ������Ӧ�ٶ�
    [SerializeField] private float deceleration;                                    // ������Ӧ�ٶ�
    [SerializeField][Range(0, 0.9f)] private float diagonalSpeedModifier;           // �Խ����ٶ�����

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
        // ��ȡԭʼ����
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // ��׼����������
        _moveInput = new Vector2(horizontal, vertical).normalized;

        // �Խ����ƶ��ٶ�����
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

        // ʹ��ƽ������ʵ�ּ��ٶ�Ч��
        _smoothedVelocity = Vector2.Lerp(
            _smoothedVelocity,
            targetVelocity,
            (_moveInput.magnitude > 0.1f ? acceleration : deceleration) * Time.fixedDeltaTime
        );

        // Ӧ���ٶ�
        _rb.velocity = _smoothedVelocity;
    }

    private void UpdateVisuals()
    {
        // ����ת����ˮƽ��
        if (_moveInput.x != 0)
        {
            spriteRenderer.flipX = _moveInput.x < 0;
        }
    }
}