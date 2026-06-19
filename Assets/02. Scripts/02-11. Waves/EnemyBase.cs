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

    [Header("--- 상태 이상 토글 옵션 ---")]
    public bool useStunOnHit = true;
    public bool useSlowSystem = true;

    [Header("--- 몹 고유 피격 이펙트 설정 ---")]
    public GameObject customHitEffectPrefab;

    [Header("--- 피격 이펙트 ---")]
    public Transform hitPoint;
    public float hitEffectDuration = 0.3f;

    [HideInInspector]
    public Transform[] wayPoints;
    private int currentIndex = 0;

    [HideInInspector]
    public WaveManager waveManager;

    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;

    void OnEnable()
    {
        currentHp = maxHp;
        currentSpeed = speed;
        currentIndex = 0;

        if (slowCoroutine != null) { StopCoroutine(slowCoroutine); slowCoroutine = null; }
        if (stunCoroutine != null) { StopCoroutine(stunCoroutine); stunCoroutine = null; }
    }

    void Start()
    {
        currentHp = maxHp;
        currentSpeed = speed;
    }

    void Update()
    {
        if (wayPoints == null || wayPoints.Length == 0) return;

        if (currentIndex >= wayPoints.Length)
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
        float elementMultiplier = 1f;

        if (attackElement != EElement.None)
        {
            ERelationType relation = ElementRelations.EvaluateRelation(attackElement, this.elementType);
            elementMultiplier = ElementRelations.GetDamageMultiplier(relation);
        }

        float finalDamage = (damage * elementMultiplier) - armor;
        if (finalDamage < 1) finalDamage = 1;

        currentHp -= finalDamage;
        Debug.Log($"{enemyName}이(가) {attackElement} 속성 공격을 받아 {finalDamage}의 피해를 입음! (남은 HP: {currentHp})");

        if (customHitEffectPrefab != null)
        {
            Vector3 spawnPosition = (hitPoint != null) ? hitPoint.position : transform.position;

            GameObject effectGo = EnemyObjectPool.Instance.SpawnFromPool(customHitEffectPrefab, spawnPosition, Quaternion.identity);

            if (effectGo != null)
            {
                StartCoroutine(DisableEffectRoutine(effectGo, hitEffectDuration));
            }
        }

        if (useStunOnHit)
        {
            ApplyStun(0.1f);
        }

        if (currentHp <= 0)
        {
            StartCoroutine(DefeatedRoutine());
        }
    }

    private IEnumerator DisableEffectRoutine(GameObject effectObj, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (effectObj != null && effectObj.activeSelf)
        {
            effectObj.SetActive(false);
        }
    }

    private IEnumerator DefeatedRoutine()
    {
        Debug.Log($"{enemyName} 처치 완료! {rewardGold} 골드 획득.");
        AddRewardGold(rewardGold);

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);

        yield return new WaitForSeconds(hitEffectDuration);

        gameObject.SetActive(false);
    }

    public void ApplySlow(float slowDuration, float slowAmount)
    {
        if (!useSlowSystem) return;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowRoutine(slowDuration, slowAmount));
    }

    private IEnumerator SlowRoutine(float duration, float amount)
    {
        currentSpeed = speed * amount;
        yield return new WaitForSeconds(duration);
        currentSpeed = speed;
        slowCoroutine = null;
    }

    private void ApplyStun(float stunDuration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        stunCoroutine = StartCoroutine(StunRoutine(stunDuration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        float previousSpeed = currentSpeed;
        currentSpeed = 0f;

        yield return new WaitForSeconds(duration);

        currentSpeed = previousSpeed;
        stunCoroutine = null;
    }

    void HandleReachGoal()
    {
        Debug.Log($"{enemyName} 기지에 도달! 플레이어 라이프 감소.");
        DecreasePlayerLife();

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);
        gameObject.SetActive(false);
    }

    void AddRewardGold(int amount) { }
    void DecreasePlayerLife() { }
}