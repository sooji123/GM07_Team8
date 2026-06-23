using UnityEngine;
using System.Collections.Generic;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public int gold { get; private set; }
    public int puzzlePoint { get; private set; }

    // EElement 열거형과 연결해 딕셔너리로 오브 저장
    private Dictionary<EElement, int> elementOrbs = new Dictionary<EElement, int>()
    {
        { EElement.Water, 0 },
        { EElement.Fire, 0 },
        { EElement.Grass, 0 },
        { EElement.Earth, 0 }
    };

    // =====골드 관리=====
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.Log($"획득하려는 골드가 0 이하임");
            return;
        } // 음수 방지

        gold += amount;
        Debug.Log($"{amount}골드 획득, 현재 {gold}골드");
    }

    public bool UseGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.Log($"지불하려는 골드가 0 이하임");
            return false;
        } // 음수 방지

        if (amount > gold)
        {
            Debug.Log($"골드가 부족하여 구매할 수 없음");
            return false;
        } // 지불하려는 골드가 소지금보다 많을 때

        gold -= amount;
        Debug.Log($"{amount}골드 소비, 현재 {gold}골드");
        return true;
    }

    // =====PP 관리(PuzzlePoint)=====
    public void AddPuzzlePoint(int amount)
    {
        if (amount <= 0)
        {
            Debug.Log($"획득하려는 PP가 0 이하임");
            return;
        } // 음수 방지

        puzzlePoint += amount;
        Debug.Log($"{amount}PP 획득, 현재 {puzzlePoint}PP");
    }

    public bool UsePP()
    {
        if (puzzlePoint <= 0)
        {
            Debug.Log($"PP가 부족하여 실행할 수 없음");
            return false;
        } // PP가 0이하일 때

        puzzlePoint--;
        Debug.Log($"1PP 소모, 현재 {puzzlePoint}PP");
        return true;
    }

    // =====속성 오브 관리(elementOrbs)=====

    // 특정 속성의 현재 재화량을 확인하는 읽기 전용 메서드
    public int GetElementOrbs(EElement element)
    {
        if (element == EElement.None || !elementOrbs.ContainsKey(element))
        {
            return 0;
        } // None 속성이거나 딕셔너리에 없으면 0 리턴

        Debug.Log($"{element} 속성 오브 {elementOrbs[element]}개");
        return elementOrbs[element];
    }

    public void AddElementOrbs(EElement element, int amount)
    {
        if (element == EElement.None)
        {
            return;
        } // None은 돌려보냄

        if (amount <= 0)
        {
            Debug.Log($"획득하려는 {element} 재화가 0 이하임");
            return;
        } // 음수 방지

        elementOrbs[element] += amount;
        Debug.Log($"{element} 속성 오브 {amount}개 획득, 현재 {elementOrbs[element]}개");
    }

    public bool UseElementOrbs(EElement element, int amount)
    {
        if (element == EElement.None)
        {
            return false;
        } // None은 돌려보냄

        if (amount <= 0)
        {
            Debug.Log($"지불하려는 {element} 재화가 0 이하임");
            return false;
        } // 음수 방지

        if (amount > elementOrbs[element])
        {
            Debug.Log($"{element} 오브가 부족하여 구매할 수 없음");
            return false;
        } // 지불하려는 오브가 소지 오브보다 많을 때

        elementOrbs[element] -= amount;
        Debug.Log($"{element} 속성 오브 {amount}개 소비, 현재 {elementOrbs[element]}개");
        return true;
    }
}
