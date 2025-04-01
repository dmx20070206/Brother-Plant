using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] protected float attackInterval = 0.5f;       // �������
    [SerializeField] protected float initialDamageDelay = 0.1f;   // �״��˺��ӳ٣���϶�����

    [Header("������Ч")]
    [SerializeField] protected Color hitColor = Color.red;        // ������ɫ
    [SerializeField] protected float flashDuration = 0.1f;        // ��ɫ�仯����ʱ��

    // �������
    protected Animator animator;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;

    // ��������
    protected EnemyStats _stats;

    // ��������
    protected Player _player;

    public abstract void StartAttack();
    public abstract void EndAttack();
    public abstract void ApplyDamage();
    public abstract CharacterStats.DamageResult CalculateDamage(float damage);
    public abstract void TakeDamage(CharacterStats.DamageResult damage, Transform transform);
}
