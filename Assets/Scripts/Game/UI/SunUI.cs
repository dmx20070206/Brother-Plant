using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SunUI : MonoBehaviour
{
    [Header("组件绑定")]
    [SerializeField] private Image sunImage;       // 阳光图标
    [SerializeField] private TMP_Text sunText;     // 数量文本

    [Header("动画设置")]
    [SerializeField] private float rotationSpeed = 90f;     // 图标旋转速度（度/秒）
    [SerializeField] private float updateInterval = 0.3f;   // 数值更新间隔

    [Header("布局设置")]
    [SerializeField] private Vector2 positionOffset = new Vector2(100, -250); // 相对左上角偏移

    private RectTransform _rectTransform;
    private float _lastUpdateTime;
    private int _lastSunValue = -1;

    private void Reset()
    {
        sunImage = GetComponentInChildren<Image>();
        sunText = GetComponentInChildren<TMP_Text>();
    }

    private void Awake()
    {
        InitializeRectTransform();
    }

    private void InitializeRectTransform()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchorMin = new Vector2(0, 1);  // 左上角锚点
        _rectTransform.anchorMax = new Vector2(0, 1);
        _rectTransform.pivot = new Vector2(0, 1);     // 基于左上角定位
    }

    private void Update()
    {
        RotateSunImage();
        CheckSunUpdate();
    }

    private void RotateSunImage()
    {
        if (sunImage != null)
        {
            sunImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private void CheckSunUpdate()
    {
        if (Time.time - _lastUpdateTime > updateInterval)
        {
            UpdateSunValue();
            _lastUpdateTime = Time.time;
        }
    }

    private void UpdateSunValue()
    {
        if (!Application.isPlaying) return;

        int currentSun = (int)GameManager.Instance.SunController.GetSunshineTotal();
        if (currentSun != _lastSunValue)
        {
            UpdateDisplay(currentSun);
            _lastSunValue = currentSun;
        }
    }

    private void UpdateDisplay(int value)
    {
        if (sunText != null)
        {
            sunText.text = value.ToString();
            PlayScaleAnimation();
        }
    }

    private void PlayScaleAnimation()
    {
        // 简单的缩放动画
        LeanTween.cancel(sunText.gameObject);
        LeanTween.scale(sunText.gameObject, Vector3.one * 1.2f, 0.15f)
            .setEase(LeanTweenType.easeOutQuad)
            .setLoopPingPong(1);
    }
}