using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdinaryZombie : Enemy
{
    protected float _flipCooldown;            // ת����ȴʱ��
    protected float _lastFlipTime;            // ��һ�η�תʱ��
    protected bool _isAttacking;              // �Ƿ����ڹ���
    protected float _lastAttackTime;          // ��һ�ι���ʱ��
    protected bool _isInAttackRange;          // �Ƿ��ڹ�����Χ��

    protected bool _isDie;                    // �Ƿ��������״̬
    protected bool _isHeadDrop;               // ͷ���Ƿ����
    protected float _timeInDie;               // ��������״̬��ʱ��
    protected float _timeToDie;               // ����״̬�ƶ����ʳ���ʱ��

    protected Color _originalColor;           // ������ɫ
    protected Coroutine _flashRoutine;        // ������ЧЯ��

    [SerializeField] private GameObject headPrefab;
    [SerializeField] private Transform headPos;

    #region Life Cycle
    private void Awake()
    {
        GameManager.Instance.RegisterEnemy(this);
        InitializeComponents();
        InitializeData();

    }
    private void Update()
    {
        UpdateMovement();
        UpdateAttackCycle();
        UpdateFacingDirection();
        UpdateDieStat();
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterEnemy(this);
        }
    }
    #endregion

    #region Initialize
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void InitializeData()
    {
        _originalColor = _spriteRenderer.color;
        _player = GameManager.Instance.Player;
        _flipCooldown = 0.1f;
        _stats = new EnemyStats();
        _stats.Initialize("OrdinaryZombie");
    }
    #endregion

    #region Trigger Detect
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isAttacking)
        {
            _isInAttackRange = true;
            StartCoroutine(InitialDamage());
            StartAttack();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isInAttackRange = false;
            EndAttack();
        }
    }
    #endregion

    #region Attack
    public override void StartAttack()
    {
        _isAttacking = true;
        _rb.velocity = Vector2.zero;
        animator.SetBool("EATTING", true);
    }
    public override void EndAttack()
    {
        _isAttacking = false;
        animator.SetBool("EATTING", false);
    }
    private void UpdateAttackCycle()
    {
        if (_isInAttackRange && Time.time > _lastAttackTime + attackInterval)
        {
            ApplyDamage();
            _lastAttackTime = Time.time;
        }
    }
    private IEnumerator InitialDamage()
    {
        yield return new WaitForSeconds(initialDamageDelay);
        if (_isInAttackRange)
        {
            ApplyDamage();
        }
    }

    public override void ApplyDamage()
    {
        var damage = _stats.CalculateDamage(_stats.damage.Value);
        _player.TakeDamage(damage, transform);
    }

    public override CharacterStats.DamageResult CalculateDamage(float damage)
    {
        return _stats.CalculateDamage(damage);
    }
    #endregion

    #region Receive Attack And Die
    public override void TakeDamage(CharacterStats.DamageResult damage, Transform transform)
    {
        // ��ʾ�˺�����
        GameManager.Instance.UIController.ShowDamageFeedback(damage, transform);

        // ����������Ч
        if (_spriteRenderer != null)
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _flashRoutine = StartCoroutine(HitEffectRoutine());
        }
        _stats.TakeDamage(damage);
        if (_stats.currentHealth <= 0)
        {
            _isDie = true;
            animator.SetBool("Die", true);
        }
    }
    private void SpawnFallingHead()
    {
        _isHeadDrop = true;

        Instantiate(headPrefab, headPos);
    }
    public void UpdateDieStat()
    {
        if (_isDie) 
        { 
            _timeInDie += Time.deltaTime;
            if (!_isHeadDrop) SpawnFallingHead();
        }
        if (_timeInDie > 0.6f) 
        { 
            animator.SetTrigger("MustDie");
            _rb.velocity /= 5;
        }
    }
    public void Damage()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Move and Rotate
    private void UpdateMovement()
    {
        Vector2 direction = (_player.transform.position - transform.position).normalized;
        _rb.velocity = _isAttacking ? Vector2.zero : direction * _stats.speed.Value;
    }


    private void UpdateFacingDirection()
    {
        if (_rb.velocity.x != 0 && Time.time > _lastFlipTime + _flipCooldown)
        {
            _spriteRenderer.flipX = _rb.velocity.x > 0;
            _lastFlipTime = Time.time;
        }
    }
    #endregion

    #region Effect
    protected IEnumerator HitEffectRoutine()
    {
        float elapsed = 0;
        while (elapsed < flashDuration)
        {
            _spriteRenderer.color = Color.Lerp(
                hitColor,
                _originalColor,
                elapsed / flashDuration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = _originalColor;
    }
    #endregion
}
