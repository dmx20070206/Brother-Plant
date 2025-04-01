using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject critEffectPrefab;
    public GameObject damagePopupPrefab;
    public GameObject healthUI;

    public GameObject UICanvas;

    public HealthUI healthBar;

    private void Awake()
    {
        GameManager.Instance.RegisterUIController(this);
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        Initialize();
    }

    public void Initialize()
    {
        healthBar = Instantiate(healthUI, UICanvas.transform).GetComponent<HealthUI>();
        healthBar.name = "HealthBar";
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterUIController();
    }

    public void ShowDamageFeedback(CharacterStats.DamageResult result, Transform transform)
    {
        DamagePopup.Create(
            transform.position,
            result.finalDamage.ToString("F0"),
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
