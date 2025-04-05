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

    protected bool _isHeadDrop;               // 头部是否掉落
    protected float _timeInDie;               // 进入死亡状态的时间
    protected float _timeToDie;               // 死亡状态移动或进食的最长时间
    protected bool _isAlreadyDie;             // 是否已经死亡

    [SerializeField] private GameObject headPrefab;
    [SerializeField] private Transform headPos;

    private Dictionary<EnemyStatusSystem.StatusType, bool> _status;

    #region Life Cycle
    private void Awake()
    {
        GameManager.Instance.RegisterEnemy(this);
        InitializeComponents();
        InitializeData();
    }

    private void Start()
    {
        _status = GetComponent<EnemyStatusSystem>()._have_status;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateAttackCycle();
        UpdateFacingDirection();
        UpdateDieStat();

        animator.speed = _status[EnemyStatusSystem.StatusType.Frozen] ? 0f : 1f;
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
        animator.SetBool("Eat", true);
    }
    public override void EndAttack()
    {
        _isAttacking = false;
        animator.SetBool("Eat", false);
    }
    private void UpdateAttackCycle()
    {
        if (_status[EnemyStatusSystem.StatusType.Frozen]) return;
        if (_isAlreadyDie) return;

        float interval = _status[EnemyStatusSystem.StatusType.Slow] ? attackInterval * 0.5f : attackInterval;

        if (_isInAttackRange && Time.time > _lastAttackTime + interval)
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
            _isAlreadyDie = true;
            _rb.velocity = Vector2.zero;
        }
    }
    public void Damage()
    {
        GameManager.Instance.SunController.TryDropItems(this);
        Destroy(gameObject);
    }
    #endregion

    #region Move and Rotate
    private void UpdateMovement()
    {
        if (_status[EnemyStatusSystem.StatusType.Frozen]) 
        {
            _rb.velocity = Vector2.zero;
            return; 
        }

        float speed = _status[EnemyStatusSystem.StatusType.Slow] ?
            _stats.speed.Value * 0.5f : _stats.speed.Value;

        Vector2 direction = (_player.transform.position - transform.position).normalized;
        _rb.velocity = _isAttacking ? Vector2.zero : direction * speed;
    }

    private void UpdateFacingDirection()
    {
        if (_status[EnemyStatusSystem.StatusType.Frozen] == true) return;

        if (_rb.velocity.x != 0 && Time.time > _lastFlipTime + _flipCooldown)
        {
            _spriteRenderer.flipX = _rb.velocity.x > 0;
            _lastFlipTime = Time.time;
        }
    }
    #endregion
}
