using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SunUI : MonoBehaviour
{
    [Header("�����")]
    [SerializeField] private Image sunImage;       // ����ͼ��
    [SerializeField] private TMP_Text sunText;     // �����ı�

    [Header("��������")]
    [SerializeField] private float rotationSpeed = 90f;     // ͼ����ת�ٶȣ���/�룩
    [SerializeField] private float updateInterval = 0.3f;   // ��ֵ���¼��

    [Header("��������")]
    [SerializeField] private Vector2 positionOffset = new Vector2(100, -250); // ������Ͻ�ƫ��

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
        _rectTransform.anchorMin = new Vector2(0, 1);  // ���Ͻ�ê��
        _rectTransform.anchorMax = new Vector2(0, 1);
        _rectTransform.pivot = new Vector2(0, 1);     // �������ϽǶ�λ
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
        // �򵥵����Ŷ���
        LeanTween.cancel(sunText.gameObject);
        LeanTween.scale(sunText.gameObject, Vector3.one * 1.2f, 0.15f)
            .setEase(LeanTweenType.easeOutQuad)
            .setLoopPingPong(1);
    }
}