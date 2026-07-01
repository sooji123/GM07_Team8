using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    [SerializeField]
    private PuzzleController controller;

    [SerializeField]
    private WaveManager waveManager;

    [SerializeField]
    private GameObject GimikShop;

    [SerializeField]
    public Button startButton;

    private void Start()
    {
        startButton.gameObject.SetActive(true); //활성화
        GimikShop.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        startButton.gameObject.SetActive(false); //비활성화
        GimikShop.SetActive(false);
        if (waveManager.CurrentWaveIndex == 1)
        {
            controller.FirstWave();
        }
        else
        {
            controller.StartWave();
        }
    }
    public void WaveEnded()
    {
        startButton.gameObject.SetActive(true); //활성화
        GimikShop.SetActive(true);
        controller.EndWave();
    }
}
