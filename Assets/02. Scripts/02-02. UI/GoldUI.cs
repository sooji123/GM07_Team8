using DG.Tweening;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [Header("UI 텍스트 세팅")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private GameObject floatingTextPrefab;

    [SerializeField] private FloatingTextUI floatingTextUI;

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

        Vector3 spawnPos = goldText.transform.position; // goldText 위치에서 플로팅 텍스트 생성
        floatingTextUI.SpawnFloatingText("+" + amount.ToString("N0"), Color.green, spawnPos);
    }

    private void PlayGoldSpendEffect(int amount)
    {
        goldText.DOColor(Color.orangeRed, 0.1f).OnComplete(() => goldText.DOColor(Color.white, 0.1f));

        
    }

    private void PlayGoldNotEnoughEffect()
    {

    }
}
