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

    [Header("--- 하위 오브젝트 연결 ---")]
    public GameObject childHitEffectObject;
    public GameObject childHPBarObject;

    [Header("--- 시간 설정 ---")]
    public float hitEffectDuration = 0.3f;
    public float deathDuration = 0.8f;

    [HideInInspector]
    public Transform[] wayPoints;
    private int currentIndex = 0;

    [HideInInspector]
    public WaveManager waveManager;

    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;
    private Coroutine hitEffectCoroutine;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    private bool isStunned = false;
    private float slowMultiplier = 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        currentHp = maxHp;
        currentSpeed = speed;
        currentIndex = 0;
        isDead = false;
        isStunned = false;
        slowMultiplier = 1f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        if (childHPBarObject != null) childHPBarObject.SetActive(true);

        if (childHitEffectObject != null) childHitEffectObject.SetActive(false);

        if (slowCoroutine != null) { StopCoroutine(slowCoroutine); slowCoroutine = null; }
        if (stunCoroutine != null) { StopCoroutine(stunCoroutine); stunCoroutine = null; }
        if (hitEffectCoroutine != null) { StopCoroutine(hitEffectCoroutine); hitEffectCoroutine = null; }
    }

    void Start()
    {
        currentHp = maxHp;
        currentSpeed = speed;
    }

    void Update()
    {
        if (isDead) return;
        if (wayPoints == null || wayPoints.Length == 0) return;

        if (currentIndex >= wayPoints.Length)
        {
            HandleReachGoal();
            return;
        }

        Transform target = wayPoints[currentIndex];

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (target.position.x > transform.position.x);
        }

        currentSpeed = isStunned ? 0f : speed * slowMultiplier;

        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
        }
    }

    public void TakeDamage(float damage, EElement attackElement)
    {
        if (isDead || currentHp <= 0) return;

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

        if (childHitEffectObject != null)
        {
            if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine);
            hitEffectCoroutine = StartCoroutine(PlayChildHitEffectRoutine());
        }

        if (useStunOnHit)
        {
            ApplyStun(0.1f);
        }

        if (currentHp <= 0)
        {
            isDead = true;
            StartCoroutine(DefeatedRoutine());
        }
    }

    private IEnumerator PlayChildHitEffectRoutine()
    {
        childHitEffectObject.SetActive(true);
        yield return new WaitForSeconds(hitEffectDuration);
        childHitEffectObject.SetActive(false);
        hitEffectCoroutine = null;
    }

    private IEnumerator DefeatedRoutine()
    {
        Debug.Log($"{enemyName} 처치 완료! {rewardGold} 골드 획득.");
        CurrencyManager.Instance.AddGold(rewardGold);

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (childHPBarObject != null) childHPBarObject.SetActive(false);
        if (childHitEffectObject != null) childHitEffectObject.SetActive(false);

        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < deathDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / deathDuration;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, normalizedTime);
                spriteRenderer.color = c;
            }

            transform.position = new Vector3(startPosition.x, startPosition.y - (normalizedTime * 0.4f), startPosition.z);

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void ApplySlow(float slowDuration, float slowAmount)
    {
        if (!useSlowSystem || isDead) return;

        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(slowDuration, slowAmount));
    }

    private IEnumerator SlowRoutine(float duration, float amount)
    {
        slowMultiplier = amount;
        yield return new WaitForSeconds(duration);
        slowMultiplier = 1f;
        slowCoroutine = null;
    }

    private void ApplyStun(float stunDuration)
    {
        if (!useStunOnHit || isDead) return;

        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunRoutine(stunDuration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        stunCoroutine = null;
    }

    void HandleReachGoal()
    {
        Debug.Log($"{enemyName} 기지에 도달! 플레이어 라이프 감소.");

        if (waveManager != null) waveManager.RemoveEnemy(gameObject);
        gameObject.SetActive(false);
    }
}