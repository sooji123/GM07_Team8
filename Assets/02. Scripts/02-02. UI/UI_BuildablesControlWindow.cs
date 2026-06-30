using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildablesControlWindow : MonoBehaviour
{
    #region
    [SerializeField]
    private RectTransform _panelRect;
    [SerializeField]
    private float _tweenDuration;
    [Header("Panel Setting")]
    [SerializeField]
    private GameObject _mainPanel;
    [SerializeField]
    private TextMeshProUGUI _costDamageText;
    [SerializeField]
    private Button _upgradeDamageBtn;
    [SerializeField]
    private TextMeshProUGUI _costSpeedText;
    [SerializeField]
    private Button _upgradeSpeedBtn;
    [SerializeField]
    private Button _elementBtn;
    [SerializeField]
    private GameObject _elementPanel;
    [Header("Sprite")]
    [SerializeField]
    private Sprite _fireSprite;
    [SerializeField]
    private Sprite _waterSprite;
    [SerializeField]
    private Sprite _grassSprite;
    [SerializeField]
    private Sprite _electricSprite;

    private TowerBuilder _towerBuilder;

    private TurretBase _targetTurret;
    private TrapBase _targetTrap;
    private int _upgradeCost = 5;
    #endregion

    public void Open(TurretBase turret, Vector3 turretPosition)
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

        _targetTurret = turret;
        _targetTrap = null;

        _towerBuilder = turret.Builder;
        transform.position = turretPosition;

        gameObject.SetActive(true);

        if (_mainPanel != null) 
        {
            _mainPanel.SetActive(true);
        }
        if (_elementPanel != null) 
        { 
            _elementPanel.SetActive(false);
        }
        RefreshUI();
        _panelRect.DOKill();
        _panelRect.localScale = Vector3.zero;
        _panelRect.DOScale(Vector3.one, _tweenDuration).SetEase(Ease.OutBack);
    }

    public void Open(TrapBase trap, Vector3 trapPosition)
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

        _targetTurret = null;
        _targetTrap = trap;

        _towerBuilder = trap.Builder;
        transform.position = trapPosition;

        gameObject.SetActive(true);

        if (_mainPanel != null)
        {
            _mainPanel.SetActive(true);
        }
        if (_elementPanel != null)
        {
            _elementPanel.SetActive(false);
        }
        RefreshUI();
        _panelRect.DOKill();
        _panelRect.localScale = Vector3.zero;
        _panelRect.DOScale(Vector3.one, _tweenDuration).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        _panelRect.DOKill();
        _panelRect.DOScale(Vector3.zero, _tweenDuration).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));

        if (_mainPanel != null)
        {
            _mainPanel.SetActive(true);
        }
        if (_elementPanel != null)
        {
            _elementPanel.SetActive(false);
        }
    }

    private void RefreshUI()
    {
        if (_upgradeDamageBtn == null || _upgradeSpeedBtn == null || _costDamageText == null || _costSpeedText == null || _elementBtn ==  null)
        { 
            return;
        }

        if (_targetTurret != null && _targetTurret is not BuffTurret)
        {
            _upgradeDamageBtn.gameObject.SetActive(true);
            _upgradeSpeedBtn.gameObject.SetActive(true);
            _elementBtn.gameObject.SetActive(true);

            if (_targetTurret.DamageUpgradeCount < 3)
            {
                _upgradeDamageBtn.interactable = true;
                _costDamageText.text = $"{_upgradeCost * (_targetTurret.DamageUpgradeCount+1)}";
            }
            else
            {
                _upgradeDamageBtn.interactable = false;
                _costDamageText.text = "";
            }

            if (_targetTurret.SpeedUpgradeCount < 3)
            {
                _upgradeSpeedBtn.interactable = true;
                _costSpeedText.text = $"{_upgradeCost * (_targetTurret.SpeedUpgradeCount + 1)}";
            }
            else
            {
                _upgradeSpeedBtn.interactable = false;
                _costSpeedText.text = "";
            }
        }
        else if (_targetTrap != null || (_targetTurret != null && _targetTurret is BuffTurret)) 
        {
            _upgradeDamageBtn.gameObject.SetActive(false);
            _upgradeSpeedBtn.gameObject.SetActive(false);
            _elementBtn.gameObject.SetActive(false);
        }
    }

    private void SwitchPanelScaleTween(GameObject activePanel, GameObject inactivePanel)
    {
        if (activePanel == null || inactivePanel == null) return;

        inactivePanel.transform.DOKill();
        activePanel.transform.DOKill();

        inactivePanel.transform.DOScale(Vector3.zero, _tweenDuration * 0.5f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                inactivePanel.SetActive(false);

                activePanel.SetActive(true);
                activePanel.transform.localScale = Vector3.zero;
                activePanel.transform.DOScale(Vector3.one, _tweenDuration * 0.8f).SetEase(Ease.OutBack);
            });
    }

    #region MainPanel Button
    public void OnClickDamageUpgradeBtn()
    {
        if (_targetTurret == null || _targetTrap != null)
        {
            return ;
        }
        bool upgradeSuccess = _towerBuilder.UpgradeDamageTower(_targetTurret, _upgradeCost * (_targetTurret.DamageUpgradeCount + 1));
        SoundManager.Instance.PlayeSFX(upgradeSuccess ? ESFXType.Upgrade : ESFXType.Upgrade_fail);

        RefreshUI();
    }

    public void OnClickSpeedUpgradeBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        if (_targetTurret == null || _targetTrap != null)
        {
            return;
        }
        bool upgradeSuccess = _towerBuilder.UpgradeSpeedTower(_targetTurret, _upgradeCost * (_targetTurret.DamageUpgradeCount + 1));
        SoundManager.Instance.PlayeSFX(upgradeSuccess ? ESFXType.Upgrade : ESFXType.Upgrade_fail);

        RefreshUI();
    }


    public void OnClickSellBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        if (_targetTurret != null)
        {
            _towerBuilder.SellTower(_targetTurret);
        }
        else if (_targetTrap != null)
        {
            _towerBuilder.SellTower(_targetTrap);
        }

        Close();
    }
    public void OnClickElemetBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        SwitchPanelScaleTween(_elementPanel, _mainPanel);
    }
    public void OnClickExitBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIClose);

        Close();
    }
    #endregion

    #region ElemetPanel Button
    public void OnClickFire()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Fire, _fireSprite);
        Close();
    }
    public void OnClickWater()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Water, _waterSprite);
        Close();
    }
    public void OnClickGrass()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Grass , _grassSprite);
        Close();
    }
    public void OnClickElectric()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Electric, _electricSprite);
        Close();
    }
    public void OnClickElemetBackBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        SwitchPanelScaleTween(_mainPanel, _elementPanel);
    }
    #endregion
}
