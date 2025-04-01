using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdinaryZombie : Enemy
{
    protected float _flipCooldown;            // 转向冷却时间
    protected float _lastFlipTime;            // 上一次翻转时间
    protected bool _isAttacking;              // 是否正在攻击
    protected float _lastAttackTime;          // 上一次攻击时间
    protected bool _isInAttackRange;          // 是否在攻击范围内

    protected bool _isDie;                    // 是否进入死亡状态
    protected bool _isHeadDrop;               // 头部是否掉落
    protected float _timeInDie;               // 进入死亡状态的时间
    protected float _timeToDie;               // 死亡状态移动或进食的最长时间

    protected Color _originalColor;           // 正常颜色
    protected Coroutine _flashRoutine;        // 受伤特效携程

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
        // 显示伤害数字
        GameManager.Instance.UIController.ShowDamageFeedback(damage, transform);

        // 触发受伤特效
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
