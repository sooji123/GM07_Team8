using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [Header("Energy Bar 연결")]
    [SerializeField]
    private Slider[] _slider;

    [Header("Skill Button 연결")]
    [SerializeField]
    private Button[] _skillButton;

    //버튼 활성화 초기화 Awake
    private void Awake()
    {
        for (int i = 0; i < _slider.Length; i++)
        {
            _skillButton[i].interactable = false;
        }
    }

    private void OnEnable()
    {
        EnergyManager.Instance.OnEnergyChanged += UpdateEnergy;
        EnergyManager.Instance.OnEnergyChanged += UpdateButton;
    }
    private void OnDisable()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnEnergyChanged -= UpdateEnergy;
            EnergyManager.Instance.OnEnergyChanged -= UpdateButton;
        }
    }

    //현 EnergyLv, Cost 기반 슬라이더 값 업데이트
    private void UpdateEnergy(int currentEnergyLv, int currentCost, int lvUpCost)
    {
        if (currentEnergyLv == 0)
        {
            _slider[0].value = (float)currentCost / lvUpCost;
        }
        else if (currentEnergyLv == 1)
        {
            _slider[0].value = 1.0f;
            _slider[1].value = (float)currentCost / lvUpCost;
        }
        else if (currentEnergyLv == 2)
        {
            _slider[0].value = 1.0f;
            _slider[1].value = 1.0f;
            _slider[2].value = (float)currentCost / lvUpCost;
        }
        else if (currentEnergyLv >= 3)
        {
            _slider[0].value = 1.0f;
            _slider[1].value = 1.0f;
            _slider[2].value = 1.0f;
        }
    }

    //현 EnergyLv 기반 버튼 활성화
    private void UpdateButton(int currentEnergyLv, int currentCost, int lvUpCost)
    {
        for (int i = 0; i < _skillButton.Length; i++)
        {
            if (i < currentEnergyLv)
            {
                _skillButton[i].interactable = true;
            }
            else
            {
                _skillButton[i].interactable = false;
            }
        }
    }
}
