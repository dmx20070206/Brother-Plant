using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum GameScene
    {
        Menu,   // 主菜单
        Game,   // 游戏场景
        Shop    // 商店场景
    }

    private void Awake()
    {
        GameManager.Instance.RegisterSceneController(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterSceneController();
    }

    public void LoadScene(GameScene scene)
    {
        StartCoroutine(LoadSceneAsync(scene));
    }

    private IEnumerator LoadSceneAsync(GameScene scene)
    {
        BeforeSceneLoad();

        AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(scene.ToString());
        sceneLoadOp.allowSceneActivation = false;

        while (sceneLoadOp.progress < 0.9f)
        {
            yield return null;
        }

        sceneLoadOp.allowSceneActivation = true;

        while (!sceneLoadOp.isDone)
        {
            yield return null;
        }

        AfterSceneLoad(scene);
    }

    private void AfterSceneLoad(GameScene scene)
    {
        Time.timeScale = 1f;
        switch (scene)
        {
            case GameScene.Menu:
                GameManager.Instance.EnterMenu();
                break;
            case GameScene.Game:
                GameManager.Instance.EnterGame();
                break;
            case GameScene.Shop:
                GameManager.Instance.EnterShop();
                break;
        }
    }

    private void BeforeSceneLoad()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        Time.timeScale = 1f;
        switch (currentScene.name)
        {
            case "Menu":
                GameManager.Instance.ExitMenu();
                break;
            case "Game":
                GameManager.Instance.ExitGame();
                break;
            case "Shop":
                GameManager.Instance.ExitShop();
                break;
        }
    }

}
