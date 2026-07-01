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
        for (int i = 0; i < _skillButton.Length; i++)
        {
            _skillButton[i].interactable = false;
        }
    }

    private void Start()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnEnergyChanged += UpdateEnergy;
            EnergyManager.Instance.OnEnergyChanged += UpdateButton;
        }        
    }
    private void OnDestroy()
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
        if (EnergyManager.Instance.lvUpCost <= 0)
        {
            Debug.Log("에너지 레벨업 비용이 0 이하입니다, 갱신 불가");
            return;
        }

        if (currentEnergyLv == 0)
        {
            _slider[0].value = (float)currentCost / lvUpCost;
            _slider[1].value = 0.0f;
            _slider[2].value = 0.0f;
        }
        else if (currentEnergyLv == 1)
        {
            _slider[0].value = 1.0f;
            _slider[1].value = (float)currentCost / lvUpCost;
            _slider[2].value = 0.0f;
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

        Debug.Log($"에너지 게이지 갱신!");
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

    public void ClickSkill(int level)
    {
        Debug.Log($"스킬 버튼 클릭! [ Lv.{level} ]");
        EnergyManager.Instance.UseEnergy(level);
    }
}
