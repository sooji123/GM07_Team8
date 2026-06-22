using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    private int maxHp = 5;
    private int currentHp;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    private void Awake()
    {
        currentHp = maxHp; //현재 체력 = 최대 체력
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            DecreasePlayerLife(1);
        }
    }
    public void DecreasePlayerLife(int damage)
    {
        currentHp -= damage;

        if ( currentHp <= 0 )
        {
            Debug.Log("체력 0, 게임 종료됩니다.");
        }
    }
}
