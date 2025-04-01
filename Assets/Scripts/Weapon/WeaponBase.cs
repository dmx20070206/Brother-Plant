using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("»ù´¡ÊôÐÔ")]
    [SerializeField] protected string weaponName = "Weapon";
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float attackInterval = 0.5f;
    [SerializeField] protected float attackRange = 50f;
    [SerializeField] protected float rotationSpeed = 20f;
    [SerializeField] protected Sprite weaponIcon;

    protected Enemy _currentTarget;
    protected float _lastSearchTime;
    protected const float SEARCH_INTERVAL = 0.3f;

    protected Transform _owner;
    protected float _lastAttackTime;
    protected bool _isActive = false;

    public string WeaponName => weaponName;
    public Sprite Icon => weaponIcon;

    public virtual void Initialize(Transform owner)
    {
        _owner = owner;
        Activate();
    }

    public virtual void Activate()
    {
        _isActive = true;
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false);
    }

    public virtual void TryAttack()
    {
        if (Time.time > _lastAttackTime + attackInterval)
        {
            ExecuteAttack();
            _lastAttackTime = Time.time;
        }
    }

    public virtual void UpdateTarget()
    {
        if (Time.time > _lastSearchTime + SEARCH_INTERVAL)
        {
            _currentTarget = GameManager.Instance.FindNearestEnemy(transform.position);
            _lastSearchTime = Time.time;
        }
    }
    public virtual bool IsTargetInRange()
    {
        return HasValidTarget() &&
               Vector3.Distance(transform.position, _currentTarget.transform.position) <= attackRange;
    }

    public virtual bool HasValidTarget()
    {
        return _currentTarget != null && _currentTarget.isActiveAndEnabled;
    }

    public abstract void RotateBarrel();

    protected abstract void ExecuteAttack();

    public abstract void UpgradeWeapon();
}