using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [Header("飞行设置")]
    [SerializeField] private int damage = 10;

    [Header("视觉效果")]
    [SerializeField] private GameObject hitEffect;

    private Dictionary<GameObject, bool> dic = new Dictionary<GameObject, bool>();
    public void Initialize(Vector2 direction, int baseDamage)
    {
        damage = baseDamage;
    }

    public void Damage()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dic.ContainsKey(other.gameObject))
            return;
        dic.Add(other.gameObject, true);

        if (other.CompareTag("Enemy"))
        {
            // 造成伤害
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                var result = GameManager.Instance.Player._stats.CalculateDamage(damage);
                result.type = EnemyStatusSystem.StatusType.Burn;
                enemy.TakeDamage(result, transform);
            }

            // 播放命中特效
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            EnemyStatusSystem.StatusType type = EnemyStatusSystem.StatusType.Burn;
            other.GetComponent<EnemyStatusSystem>().ApplyStatus(type, 1f);
        }
    }
}
