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
        float savedBGM = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        if (_bgmSlider != null)
        {
            _bgmSlider.value = savedBGM;
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.value = savedSFX;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(savedBGM);
            SoundManager.Instance.SetSFXVolume(savedSFX);
        }
    }

    public void OnClickSettingExitBtn()
    {
        UI_Manager.Instance.CloseSettingWindow();
    }

    public void OnClickRestartBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Game).Forget();
    }

    public void OnClickHomeBtn()
    {
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
