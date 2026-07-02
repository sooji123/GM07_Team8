using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    #region
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    private UI_BuildablesControlWindow _buildablesWindow;
    private UI_SettingWindow _settingWindow;
    private UI_GameOverWindow _gameOverWindow;
    private UI_GameClearWindow _gameClearWindow;

    [SerializeField]
    private bool _isOpenSettingWindow;

    private bool _isGameOverWindow;

    private bool _isGameClearWindow;

    private bool _isDoubleSpeed;

    public bool IsOpenSettingWindow => _isOpenSettingWindow;
    public bool IsGameOverWindow => _isGameOverWindow;
    public bool IsGameClearWindow => _isGameClearWindow;
    public bool IsDoubleSpeed => _isDoubleSpeed;
    #endregion
    protected override void Awake()
    {
        base.Awake();

        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;
        _isOpenSettingWindow = false;
        _isGameOverWindow = false;
        _isGameClearWindow = false;
        _isDoubleSpeed = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        if (scene.name == SceneName.GetSceneName(EScenes.Title))
        {
            _buildablesWindow = null;
            _settingWindow = null;
            _gameOverWindow = null;
            _gameClearWindow = null;
            return;
        }

        _buildablesWindow = FindFirstObjectByType<UI_BuildablesControlWindow>(FindObjectsInactive.Include);
        if (_buildablesWindow != null)
        {
            _buildablesWindow.gameObject.SetActive(false);
        }

        _settingWindow = FindFirstObjectByType<UI_SettingWindow>(FindObjectsInactive.Include);
        if (_settingWindow != null)
        {
            _settingWindow.gameObject.SetActive(false);
        }

        _gameOverWindow = FindFirstObjectByType<UI_GameOverWindow>(FindObjectsInactive.Include);
        if (_gameOverWindow != null)
        {
            _gameOverWindow.gameObject.SetActive(false);
        }

        _gameClearWindow = FindFirstObjectByType<UI_GameClearWindow>(FindObjectsInactive.Include);
        if (_gameClearWindow != null)
        {
            _gameClearWindow.gameObject.SetActive(false);
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

    #region Turret and Trap Window Control
    public void OpenTurretWindow(TurretBase turret, Vector3 turretPosition)
    {
        if (_buildablesWindow != null)
        {
            _buildablesWindow.Open(turret, turretPosition);
        }
    }

    public void OpenTrapWindow(TrapBase trap, Vector3 trapPosition)
    {
        if (_buildablesWindow != null)
        {
            _buildablesWindow.Open(trap, trapPosition);
        }
    }
    #endregion

    #region Setting Window Control
    public void OpenSettingWindow()
    {
        _isOpenSettingWindow = true;

        SoundManager.Instance.PlayeSFX(ESFXType.UIOpen);

        if (_settingWindow == null) return;

        _settingWindow.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseSettingWindow()
    {
        _isOpenSettingWindow = false;

        SoundManager.Instance.PlayeSFX(ESFXType.UIClose);

        if (_settingWindow == null) return;

        _settingWindow.gameObject.SetActive(false);

        TimeScale();
    }
    public void SwitchSettingWindow(bool isOpen)
    {
        _isOpenSettingWindow = isOpen;
    }
    #endregion

    #region Game Over Window Control
    public void GameOverWindow()
    {
        _isGameOverWindow = true;

        if(SoundManager.Instance == null) return;
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayeSFX(ESFXType.GameOver);

        _gameOverWindow.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }
    public void SwitchGameOverWindow(bool isOpen)
    {
        _isGameOverWindow = isOpen;
    }
    #endregion

    #region Game Clear Window Control
    public void GameClearWindow()
    {
        _isGameClearWindow = true;

        if (SoundManager.Instance == null) return;
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayeSFX(ESFXType.GameClear);

        _gameClearWindow.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }
    public void SwitchGameClearWindow(bool isOpen)
    {
        _isGameClearWindow = isOpen;
    }
    #endregion

    public bool DoubleSpeedUp()
    {
        _isDoubleSpeed = !_isDoubleSpeed;
        TimeScale();
        return _isDoubleSpeed;
    }

    public void TimeScale()
    {
        if (_isDoubleSpeed == true)
        {
            Time.timeScale = 2f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
