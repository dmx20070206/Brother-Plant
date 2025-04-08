using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterGame : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(MenuToGame);
    }

    private void MenuToGame()
    {
        _button.interactable = false;
        GameManager.Instance.SceneController.LoadScene(SceneController.GameScene.Game);
    }
}
