using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private ETurretType _turretType;
    [SerializeField]
    private Button _upgradeBtn;

    private bool _isUpgrade;

    private void Start()
    {
        if(_upgradeBtn != null)
        {
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
            _isUpgrade = false;
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

        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        UpgradeManager.Instance.UpgradeTurretType(_turretType);
        SetSlotSoldOut();
    }

    private void SetSlotSoldOut()
    {
        _isUpgrade = true;
        _upgradeBtn.interactable = false;
        _upgradeBtn.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if(_isUpgrade)
        {
            return;
        }
        _upgradeBtn.gameObject.SetActive(!_upgradeBtn.gameObject.activeSelf);
    }
}
