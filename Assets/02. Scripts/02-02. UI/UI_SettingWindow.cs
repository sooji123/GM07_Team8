using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SettingWindow : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Awake()
    {
        if (_bgmSlider != null)
        {
            _bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            _sfxSlider.gameObject.AddComponent<EventTrigger>();
        }
    }

    private void OnEnable()
    {
        float currentBGM = 1f;
        float currentSFX = 1f;

        if (SoundManager.Instance != null)
        {
            currentBGM = SoundManager.Instance.GetBGMVolume();
            currentSFX = SoundManager.Instance.GetSFXVolume();
        }
        else
        {
            currentBGM = PlayerPrefs.GetFloat("BGM_Volume", 1f);
            currentSFX = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.value = currentBGM;
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.value = currentSFX;
        }
    }

    public void OnClickSettingExitBtn()
    {
        UI_Manager.Instance.CloseSettingWindow();
    }

    public void OnClickRestartBtn()
    {
        UI_Manager.Instance.SwitchSettingWindow(false);

        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        UpgradeManager.Instance.ResetUpgrade();

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Game).Forget();
    }

    public void OnClickHomeBtn()
    {
        UI_Manager.Instance.SwitchSettingWindow(false);

        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Title).Forget();
    }

    private void OnBgmVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(value);
        }

        PlayerPrefs.SetFloat("BGM_Volume", value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXVolume(value);
        }

        PlayerPrefs.SetFloat("SFX_Volume", value);
    }

    public void OnSfxSliderPointerUp()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        }
    }
}
