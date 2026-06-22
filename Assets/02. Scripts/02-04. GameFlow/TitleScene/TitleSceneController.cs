using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayeBGM(EBGMType.Title);
    }
    public void OnClickStartButton()
    {
        SoundManager.Instance.PlayeSFX(ESFXType.ButtonClick);
        GameSceneManager.Instance.LoadScene(EScenes.Game);
        
    }
}
