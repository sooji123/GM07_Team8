using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [Header("Hp Bar ¿¬°á")]
    [SerializeField]
    private Slider[] _slider;

    private void OnEnable()
    {
        EnergyManager.Instance.OnEnergyChanged += UpdateUI;
    }
    private void OnDisable()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnEnergyChanged -= UpdateUI;
        }
    }

    private void UpdateUI(int currentEnergyLv, int currentCost, int lvUpCost)
    {
        if (currentEnergyLv == 0)
        {
            _slider[0].value = (float)currentCost / lvUpCost;
        }
        else if (currentEnergyLv == 1)
        {
            _slider[1].value = (float)currentCost / lvUpCost;
        }
        else if (currentEnergyLv == 2)
        {
            _slider[2].value = (float)currentCost / lvUpCost;
        }
    }
}
