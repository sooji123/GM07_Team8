using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextUI : MonoBehaviour
{
    [SerializeField] private float floatingDuration = 0.8f; // 텍스트가 부유 시간
    public void TextSetting(string text, Color color)
    {
        TMP_Text tmp = GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.color = color;

        transform.DOMoveY(transform.position.y + 1.0f, floatingDuration).SetEase(Ease.OutCubic);
        tmp.DOFade(0f, floatingDuration).OnComplete(() => Destroy(gameObject));
    }
}