using UnityEngine;

public class PeaProjectile : MonoBehaviour
{
    [Header("飞行设置")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 10;

    [Header("视觉效果")]
    [SerializeField] private GameObject hitEffect;

    private Vector2 _direction;
    private float _spawnTime;

    public void Initialize(Vector2 direction, int baseDamage)
    {
        _direction = direction;
        damage = baseDamage;

        _spawnTime = Time.time;
    }

    private void Update()
    {
        // 移动逻辑
        transform.position += (Vector3)_direction * speed * Time.deltaTime;

        // 生命周期检测
        if (Time.time > _spawnTime + lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 造成伤害
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                var result = GameManager.Instance.Player._stats.CalculateDamage(damage);
                result.type = EnemyStatusSystem.StatusType.Normal;
                enemy.TakeDamage(result, transform);
            }

            // 播放命中特效
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}