using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private TurretData _turretData;
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [Header("Button")]
    [SerializeField]
    private Button _upgradeBtn;
    [SerializeField]
    private TextMeshProUGUI _upgradeBtnText;
    [SerializeField]
    private TextMeshProUGUI _upgradeBtnExp;
    [SerializeField]
    private TextMeshProUGUI _upgradeBtnCost;
    [Header("Shop")]
    [SerializeField]
    private TextMeshProUGUI _shopLevelText;
    [SerializeField]
    private Image _fillImage;
    [SerializeField]
    private GameObject _max;

    private int _level = 1;
    private ETurretType _turretType;

    private void Start()
    {
        if(_turretData != null)
        {
            _turretType = _turretData.turretType;
        }
        if (_upgradeBtn != null && _levelText !=null && _max != null)
        {
            _upgradeBtn.gameObject.SetActive(true);
            _upgradeBtn.interactable = true;
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
            _max.SetActive(false);
            _level = 1;
            _levelText.text = _level.ToString();
            UpdateSlotUI();
        }
    }

    private void OnClickUpgradeBtn()
    {
        if(UpgradeManager.Instance == null || SoundManager.Instance == null)
        {
            return;
        }

        if (_level >= 3)
        {
            return;
        }

        int cost = (_level == 1) ? 30 : 40;

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

            if (_upgradeBtn.TryGetComponent<RectTransform>(out RectTransform upgradeBtnRect))
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
        _shopLevelText.text = $"Lvl. {_level}";

        if (_level >= 3)
        {
            if(_upgradeBtn != null && _max != null)
            {
                _upgradeBtn.interactable = false;
                _upgradeBtn.gameObject.SetActive(false);
                _max.SetActive(true);
            }
        }
        else
        {
            int nextLevelIndex = _level - 1;

            if (_turretData != null && _turretData.turretLevels != null && nextLevelIndex < _turretData.turretLevels.Length)
            {
                TurretLevel nextLevelData = _turretData.turretLevels[nextLevelIndex];

                if (_upgradeBtnText != null)
                    _upgradeBtnText.text = nextLevelData.level;
                if (_upgradeBtnExp != null) 
                    _upgradeBtnExp.text = nextLevelData.explanation;
                if (_upgradeBtnCost != null) 
                    _upgradeBtnCost.text = nextLevelData.cost;
            }
        }

        if (_fillImage != null)
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
