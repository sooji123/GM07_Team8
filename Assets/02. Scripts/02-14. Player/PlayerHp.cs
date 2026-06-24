using UnityEngine;

public class PlayerHp : Singleton<PlayerHp>
{
    [SerializeField]
    private int maxHp = 5;
    private int currentHp;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    protected override void Awake()
    {
        base.Awake();
        currentHp = maxHp;
    }

    public void DecreasePlayerLife(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Debug.Log("체력 0, 게임 종료됩니다.");
        }
    }
}
