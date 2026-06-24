using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField]
    private Button _settingButton;

    private void Start()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeBGM(EBGMType.Game);
        }

        if (_settingButton != null && UI_Manager.Instance != null)
        {
            _settingButton.onClick.RemoveAllListeners();
            _settingButton.onClick.AddListener(UI_Manager.Instance.OpenSettingWindow);
        }
    }

    private void Update()
    {
        if (UI_Manager.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(UI_Manager.Instance.IsOpenSettingWindow == false)
            {
                UI_Manager.Instance.OpenSettingWindow();
            }
            else
            {
                UI_Manager.Instance.CloseSettingWindow();
            }
        }
    }
}
