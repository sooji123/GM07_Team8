using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_GameClearWindow : MonoBehaviour
{
    public void OnClickRestartBtn()
    {
        UI_Manager.Instance.SwitchGameClearWindow(false);

        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        UpgradeManager.Instance.ResetUpgrade();

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Game).Forget();
    }

    public void OnClickHomeBtn()
    {
        UI_Manager.Instance.SwitchGameClearWindow(false);

        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Title).Forget();
    }
}
