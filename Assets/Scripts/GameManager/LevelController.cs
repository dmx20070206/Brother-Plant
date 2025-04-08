using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public int curLevel;
    public List<EnemySpawnData> curEnemySpawnDatas = new List<EnemySpawnData>();

    private void Awake()
    {
        GameManager.Instance.RegisterLevelController(this);
        ChangeLevel(0);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterLevelController();
    }

    public int CurLevel
    {
        get { return curLevel; }
    }

    public void ChangeLevel(int level = -1)
    {
        if (level == -1) curLevel++;
        else curLevel = level;

        curEnemySpawnDatas.Clear();
        curEnemySpawnDatas = GameManager.Instance.EnemySpawnDataBase.GetEnemySpawnDataByLevel(curLevel);

        if (curLevel != 0)
            GameManager.Instance.SceneController.LoadScene(SceneController.GameScene.Shop);
    }
}
