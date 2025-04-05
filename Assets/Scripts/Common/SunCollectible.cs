using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCollectible : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 30f;

    [Header("�ƶ�����")]
    [SerializeField] private float attractRange = 10f;       // ������Χ
    [SerializeField] private float baseSpeed = 1f;           // �����ƶ��ٶ�
    [SerializeField] private float acceleration = 1f;        // ���ٶ�
    [SerializeField] private float minDistance = 0.1f;       // ��С���پ���

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
            // ����������Χ
            StartAttraction();
        }
        else if (_isAttracted && distance <= minDistance)
        {
            // �ﵽ���پ���
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

        // �����ƶ�����
        Vector3 direction = (_player.position - transform.position).normalized;

        // Ӧ�ü���Ч��
        _currentSpeed += acceleration * Time.fixedDeltaTime;

        // �ƶ�λ��
        transform.position += direction * _currentSpeed * Time.fixedDeltaTime;
    }
}
