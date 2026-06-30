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
    private void Start()
    {
        if(_upgradeBtn != null && _shopBtn != null && _levelText !=null)
        {
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
            _upgradeBtn.gameObject.SetActive(false);
            _shopBtn.onClick.AddListener(Toggle);
            _levelText.text = "1";
        }
    }

    private void OnClickUpgradeBtn()
    {
        if(UpgradeManager.Instance == null || SoundManager.Instance == null)
        {
            return;
        }

        if (UpgradeManager.Instance.IsUpgraded(_turretType))
        {
            return;
        }

        if (CurrencyManager.Instance.GetElementOrbs(EElement.Fire) >= 30 &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Water) >= 30 &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Grass) >= 30 &&
            CurrencyManager.Instance.GetElementOrbs(EElement.Electric) >= 30)
        {
            CurrencyManager.Instance.UseElementOrbs(EElement.Fire, 30);
            CurrencyManager.Instance.UseElementOrbs(EElement.Water, 30);
            CurrencyManager.Instance.UseElementOrbs(EElement.Grass, 30);
            CurrencyManager.Instance.UseElementOrbs(EElement.Electric, 30);

            SoundManager.Instance.PlayeSFX(ESFXType.Upgrade);
            UpgradeManager.Instance.UpgradeTurretType(_turretType);
            SetSlotSoldOut();
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

    private void SetSlotSoldOut()
    {
        _levelText.text = "2";
        _upgradeBtn.gameObject.SetActive(false);
        _upgradeBtn.interactable = false;
        _shopBtn.onClick.RemoveListener(OnClickUpgradeBtn);
    }

    public void Toggle()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        }
        _upgradeBtn.gameObject.SetActive(!_upgradeBtn.gameObject.activeSelf);
    }
}
