using TMPro;
using UnityEngine;
using DG.Tweening;

public class OrbUI : MonoBehaviour
{
    [Header("UI 텍스트 세팅")]
    [SerializeField] private TextMeshProUGUI waterText;
    [SerializeField] private TextMeshProUGUI fireText;
    [SerializeField] private TextMeshProUGUI grassText;
    [SerializeField] private TextMeshProUGUI electricText;

    [Header("UI/포지션/캔버스")]
    [SerializeField] private FloatingTextUI floatingTextUI;
    public FloatingTextPosition floatingTextPosition;
    public RectTransform parentCanvas;

    private void OnEnable()
    {
        CurrencyManager.Instance.OnOrbChanged += UpdateOrbUI;
        CurrencyManager.Instance.OnOrbEarned += PlayOrbEarnEffect;
        CurrencyManager.Instance.OnOrbSpent += PlayOrbSpendEffect;
        CurrencyManager.Instance.OnOrbNotEnough += PlayOrbNotEnoughEffect;
        Debug.Log("오브 UI 이벤트 연결 완료");
    }
    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnOrbChanged -= UpdateOrbUI;
            CurrencyManager.Instance.OnOrbEarned -= PlayOrbEarnEffect;
            CurrencyManager.Instance.OnOrbSpent -= PlayOrbSpendEffect;
            CurrencyManager.Instance.OnOrbNotEnough -= PlayOrbNotEnoughEffect;
        }
    }

    public void UpdateOrbUI(EElement element, int orb)
    {
        TextMeshProUGUI targetText = GetTextByElement(element);
        if (targetText == null)
        {
            return;
        }

        targetText.transform.DOComplete();
        targetText.transform.DOPunchPosition(Vector3.up * 5f, 0.2f, 1, 0.5f);

        targetText.text = orb.ToString("N0");
    }

    public void PlayOrbEarnEffect(EElement element, int amount)
    {
        TextMeshProUGUI targetText = GetTextByElement(element);
        if (targetText == null)
        {
            return;
        }

        Color effectColor = GetColorByElement(element);
        targetText.DOColor(effectColor, 0.1f).OnComplete(() => targetText.DOColor(Color.white, 0.1f));

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas,
            Camera.main.WorldToScreenPoint(targetText.transform.position),
            Camera.main,
            out localPos
        );

        floatingTextPosition.SpawnFloatingTextOnCanvas("+" + amount, effectColor, localPos);
    }

    public void PlayOrbSpendEffect(EElement element, int amount)
    {
        TextMeshProUGUI targetText = GetTextByElement(element);
        if (targetText == null)
        {
            return;
        }

        Color effectColor = GetColorByElement(element);
        targetText.DOColor(effectColor, 0.1f).OnComplete(() => targetText.DOColor(Color.white, 0.1f));

        floatingTextPosition.SpawnFloatingTextOnMouse("-" + amount, effectColor, Input.mousePosition);
    }

    public void PlayOrbNotEnoughEffect(EElement element)
    {
        TextMeshProUGUI targetText = GetTextByElement(element);
        if (targetText == null)
        {
            return;
        }

        targetText.DOColor(Color.red, 0.1f).SetLoops(4, LoopType.Yoyo)
            .OnComplete(() => targetText.DOColor(Color.white, 0.1f));

        targetText.transform.DOShakePosition(0.3f, new Vector3(10f, 0, 0), 10, 0);
        floatingTextPosition.SpawnFloatingTextOnMouse("Broke!", Color.red, Input.mousePosition);
    }

    private TextMeshProUGUI GetTextByElement(EElement element)
    {
        return element switch
        {
            EElement.Water => waterText,
            EElement.Fire => fireText,
            EElement.Grass => grassText,
            EElement.Electric => electricText,
            EElement.None => null
        };
    }
    private Color GetColorByElement(EElement element)
    {
        return element switch
        {
            EElement.Water => Color.cyan,
            EElement.Fire => Color.orange,
            EElement.Grass => Color.green,
            EElement.Electric => Color.yellow,
            EElement.None => Color.white
        };
    }
}
