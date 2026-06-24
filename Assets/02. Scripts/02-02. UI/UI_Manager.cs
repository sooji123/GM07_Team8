using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    private UI_BuildablesControlWindow _buildablesWindow;
    private UI_SettingWindow _settingWindow;

    protected override void Awake()
    {
        base.Awake();

        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;

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

    #region Setting Window Control
    public void OpenSettingWindow()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

        if (_settingWindow == null) return;

        if (_buildablesWindow != null && _buildablesWindow.gameObject.activeSelf)
        {
            _buildablesWindow.Close();
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
