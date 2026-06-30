using DG.Tweening;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [Header("UI 텍스트 세팅")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("UI/포지션/캔버스")]
    [SerializeField] private FloatingTextUI floatingTextUI;
    public FloatingTextPosition floatingTextPosition;
    public RectTransform parentCanvas;

    private void OnEnable()
    {
        CurrencyManager.Instance.OnGoldChanged += UpdateGoldUI;
        CurrencyManager.Instance.OnGoldEarned += PlayGoldEarnEffect;
        CurrencyManager.Instance.OnGoldSpent += PlayGoldSpendEffect;
        CurrencyManager.Instance.OnGoldNotEnough += PlayGoldNotEnoughEffect;
        Debug.Log("골드 UI 이벤트 연결 완료");
    }
    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        { 
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldUI;
            CurrencyManager.Instance.OnGoldEarned -= PlayGoldEarnEffect;
            CurrencyManager.Instance.OnGoldSpent -= PlayGoldSpendEffect;
            CurrencyManager.Instance.OnGoldNotEnough -= PlayGoldNotEnoughEffect;
        }
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.transform.DOComplete(); // 애니메이션 겹치는거 방지
        goldText.transform.DOPunchPosition(Vector3.up * 5f, 0.2f, 1, 0.5f);

        goldText.text = gold.ToString("N0"); // 천 단위 구분 쉼표 넣는거(아마 안 넘겠지만 기능이 있길래..)
    }

    private void PlayGoldEarnEffect(int amount)
    {
        goldText.DOColor(Color.gold, 0.1f).OnComplete(() => goldText.DOColor(Color.white, 0.1f));

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas,
            Camera.main.WorldToScreenPoint(goldText.transform.position),
            Camera.main, out localPos );

        floatingTextPosition.SpawnFloatingTextOnCanvas("+" + amount, Color.green, localPos);
    }

    private void PlayGoldSpendEffect(int amount)
    {
        goldText.DOColor(Color.orangeRed, 0.1f).OnComplete(() => goldText.DOColor(Color.white, 0.1f));

        floatingTextPosition.SpawnFloatingTextOnMouse("-" + amount, Color.red, Input.mousePosition);
    }

    private void PlayGoldNotEnoughEffect()
    {
        goldText.DOColor(Color.red, 0.1f).SetLoops(4, LoopType.Yoyo)
            .OnComplete(() => goldText.DOColor(Color.white, 0.1f));

        goldText.transform.DOShakePosition(0.3f, new Vector3(10f, 0, 0), 10, 0);

        floatingTextPosition.SpawnFloatingTextOnMouse("Broke!", Color.red, Input.mousePosition);
    }
}
