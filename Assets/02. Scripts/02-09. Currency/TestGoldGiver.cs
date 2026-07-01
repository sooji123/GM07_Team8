using TMPro;
using UnityEngine;

public class TestGoldGiver : MonoBehaviour
{
    [Header("골드량")]
    [SerializeField] private int gold = 10;

    public void OnClickGiveGold()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(gold);
        }
        else
        {
            Debug.LogError("currencyManager 없어용 넣으세용");
        }
    }
}
