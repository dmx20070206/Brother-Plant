using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningVFX : MonoBehaviour
{
    private Animator _animator;
    private float _destroyDelay;

    void Start()
    {
        InitializeComponents();
        StartCoroutine(DestroyAfterAnimation());
    }

    private void InitializeComponents()
    {
        _animator = GetComponent<Animator>();

        if (_animator != null && _animator.GetCurrentAnimatorStateInfo(0).length > 0) 
        {
            _destroyDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // 等待动画时长 + 一帧时间确保动画完成
        yield return new WaitForSeconds(_destroyDelay + Time.deltaTime);

        // 安全销毁检查
        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
