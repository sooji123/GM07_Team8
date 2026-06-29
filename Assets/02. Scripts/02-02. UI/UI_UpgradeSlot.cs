using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private ETurretType _turretType;
    [SerializeField]
    private Button _shopBtn;
    [SerializeField]
    private Button _upgradeBtn;

    private void Start()
    {
        if(_upgradeBtn != null && _shopBtn != null)
        {
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
            _upgradeBtn.gameObject.SetActive(false);
            _shopBtn.onClick.AddListener(Toggle);
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
        }
    }

    private void SetSlotSoldOut()
    {
        _upgradeBtn.interactable = false;
        _upgradeBtn.gameObject.SetActive(false);
        _shopBtn.interactable = false;
        _shopBtn.onClick.RemoveListener(Toggle);
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
