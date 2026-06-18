using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("--- 몹 기본 정보 ---")]
    public string enemyName = "몬스터 이름";
    public EElement elementType = EElement.None;

    [Header("--- 몹 능력치 ---")]
    public float maxHp = 100f;
    public float currentHp;
    public float speed = 2f;
    private float currentSpeed;
    public int armor = 5;
    public int rewardGold = 10;

    [HideInInspector]
    public Transform[] wayPoints;
    private int currentIndex = 0;

    [HideInInspector]
    public WaveManager waveManager;

    private Coroutine slowCoroutine;

    void OnEnable()
    {
        currentHp = maxHp;
        currentSpeed = speed;
        currentIndex = 0;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }
    }

    void Start()
    {
        currentHp = maxHp;
        currentSpeed = speed;
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

        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
        }
    }

    public void TakeDamage(float damage, EElement attackElement)
    {
        ERelationType relation = ElementRelations.EvaluateRelation(attackElement, this.elementType);
        float elementMultiplier = ElementRelations.GetDamageMultiplier(relation);

        float finalDamage = (damage * elementMultiplier) - armor;
        if (finalDamage < 1) finalDamage = 1;

        currentHp -= finalDamage;
        Debug.Log($"{enemyName}이(가) {attackElement} 속성 공격({relation})을 받아 {finalDamage}의 피해를 입음! (남은 HP: {currentHp})");

        if (currentHp <= 0)
        {
            HandleDefeated();
        }
    }

    public void ApplySlow(float slowDuration, float slowAmount)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }

        slowCoroutine = StartCoroutine(SlowRoutine(slowDuration, slowAmount));
    }

    private IEnumerator SlowRoutine(float duration, float amount)
    {
        currentSpeed = speed * amount;
        Debug.Log($"{enemyName} 슬로우 적용! 현재 속도: {currentSpeed}");

        yield return new WaitForSeconds(duration);

        currentSpeed = speed;
        slowCoroutine = null;
        Debug.Log($"{enemyName} 슬로우 해제! 현재 속도: {currentSpeed}");
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

    void AddRewardGold(int amount)
    {

    }

    void DecreasePlayerLife()
    {

    }
}