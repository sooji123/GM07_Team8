using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private ETurretType _turretType;
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [Header("Button")]
    [SerializeField]
    private Button _upgrade2Btn;
    [SerializeField]
    private Button _upgrade3Btn;
    [Header("Shop")]
    [SerializeField]
    private TextMeshProUGUI _shopLevelText;
    [SerializeField]
    private Image _fillImage;
    [SerializeField]
    private GameObject _lockImage;
    [SerializeField]
    private RectTransform _lockRect;

    private int _level = 1;


    private void Start()
    {
        if(_upgrade2Btn != null && _upgrade3Btn != null && _levelText !=null)
        {
            _upgrade2Btn.onClick.AddListener(()=>OnClickUpgradeBtn(2));
            _upgrade3Btn.onClick.AddListener(()=>OnClickUpgradeBtn(3));
            _level = 1;
            _levelText.text = _level.ToString();
            _fillImage.fillAmount = 0.33f;
        }
    }

    private void OnClickUpgradeBtn(int level)
    {
        if(UpgradeManager.Instance == null || SoundManager.Instance == null)
        {
            return;
        }

        if (_level >= 3 || level != _level+1)
        {
            return;
        }

        int cost = (_level == 1) ? 30 : 40;
        Button curretButton = (level == 2) ? _upgrade2Btn : _upgrade3Btn;

        if (CurrencyManager.Instance.GetElementOrbs(EElement.Fire) >= cost &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Water) >= cost &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Grass) >= cost &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Electric) >= cost)
        {
            CurrencyManager.Instance.UseElementOrbs(EElement.Fire, cost);
            CurrencyManager.Instance.UseElementOrbs(EElement.Water, cost);
            CurrencyManager.Instance.UseElementOrbs(EElement.Grass, cost);
            CurrencyManager.Instance.UseElementOrbs(EElement.Electric, cost);

            SoundManager.Instance.PlayeSFX(ESFXType.Upgrade);
            UpgradeManager.Instance.UpgradeTurretType(_turretType);

            UpdateTurretUI();
            UpdateSlotUI();
        }
        else
        {
            SoundManager.Instance.PlayeSFX(ESFXType.Upgrade_fail);

            if (curretButton.TryGetComponent<RectTransform>(out RectTransform upgradeBtnRect))
            {
                upgradeBtnRect.DOKill();
                upgradeBtnRect.DOShakeAnchorPos(0.25f, Vector3.right * 15f, 30, 90f, false, true);
            }
        }
    }

    private void UpdateSlotUI()
    {
        if(UpgradeManager.Instance == null)
        {
            return;
        }

        _level = UpgradeManager.Instance.GetUpgradeLevel(_turretType);
        _shopLevelText.text = $"Lvl {_level}";

        switch(_level)
        {
            case 1:
                _upgrade2Btn.interactable = true;
                _upgrade3Btn.interactable = false;
                _lockImage.SetActive(true);
                break;
            case 2:
                _upgrade2Btn.interactable = false;
                _upgrade3Btn.interactable = true;

                _lockRect.DOKill();
                _lockRect.DOShakeAnchorPos(0.5f, Vector3.right * 10f, 20, 90f, false, true)
                    .OnComplete(() =>
                    {
                    _lockRect.DOAnchorPosY(-100f, 0.3f).SetEase(Ease.InBack)
                        .OnComplete(() => _lockImage.SetActive(false));
                    });
                break;
            case 3:
                _upgrade2Btn.interactable = false;
                _upgrade3Btn.interactable = false;
                break;
            default:
                break;
        }

        if(_fillImage != null)
        {
            _fillImage.DOKill();

            float amount = _level switch
            {
                1 => 0.33f,
                2 => 0.66f,
                3 => 1.0f,
                _ => 0f
            };
            _fillImage.DOFillAmount(amount, 0.5f).SetEase(Ease.OutCubic);
        }
    }
    private void UpdateTurretUI()
    {
        if (UpgradeManager.Instance != null && _levelText != null)
        {
            _level = UpgradeManager.Instance.GetUpgradeLevel(_turretType);

            _levelText.transform.DOKill();
            _levelText.transform.localScale = Vector3.one;

            _levelText.transform.DOScale(1.5f, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _levelText.text = _level.ToString();

                    _levelText.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
                });
        }
    }
}
