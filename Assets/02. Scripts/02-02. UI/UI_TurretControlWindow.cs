using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TurretControlWindow : MonoBehaviour
{
    [SerializeField] private RectTransform _panelRect;
    [SerializeField] private float _tweenDuration;
    [Header("Panel Setting")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _upgradeBtn;
    [SerializeField] private GameObject _elementPanel;

    private TowerBuilder _towerBuilder;

    private TurretBase _targetTurret;

    public void Open(TurretBase turret, Vector3 turretPosition)
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

        _targetTurret = turret;
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

    public void Close()
    {
        _panelRect.DOKill();
        _panelRect.DOScale(Vector3.zero, _tweenDuration).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void RefreshUI()
    {
        if (_targetTurret == null || _upgradeBtn == null || _costText == null)
        {
            return;
        }

        if (_targetTurret.CurrentLevel < 3)
        {
            _upgradeBtn.interactable = true;
            _costText.text = $"X{_targetTurret.CurrentStat.upgradeCost}";
        }
        else
        {
            _upgradeBtn.interactable = false;
            _costText.text = $"X0";
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
    public void OnClickUpgradeBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        if (_targetTurret == null)
        {
            return ;
        }
        _towerBuilder.UpgradeTower(_targetTurret);
        RefreshUI();
    }
    public void OnClickSellBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.SellTower(_targetTurret);
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

        _towerBuilder.ElementTower(_targetTurret, EElement.Fire);
        Close();
    }
    public void OnClickWater()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Water);
        Close();
    }
    public void OnClickGrass()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        _towerBuilder.ElementTower(_targetTurret, EElement.Grass);
        Close();
    }
    public void OnClickElectric()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
    }
    public void OnClickElemetBackBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        SwitchPanelScaleTween(_mainPanel, _elementPanel);
    }
    #endregion
}
