using UnityEngine;

public class CurrencyManager : Singleton<CurrencyManager>
{
    public int gold { get; private set; }
    public int puzzlePoint { get; private set; }

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
}
