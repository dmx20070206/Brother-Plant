using UnityEngine;

public class PeaShooterWeapon : WeaponBase
{
    [Header("发射设置")]
    [SerializeField] private PeaProjectile peaPrefab;
    [SerializeField] private Transform shootPoints;

    [Header("进阶设置")]
    [SerializeField] private int projectilesPerShot = 1;

    private void Update()
    {
        UpdateTarget();
        RotateBarrel();
        TryAttack();
    }

    public override void Initialize(Transform owner)
    {
        base.Initialize(owner);
        if (!shootPoints) shootPoints = transform.Find("FirePoint");

        weaponName = "PeaShooter";
        WeaponData data = GameManager.Instance._weaponDatabase.GetWeapon(weaponName);

        if (data != null)
        {
            damage = data.damage;
            attackInterval = data.attackInterval;
            weaponIcon = data.weaponIcon;
            attackRange = data.attackRange;
            rotationSpeed = data.rotationSpeed;
        }
    }

    protected override void ExecuteAttack()
    {
        if (!IsTargetInRange()) return;

        for (int i = 0; i < projectilesPerShot; i++)
        {
            // 生成豌豆
            PeaProjectile pea = Instantiate(
                peaPrefab,
                shootPoints.position,
                shootPoints.rotation
            );

            // 初始化参数
            pea.Initialize(
                shootPoints.TransformDirection(Vector3.right),
                Mathf.RoundToInt(damage)
            );
        }
    }

    public override void UpgradeWeapon()
    {
        projectilesPerShot += 1;
    }

    public override void UpdateTarget()
    {
        base.UpdateTarget();
    }
    public override bool IsTargetInRange()
    {
        return base.IsTargetInRange();
    }

    public override bool HasValidTarget()
    {
        return base.HasValidTarget();
    }

    public override void RotateBarrel()
    {
        if (!HasValidTarget())
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        Vector2 direction = (_currentTarget.transform.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float scaleSign = Mathf.Sign(transform.localScale.x);
        if (scaleSign < 0)
        {
            direction.x *= -1;
            targetAngle = targetAngle + 180f;

        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}