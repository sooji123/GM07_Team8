using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeSlot : MonoBehaviour
{
    [SerializeField]
    private ETurretType _turretType;
    [SerializeField]
    private Button _upgradeBtn;

    private void Start()
    {
        if(_upgradeBtn != null)
        {
            _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
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
        _upgradeBtn.interactable = false;
    }
}
