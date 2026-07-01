using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextUI : MonoBehaviour
{ 
    [Header("플로팅 텍스트 세팅")]
    [SerializeField] private GameObject floatingTextPrefab;

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
}