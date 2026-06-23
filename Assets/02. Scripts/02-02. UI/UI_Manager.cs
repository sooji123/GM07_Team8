using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    private UI_TurretControlWindow _turretWindow;
    private UI_SettingWindow _settingWindow;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);

            _fadeCanvasGroup.alpha = 0f;
            _fadeCanvasGroup.blocksRaycasts = false;
        }

        _turretWindow = FindAnyObjectByType<UI_TurretControlWindow>();

        _settingWindow = FindAnyObjectByType<UI_SettingWindow>();
        if( _settingWindow != null)
        {
            _settingWindow.gameObject.SetActive(false);
        }
    }
    public async UniTask FadeOutAsync()
    {
        _fadeCanvasGroup.blocksRaycasts = true;
        await _fadeCanvasGroup.DOFade(1f, _fadeDuration)
            .SetUpdate(true)
            .ToUniTask();
    }

    public async UniTask FadeInAsync()
    {
        await _fadeCanvasGroup.DOFade(0f, _fadeDuration)
            .SetUpdate(true)
            .ToUniTask();
        _fadeCanvasGroup.blocksRaycasts = false;
    }

    public void OpenTurretWindow(TurretBase turret, Vector3 turretPosition)
    {
        if (_turretWindow != null)
        {
            _turretWindow.Open(turret, turretPosition);
        }
    }

    #region Setting Window Control
    public void OpenSettingWindow()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

        if (_settingWindow == null) return;

        if (_turretWindow != null && _turretWindow.gameObject.activeSelf)
        {
            _turretWindow.Close();
        }

        _settingWindow.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseSettingWindow()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIClose);

        if (_settingWindow == null) return;

        _settingWindow.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }
    #endregion
}
