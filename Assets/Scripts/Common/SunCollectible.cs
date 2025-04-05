using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCollectible : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 30f;

    [Header("移动设置")]
    [SerializeField] private float attractRange = 10f;       // 吸引范围
    [SerializeField] private float baseSpeed = 1f;           // 基础移动速度
    [SerializeField] private float acceleration = 1f;        // 加速度
    [SerializeField] private float minDistance = 0.1f;       // 最小销毁距离

    private int value;
    private float size;

    private Transform _player;
    private bool _isAttracted;
    private float _currentSpeed;

    public void Initialize(int val, float s)
    {
        value = val;
        size = s;
        transform.localScale = Vector3.one * size;

        _player = GameManager.Instance.Player.transform;
        _currentSpeed = baseSpeed;
    }

    void Update()
    {
        HandleRotation();
        CheckPlayerDistance();
        if (_isAttracted) MoveTowardsPlayer();
    }

    public void CollectSun()
    {
        GameManager.Instance.SunController.AddSun(value);
        Destroy(gameObject);
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    private void CheckPlayerDistance()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= attractRange && !_isAttracted)
        {
            // 进入吸引范围
            StartAttraction();
        }
        else if (_isAttracted && distance <= minDistance)
        {
            // 达到销毁距离
            CollectSun();
        }
    }

    private void StartAttraction()
    {
        _isAttracted = true;
    }

    private void MoveTowardsPlayer()
    {
        if (_player == null) return;

        // 计算移动方向
        Vector3 direction = (_player.position - transform.position).normalized;

        // 应用加速效果
        _currentSpeed += acceleration * Time.fixedDeltaTime;

        // 移动位置
        transform.position += direction * _currentSpeed * Time.fixedDeltaTime;
    }
}
