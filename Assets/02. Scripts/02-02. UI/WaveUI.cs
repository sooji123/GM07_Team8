using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("Wave ¿¬°á")]
    [SerializeField]
    private WaveManager waveManager;

    [SerializeField]
    private TextMeshProUGUI waveText;

    void Update()
    {
        if (waveManager == null || waveText == null)
        {
            return;
        }
        /*
        if (waveManager.waves.Count > 9)
        {
            if (waveManager.currentWaveIndex > 9)
            {
                waveText.text = $"{waveManager.currentWaveIndex}/{waveManager.waves}";
                return;
            }
            waveText.text = $"0{waveManager.currentWaveIndex}/{waveManager.waves}";
            return;
        }
        waveText.text = $"0{waveManager.currentWaveIndex}/0{waveManager.waves}";
        */
    }
}
