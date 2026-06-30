using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private ETurretType _turretType;
    [SerializeField]
    private Button _shopBtn;
    [SerializeField]
    private Button _upgradeBtn;
    [SerializeField]
    private TextMeshProUGUI _levelText;

    private int _level = 1;


    private void Start()
    {
        if(_upgradeBtn != null && _shopBtn != null && _levelText !=null)
        {
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
            _upgradeBtn.gameObject.SetActive(false);
            _shopBtn.onClick.AddListener(Toggle);
            _level = 1;
            _levelText.text = _level.ToString();
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

        int cost = (_level == 1) ? 30 : 50;

        /*if (CurrencyManager.Instance.GetElementOrbs(EElement.Fire) >= cost &&
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
        }*/

        SoundManager.Instance.PlayeSFX(ESFXType.Upgrade);
        UpgradeManager.Instance.UpgradeTurretType(_turretType);
        UpdateSlotUI(); //테스트용
    }

    private void UpdateSlotUI()
    {
        if(UpgradeManager.Instance != null && _levelText != null)
        {
            _level = UpgradeManager.Instance.GetUpgradeLevel(_turretType);

            if (_level >= 3)
            {
                _upgradeBtn.gameObject.SetActive(false);
                _upgradeBtn.interactable = false;
                _shopBtn.onClick.RemoveListener(Toggle);
            }

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
    private void Toggle()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        }
        _upgradeBtn.gameObject.SetActive(!_upgradeBtn.gameObject.activeSelf);
    }
}
