using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] private Image _progressImage;
    [SerializeField] private Image _posImage;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _time;

    [SerializeField] private float totalTime;
    [SerializeField] private GameObject flag;

    [Header("旗子设置")]
    [SerializeField] private List<float> flagPos;
    [SerializeField] private List<FlagUI> flags;

    [Header("位置设置")]
    [SerializeField] private RectTransform startPos;
    [SerializeField] private RectTransform endPos;

    private float _currentTime;
    private bool _isProgressing;

    private float _lastUpdateTime;
    private int _lastDisplaySeconds;

    void Start()
    {
        InitializeProgress();
    }

    void Update()
    {
        if (!_isProgressing) return;

        UpdateProgress();
        UpdatePosition();
        UpdateText();
        CheckFlags();
    }

    private void InitializeProgress()
    {
        _progressImage.fillAmount = 1f;
        _currentTime = 0f;
        _isProgressing = true;

        _level.text = "Wave " + GameManager.Instance.LevelController.CurLevel;

        flagPos = new List<float> { Random.Range(0.3f, 0.5f), Random.Range(0.7f, 0.9f) };
        foreach (var item in flagPos)
        {
            Vector2 newPos = Vector2.Lerp(startPos.position, endPos.position, item);
            GameObject obj = Instantiate(flag, transform);
            obj.transform.position = newPos;
            flags.Add(obj.GetComponent<FlagUI>());
        }
    }

    #region Updata process, position and text
    private void UpdateProgress()
    {
        if (_currentTime <= totalTime)
            _currentTime += Time.deltaTime;
        else GameManager.Instance.UIController.OnCountdownEnd();
        _progressImage.fillAmount = 1 - Mathf.Clamp01(_currentTime / totalTime);
    }
    private void UpdatePosition()
    {
        float progress = _currentTime / totalTime;

        float adjustedProgress = Mathf.SmoothStep(0, 1, progress);

        // 计算插值位置
        Vector2 newPos = Vector2.Lerp(startPos.position, endPos.position, adjustedProgress);
        _posImage.transform.position = newPos;
    }
    private void UpdateText()
    {
        float remaining = totalTime - _currentTime;
        int displaySeconds = Mathf.CeilToInt(remaining);

        if (displaySeconds != _lastDisplaySeconds || Time.time - _lastUpdateTime >= 0.5f)
        {
            _time.text = displaySeconds.ToString();
            _lastDisplaySeconds = displaySeconds;
            _lastUpdateTime = Time.time;
        }
    }
    #endregion

    int curIndex = -1;
    private void CheckFlags()
    {
        for (int i = curIndex + 1; i < flags.Count; i++)
        {
            float flagTime = flagPos[i] * totalTime;
            if (_currentTime >= flagTime)
            {
                if (flags.Count > i)
                {
                    curIndex = i;
                    flags[i].RiseUp();
                }
            }
        }
    }

    #region start pause and reset
    public void StartProgress()
    {
        _isProgressing = true;
    }

    public void PauseProgress()
    {
        _isProgressing = false;
    }

    public void ResetProgress()
    {
        InitializeProgress();
    }
    #endregion
}