using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayeBGM(EBGMType.Title);
    }
    public void OnClickStartBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        GameSceneManager.Instance.LoadSceneWithFade(EScenes.Game).Forget();
    }
    public void OnClickExitBtn()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
