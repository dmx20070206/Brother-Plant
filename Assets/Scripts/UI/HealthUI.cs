using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public Image healthPointImage;
    public Image healthPointEffect;

    private Player player;
    [SerializeField] private float hurtSpeed = 0.003f;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        healthPointImage.fillAmount = player.stats.currentHealth / player.stats.maxHealth.Value;

        if (healthPointEffect.fillAmount >= healthPointImage.fillAmount)
        {
            healthPointEffect.fillAmount -= hurtSpeed;
        }
        else
        {
            healthPointEffect.fillAmount = healthPointImage.fillAmount;
        }
    }

}
