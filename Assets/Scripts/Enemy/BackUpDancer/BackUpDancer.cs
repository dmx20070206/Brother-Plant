using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackUpDancer : Enemy
{
    protected float _flipCooldown;            // ת����ȴʱ��
    protected float _lastFlipTime;            // ��һ�η�תʱ��
    protected bool _isAttacking;              // �Ƿ����ڹ���
    protected float _lastAttackTime;          // ��һ�ι���ʱ��
    protected bool _isInAttackRange;          // �Ƿ��ڹ�����Χ��

    protected enum CurState { Move, Dance};   
    protected CurState curState;              // ��ǰ״̬
    protected float _timeInMove;              // �����ƶ�״̬��ʱ��
    protected float _timeInDance;             // �����赸״̬��ʱ��

    protected bool _isHeadDrop;               // ͷ���Ƿ����
    protected float _timeInDie;               // ��������״̬��ʱ��
    protected float _timeToDie;               // ����״̬�ƶ����ʳ���ʱ��
    protected bool _isAlreadyDie;             // �Ƿ��Ѿ�����

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
        _stats.Initialize("BackUpDancer");
        curState = CurState.Dance;
        _isAlreadyDie = false;
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
        // ��ʾ�˺�����
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

    #region Move, Dance and Rotate
    private void UpdateMovement()
    {
        if (_status[EnemyStatusSystem.StatusType.Frozen])
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        SwitchState();

        float speed = _status[EnemyStatusSystem.StatusType.Slow] ?
            _stats.speed.Value * 0.5f : _stats.speed.Value;

        speed = curState == CurState.Move ? speed : speed * 0.1f;

        Vector2 direction = (_player.transform.position - transform.position).normalized;
        _rb.velocity = _isAttacking ? Vector2.zero : direction * speed;
    }

    private void SwitchState()
    {
        switch (curState)
        {
            case CurState.Move:
                _timeInMove += Time.deltaTime;
                break;
            case CurState.Dance:
                _timeInDance += Time.deltaTime;
                break;
            default:
                break;
        }

        if(_timeInMove > 2.5f)
        {
            animator.SetTrigger("Switch");
            _timeInMove = _timeInDance = 0f;
            curState = CurState.Dance;
        }

        if (_timeInDance > 2.5f)
        {
            animator.SetTrigger("Switch");
            _timeInMove = _timeInDance = 0f;
            curState = CurState.Move;
        }
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
