using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("--- 몹 기본 정보 ---")]
    public string enemyName = "몬스터 이름";
    public ElementType elementType = ElementType.None;

    [Header("--- 몹 능력치 ---")]
    public float maxHp = 100f;
    public float currentHp;
    public float speed = 2f;
    public int armor = 5;
    public int rewardGold = 10;

    [HideInInspector]
    public Transform[] wayPoints;
    private int currentIndex = 0;

    [HideInInspector]
    public WaveManager waveManager;

    void OnEnable()
    {
        currentHp = maxHp;
        currentIndex = 0;
    }

    void Start()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        if (wayPoints == null || currentIndex >= wayPoints.Length)
        {
            HandleReachGoal();
            return;
        }

        Transform target = wayPoints[currentIndex];

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (target.position.x > transform.position.x);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
        }
    }

    public void TakeDamage(float damage, ElementType attackElement)
    {
        float elementMultiplier = GetElementMultiplier(attackElement, this.elementType);
        float finalDamage = (damage * elementMultiplier) - armor;
        if (finalDamage < 1) finalDamage = 1;

        currentHp -= finalDamage;
        Debug.Log($"{enemyName}이(가) {attackElement} 속성 공격을 받아 {finalDamage}의 피해를 입음!");

        if (currentHp <= 0)
        {
            HandleDefeated();
        }
    }

    void HandleReachGoal()
    {
        Debug.Log($"{enemyName} 기지에 도달! 플레이어 라이프 감소.");

        DecreasePlayerLife();

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);
        gameObject.SetActive(false);
    }

    void HandleDefeated()
    {
        Debug.Log($"{enemyName} 처치 완료! {rewardGold} 골드 획득.");

        AddRewardGold(rewardGold);

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);
        gameObject.SetActive(false);
    }

    // 돈 올리는 함수
    void AddRewardGold(int amount)
    {

    }

    // 라이프 깎는 함수
    void DecreasePlayerLife()
    {

    }

    float GetElementMultiplier(ElementType attacker, ElementType defender)
    {
        if (attacker == ElementType.None || defender == ElementType.None) return 1f;
        if (attacker == ElementType.Water && defender == ElementType.Fire) return 2.0f;
        if (attacker == ElementType.Fire && defender == ElementType.Grass) return 2.0f;
        if (attacker == ElementType.Grass && defender == ElementType.Earth) return 2.0f;
        if (attacker == ElementType.Earth && defender == ElementType.Water) return 2.0f;
        if (attacker == ElementType.Fire && defender == ElementType.Water) return 0.5f;
        if (attacker == ElementType.Grass && defender == ElementType.Fire) return 0.5f;
        if (attacker == ElementType.Earth && defender == ElementType.Grass) return 0.5f;
        if (attacker == ElementType.Water && defender == ElementType.Earth) return 0.5f;
        return 1.0f;
    }
}