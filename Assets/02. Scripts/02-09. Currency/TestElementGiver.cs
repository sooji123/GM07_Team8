using UnityEngine;

public class TestElementGiver : MonoBehaviour
{
    [Header("속성")]
    [SerializeField] private int element = 10;

    public void OnClickGiveElement()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddElementOrbs(EElement.Water, element);
            CurrencyManager.Instance.AddElementOrbs(EElement.Fire, element);
            CurrencyManager.Instance.AddElementOrbs(EElement.Grass, element);
            CurrencyManager.Instance.AddElementOrbs(EElement.Electric, element);
        }
        else
        {
            Debug.LogError("currencyManager 없어용 넣으세용");
        }
    }
}
