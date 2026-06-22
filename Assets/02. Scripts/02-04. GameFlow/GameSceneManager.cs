using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GameSceneManager : Singleton<GameSceneManager>
{
    public void LoadScene(EScenes sceneType)
    {
        string sceneName = SceneName.GetSceneName(sceneType);
        SceneManager.LoadScene(sceneName);
    }

    public async UniTask LoadSceneWithFade(EScenes sceneType)
    {
        string sceneName = SceneName.GetSceneName(sceneType);

        if (UIManager.Instance != null) 
        {
            await UIManager.Instance.FadeOutAsync();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            await UniTask.Yield();
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));

        if (UIManager.Instance != null)
        {
            await UIManager.Instance.FadeInAsync();
        }
    }
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
