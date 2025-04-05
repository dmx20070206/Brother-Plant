using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject critEffectPrefab;
    public GameObject damagePopupPrefab;
    public GameObject healthUI;
    public GameObject sunUI;

    public GameObject UICanvas;

    public HealthUI healthBar;
    public SunUI sunCount;

    private void Awake()
    {
        GameManager.Instance.RegisterUIController(this);
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        Initialize();
    }

    public void Initialize()
    {
        healthBar = Instantiate(healthUI, UICanvas.transform).GetComponent<HealthUI>();
        sunCount = Instantiate(sunUI, UICanvas.transform).GetComponent<SunUI>();
        healthBar.name = "HealthBar";
        sunCount.name = "SunCount";
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterUIController();
    }

    public void ShowDamageFeedback(CharacterStats.DamageResult result, Transform transform)
    {
        Color targetColor = result.type == EnemyStatusSystem.StatusType.Frozen || result.type == EnemyStatusSystem.StatusType.Slow ?
            Color.blue : result.type == EnemyStatusSystem.StatusType.Burn ?
            Color.red : result.type == EnemyStatusSystem.StatusType.Poison ?
            Color.green : Color.white;

        DamagePopup.Create(
            transform.position,
            result.finalDamage.ToString("F0"),
            targetColor,
            isCrit: result.isCrit
        );

        if (result.isCrit)
        {
            GameObject instance = Instantiate(critEffectPrefab, transform.position, Quaternion.identity);
            GameObject worldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas");
            if (worldCanvas != null)
            {
                instance.transform.SetParent(worldCanvas.transform, false);
            }
        }
    }
}
