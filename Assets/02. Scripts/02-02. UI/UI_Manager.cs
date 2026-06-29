using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    private UI_BuildablesControlWindow _buildablesWindow;
    private UI_SettingWindow _settingWindow;
    [SerializeField]
    private bool _isOpenSettingWindow;

    public bool IsOpenSettingWindow => _isOpenSettingWindow;
    protected override void Awake()
    {
        base.Awake();

        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;
        _isOpenSettingWindow = false;

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
        Debug.Log("ÅÍ·¿À©µµ¿ì");
        if (_buildablesWindow != null)
        {
            _buildablesWindow.Open(turret, turretPosition);
        }
    }

    public void OpenTrapWindow(TrapBase trap, Vector3 trapPosition)
    {
        Debug.Log("Æ®·¦À©µµ¿ì");
        if (_buildablesWindow != null)
        {
            _buildablesWindow.Open(trap, trapPosition);
        }
    }

    #region Setting Window Control
    public void OpenSettingWindow()
    {
        _isOpenSettingWindow = true;

        SoundManager.Instance.PlayeSFX(ESFXType.UIOpne);

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

        Time.timeScale = 1f;
    }
    public void SwitchSettingWindow(bool isOpen)
    {
        _isOpenSettingWindow = isOpen;
    }
    #endregion
}
