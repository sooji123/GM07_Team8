using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextUI : MonoBehaviour
{ 
    [Header("플로팅 텍스트 세팅")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private RectTransform ParentCanvas;

    [Header("플로팅 텍스트 지속 시간")]
    [SerializeField] private float floatingDuration = 0.8f; // 텍스트 부유 시간

    public void TextSetting(string text, Color color)
    {
        TMP_Text tmp = GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.color = color;

        transform.DOMoveY(transform.position.y + 1.0f, floatingDuration).SetEase(Ease.OutCubic);
        tmp.DOFade(0f, floatingDuration).OnComplete(() => Destroy(gameObject));
    }

    public void SpawnFloatingText(string text, Color color, Vector2 localPosition)
    {
        GameObject obj = Instantiate(floatingTextPrefab, ParentCanvas);
        RectTransform objRect = obj.GetComponent<RectTransform>();

        objRect.anchoredPosition = localPosition;
        TextSetting(text, color);
    }

    public void SpawnFloatingText(string text, Color color, Vector3 worldPosition)
    {
        GameObject obj = Instantiate(floatingTextPrefab);

        obj.transform.position = worldPosition;
        TextSetting(text, color);
    }
}