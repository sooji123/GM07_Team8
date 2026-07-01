using UnityEngine;

public class TestEnergyFull : MonoBehaviour
{
    [Header("에너지량")]
    [SerializeField] private int energy = 30;

    public void OnClickGiveEnergy()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.AddEnergy(energy);
        }
        else
        {
            Debug.LogError("EnergyManager 없어용 넣으세용");
        }
    }
}
