using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[SerializeField]
public class GameDataController : MonoBehaviour
{
    public PlayerStats PlayerData { get; private set; }
    public List<string> WeaponDatas { get; private set; }

    public int SunlightCount { get; private set; }
    public int CurrentLevel { get; private set; }

    private string savePath;

    private void Awake()
    {
        GameManager.Instance.RegisterGameDataController(this);

        savePath = Path.Combine(Application.persistentDataPath, "gameSave.dat");

        // 初始武器
        PlayerData = CharacterStats.Attribute_to_PlayerStats("DMX");
        WeaponDatas = new List<string> { "PeaShooter" };
    }

    void OnDestroy()
    {
        GameManager.Instance.UnregisterGameDataController();
    }

    public void SaveData()
    {
        ClearData();

        PlayerData = GameManager.Instance.Player._stats;
        foreach (var item in GameManager.Instance.WeaponManager._weapons)
            WeaponDatas.Add(item.name);
        SunlightCount = (int)GameManager.Instance.SunController.GetSunshineTotal();
        CurrentLevel = GameManager.Instance.LevelController.CurLevel;
    }

    public void ClearData()
    {
        PlayerData = null;
        WeaponDatas.Clear();
        SunlightCount = CurrentLevel = 0;
    }
}
