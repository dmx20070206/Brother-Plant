using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public PlayerStats stats;

    [Header("受伤特效")]
    [SerializeField] private Color hitColor = Color.red;        // 受伤颜色
    [SerializeField] private float flashDuration = 0.1f;        // 颜色变化持续时间


    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private Coroutine _flashRoutine;

    #region Life Cycle
    void Awake()
    {
        GameManager.Instance.RegisterPlayer(this);

        stats = new PlayerStats();
        stats.Initialize("DMX");

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    void Update()
    {
        stats.UpdateStats(Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterPlayer();
    }
    #endregion

    #region Receive Attack
    public void TakeDamage(CharacterStats.DamageResult damage, Transform transform)
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

        // 掉血逻辑
        stats.TakeDamage(damage);
    }
    #endregion

    #region Effect
    private IEnumerator HitEffectRoutine()
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
