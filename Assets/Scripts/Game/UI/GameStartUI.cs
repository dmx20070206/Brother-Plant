using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.speed = 0;
        gameObject.SetActive(false);
    }

    public void PausePlay()
    {
        _animator.speed = 0f;
        gameObject.SetActive(false);
    }

    public void StartPlay()
    {
        gameObject.SetActive(true);
        _animator.speed = 1f;
    }

    public void Hide()
    {
        PausePlay();
        _animator.SetTrigger("Reset");
        gameObject.SetActive(false);
    }
}
