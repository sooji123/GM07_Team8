using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    [SerializeField]
    private PuzzleController controller;

    [SerializeField]
    private WaveManager waveManager;

    [SerializeField]
    public Button startButton;

    private void Start()
    {
        startButton.gameObject.SetActive(true); //활성화
    }

    public void OnStartButtonClicked()
    {
        startButton.gameObject.SetActive(false); //비활성화
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
        controller.EndWave();
    }
}
