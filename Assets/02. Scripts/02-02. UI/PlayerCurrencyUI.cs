using TMPro;
using UnityEngine;

public class PlayerCurrencyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    [SerializeField]
    private TextMeshProUGUI waterText;

    [SerializeField]
    private TextMeshProUGUI fireText;

    [SerializeField]
    private TextMeshProUGUI grassText;

    [SerializeField]
    private TextMeshProUGUI electricText;

    private void Update()
    {
        if (CurrencyManager.Instance == null)
        {
            return;
        }

        if (CurrencyManager.Instance != null && goldText != null)
        {
            goldText.text = $"{CurrencyManager.Instance.gold}";
        }

        if (CurrencyManager.Instance != null && waterText != null)
        {
            waterText.text = $"{CurrencyManager.Instance.GetElementOrbs(EElement.Water)}";
        }

        if (CurrencyManager.Instance != null && fireText != null)
        {
            fireText.text = $"{CurrencyManager.Instance.GetElementOrbs(EElement.Fire)}";
        }

        if (CurrencyManager.Instance != null && grassText != null)
        {
            grassText.text = $"{CurrencyManager.Instance.GetElementOrbs(EElement.Grass)}";
        }

        if (CurrencyManager.Instance != null && electricText != null)
        {
            electricText.text = $"{CurrencyManager.Instance.GetElementOrbs(EElement.Electric)}";
        }
    }
}
