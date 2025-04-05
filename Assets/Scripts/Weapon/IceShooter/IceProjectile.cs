using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 10;

    [Header("�Ӿ�Ч��")]
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
        // �ƶ��߼�
        transform.position += (Vector3)_direction * speed * Time.deltaTime;

        // �������ڼ��
        if (Time.time > _spawnTime + lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ����˺�
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                var result = GameManager.Instance.Player._stats.CalculateDamage(damage);
                result.type = EnemyStatusSystem.StatusType.Slow;
                enemy.TakeDamage(result, transform);
            }

            // ����������Ч
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            EnemyStatusSystem.StatusType type = UnityEngine.Random.value < 0.1f ? EnemyStatusSystem.StatusType.Frozen : EnemyStatusSystem.StatusType.Slow;
            other.GetComponent<EnemyStatusSystem>().ApplyStatus(type, 0.5f);
            
            Destroy(gameObject);
        }
    }
}
