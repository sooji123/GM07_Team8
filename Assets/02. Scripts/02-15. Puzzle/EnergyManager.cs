using UnityEngine;
using System;

public class EnergyManager : Singleton<EnergyManager>
{
    [Header("에너지 세팅")] // 칸
    public int currentEnergy = 0;
    public int maxEnergy = 3;

    [Header("에너지별 게이지 세팅")] // 게이지
    [SerializeField] private int lv1Cost = 30;
    [SerializeField] private int lv2Cost = 50;
    [SerializeField] private int lv3Cost = 70;

    public int currentCost = 0;
    public int lvUpCost = 0;

    public event Action<int, int> OnEnergyChanged; // 퍼즐을 맞췄을 때 호출

    public void AddEnergy(int amount)
    {
        if (currentEnergy == maxEnergy)
        {
            Debug.Log($"이미 에너지 max");
            return;
        }

        switch (currentEnergy)
        {
            case 0
                : lvUpCost = lv1Cost;
                break;
            case 1
                : lvUpCost = lv2Cost;
                break;
            case 2
                : lvUpCost = lv3Cost;
                break;
            case 3 
                : 
                break;
        }

        currentCost += amount;

        if (currentCost >= lvUpCost)
        {
            currentEnergy++;
            currentCost -= lvUpCost;
            Debug.Log($"에너지 획득, 현재 에너지: {currentEnergy} / {maxEnergy}");
        }

        Debug.Log($"현재 게이지 [ {currentCost} / {lvUpCost} ]");
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy); // 신호 쏘기
    }

    public void UseEnergy(int level)
    {
        int requireCost = 0;
              
        if (currentEnergy >= requireCost) // 에너지가 충분하다면
        {
            if (currentEnergy < requireCost)
            {
                /*
                 * 에너지가 "칸 단위로" 차면  
                 * 사용 시 여분 에너지는 삭제시킬거임
                 */
            }
            currentEnergy -= requireCost;

            // UI 갱신 신호 보내기
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

            Debug.Log($"Lv.{level} 스킬 발동 -> 남은 에너지");

            // 스킬 코드 Lv1
            // 스킬 코드 Lv2
            // 스킬 코드 Lv3
        }
        else
        {
            Debug.Log($"에너지 부족 -> 필요량: {requireCost}, 현재: {currentEnergy})");
        }
    }
}