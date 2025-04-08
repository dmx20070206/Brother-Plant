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
    public GameObject gameStartUI;
    public GameObject progressUI;

    public GameObject UICanvas;

    public HealthUI _health_ui;
    public SunUI _sun_ui;
    public GameStartUI _gameStart_ui;
    public ProgressUI _progress_ui;

    private void Awake()
    {
        GameManager.Instance.RegisterUIController(this);
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        Initialize();
    }

    public void Initialize()
    {
        _health_ui = Instantiate(healthUI, UICanvas.transform).GetComponent<HealthUI>();
        _sun_ui = Instantiate(sunUI, UICanvas.transform).GetComponent<SunUI>();
        _gameStart_ui = Instantiate(gameStartUI, UICanvas.transform).GetComponent<GameStartUI>();
        _progress_ui = Instantiate(progressUI, UICanvas.transform).GetComponent<ProgressUI>();
        _health_ui.name = "Health";
        _sun_ui.name = "Sun";
        _gameStart_ui.name = "GameStart";
        _progress_ui.name = "Progress";

        ShowGameStartPrompt();
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

    public void ShowGameStartPrompt()
    {
        _gameStart_ui.StartPlay();
    }

    public void OnCountdownEnd()
    {
        GameManager.Instance.LevelController.ChangeLevel();
    }
}
