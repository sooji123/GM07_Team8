using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField]
    private Button _settingButton;

    private void Start()
    {

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeBGM(EBGMType.Game);
        }

        if (_settingButton != null && UI_Manager.Instance != null)
        {
            _settingButton.onClick.RemoveAllListeners();
            _settingButton.onClick.AddListener(UI_Manager.Instance.OpenSettingWindow);
        }
        
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.InitGold(100);
            CurrencyManager.Instance.InitElementOrbs(EElement.Water, 0);
            CurrencyManager.Instance.InitElementOrbs(EElement.Fire, 0);
            CurrencyManager.Instance.InitElementOrbs(EElement.Grass, 0);
            CurrencyManager.Instance.InitElementOrbs(EElement.Electric, 0);
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
