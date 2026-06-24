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
        
        if (waveManager.waves.Count > 9)
        {
            if (waveManager.CurrentWaveIndex > 9)
            {
                waveText.text = $"{waveManager.CurrentWaveIndex}/{waveManager.waves.Count}";
                return;
            }
            waveText.text = $"0{waveManager.CurrentWaveIndex}/{waveManager.waves.Count}";
            return;
        }
        waveText.text = $"0{waveManager.CurrentWaveIndex}/0{waveManager.waves.Count}";
    }
}
