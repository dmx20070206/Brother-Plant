using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 1, 0);

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, fadeDuration);
    }

    public static void Create(Vector3 position, string text, bool isCrit = false)
    {
        GameObject instance = Instantiate(GameManager.Instance.UIController.damagePopupPrefab, position, Quaternion.identity);

        GameObject worldCanvas = GameObject.FindGameObjectWithTag("WorldCanvas");
        if (worldCanvas != null)
        {
            instance.transform.SetParent(worldCanvas.transform, false);
        }

        DamagePopup popup = instance.GetComponent<DamagePopup>();
        popup.Setup(text, isCrit);
    }

    private void Setup(string text, bool isCrit)
    {
        textMesh.text = text;
        textMesh.color = isCrit ? Color.yellow : Color.white;
        textMesh.fontSize = isCrit ? 0.8f : 0.4f;

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveOffset;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float progress = t / fadeDuration;
            transform.position = Vector3.Lerp(startPos, endPos, progress);
            textMesh.alpha = Mathf.Lerp(1, 0, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}