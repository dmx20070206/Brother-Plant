using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("攻击设置")]
    [SerializeField] protected float attackInterval = 0.5f;       // 攻击间隔
    [SerializeField] protected float initialDamageDelay = 0.1f;   // 首次伤害延迟（配合动画）

    [Header("受伤特效")]
    [SerializeField] protected Color hitColor = Color.red;        // 受伤颜色
    [SerializeField] protected float flashDuration = 0.1f;        // 颜色变化持续时间

    // 自身组件
    protected Animator animator;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;

    // 自身数据
    protected EnemyStats _stats;

    // 辅助数据
    protected Player _player;

    public abstract void StartAttack();
    public abstract void EndAttack();
    public abstract void ApplyDamage();
    public abstract CharacterStats.DamageResult CalculateDamage(float damage);
    public abstract void TakeDamage(CharacterStats.DamageResult damage, Transform transform);
}
