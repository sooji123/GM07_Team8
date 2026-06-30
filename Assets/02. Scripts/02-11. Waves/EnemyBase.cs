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
    public int barrierHitCount = 10;
    private int currentBarrierHitCount;
    public int rewardGold = 10;

    [Header("--- 방패 : 일정 데미지 이하 무시 ---")]
    public bool useShieldBlock = false;
    public float blockThresholdDamage = 15f;

    [Header("--- 배리어 : 일정 타격 횟수 무시 ---")]
    public bool useMagicBarrier = false;

    [Header("--- 녹십자 : 체력 회복 ---")]
    public bool useRegeneration = false;
    public float regenAmount = 5f;
    public float regenInterval = 1f;
    private float regenTimer = 0f;

    [Header("--- 별 효과 : 타격 횟수로만 피격 ---")]
    public bool useHitCountOnly = false;

    [Header("--- 상태 이상 토글 옵션 ---")]
    public bool useStunOnHit = true;
    public bool useSlowSystem = true;

    [Header("--- 하위 오브젝트 연결 ---")]
    public GameObject childHitEffectObject;
    public GameObject childHPBarObject;
    [Space(10)]
    [Header("기믹 자식 오브젝트들")]
    public GameObject childShieldObject;
    public GameObject childBarrierObject;
    public GameObject childRegenObject;
    public GameObject childStarObject;

    [Header("--- 시간 설정 ---")]
    public float hitEffectDuration = 0.3f;
    public float deathDuration = 0.8f;

    [HideInInspector] public Transform[] wayPoints;
    private int currentIndex = 0;
    [HideInInspector] public WaveManager waveManager;

    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;
    private Coroutine hitEffectCoroutine;

    private SpriteRenderer[] spriteRenderers;
    private Animator animator;
    private bool isDead = false;
    private bool isStunned = false;
    private float slowMultiplier = 1f;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        currentHp = maxHp;
        currentSpeed = speed;
        currentIndex = 0;
        isDead = false;
        isStunned = false;
        slowMultiplier = 1f;

        if (animator != null)
        {
            animator.speed = 1f;
            animator.Play("Move", 0, 0f);
            animator.ResetTrigger("Die");
            animator.ResetTrigger("Hit");
        }

        if (spriteRenderers != null)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 1f;
                    sr.color = c;
                }
            }
        }

        if (childShieldObject != null) childShieldObject.SetActive(false);
        if (childRegenObject != null) childRegenObject.SetActive(false);
        if (childBarrierObject != null) childBarrierObject.SetActive(false);
        if (childStarObject != null) childStarObject.SetActive(useHitCountOnly);

        regenTimer = 0f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

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

        if (useRegeneration && currentHp < maxHp)
        {
            HandleRegeneration();
        }

        if (wayPoints == null || wayPoints.Length == 0) return;

        if (currentIndex >= wayPoints.Length)
        {
            HandleReachGoal();
            return;
        }

        Transform target = wayPoints[currentIndex];

        if (spriteRenderers != null)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null) sr.flipX = (target.position.x > transform.position.x);
            }
        }

        currentSpeed = isStunned ? 0f : speed * slowMultiplier;
        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
        }
    }

    private void HandleRegeneration()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer >= regenInterval)
        {
            currentHp = Mathf.Min(maxHp, currentHp + regenAmount);
            Debug.Log($"{enemyName} 기믹 효과로 HP {regenAmount} 재생! (현재 HP: {currentHp})");
            regenTimer = 0f;
        }
    }

    public void TakeDamage(float damage, EElement attackElement)
    {
        if (isDead) return;

        string damageTypeLog = "일반 피해";
        float finalDamage = damage;

        if (useMagicBarrier && currentBarrierHitCount > 0)
        {
            currentBarrierHitCount--;
            Debug.Log($"{enemyName} 마법 보호막 발동! 타격을 무시합니다. (남은 보호막 횟수: {currentBarrierHitCount})");

            if (currentBarrierHitCount <= 0 && childBarrierObject != null)
            {
                childBarrierObject.SetActive(false);
                Debug.Log($"{enemyName}의 마법 보호막이 완전히 파괴되었습니다!");
            }

            ProcessFinalDamage(0f, attackElement, "마법 보호막 방어", true);
            return;
        }

        float elementMultiplier = 1f;
        ERelationType relation = ERelationType.None;

        if (attackElement != EElement.None)
        {
            relation = ElementRelations.EvaluateRelation(attackElement, this.elementType);
            elementMultiplier = ElementRelations.GetDamageMultiplier(relation);
        }
        finalDamage *= elementMultiplier;

        if (useShieldBlock)
        {
            if (finalDamage <= blockThresholdDamage)
            {
                Debug.Log($"{enemyName}의 방패가 상성 적용된 데미지 {finalDamage}를 완전히 차단했습니다! (기준치: {blockThresholdDamage})");
                ProcessFinalDamage(0f, attackElement, "방패 무시 효과", false);
                return;
            }
        }

        finalDamage -= armor;
        if (finalDamage < 1) finalDamage = 1;

        if (useHitCountOnly)
        {
            damageTypeLog = "타격 횟수 기믹(별 효과)";
            if (attackElement != EElement.None)
            {
                if (relation == ERelationType.Advantage) finalDamage = 0f;
                else if (relation == ERelationType.Disadvantage) finalDamage = 2f;
                else finalDamage = 1f;
            }
            else
            {
                finalDamage = 1f;
            }
        }

        ProcessFinalDamage(finalDamage, attackElement, damageTypeLog, false);
    }

    private void ProcessFinalDamage(float finalDamage, EElement attackElement, string damageType, bool wasBarrierBlocked)
    {
        if (isDead) return;

        currentHp -= finalDamage;
        Debug.Log($"{enemyName}이(가) [{damageType}] 상태로 {finalDamage}의 피해를 입음! (남은 HP: {currentHp})");

        if (SoundManager.Instance != null)
        {
            ESFXType soundToPlay = ESFXType.EnemyHit_Normal;

            if (wasBarrierBlocked)
            {
                soundToPlay = ESFXType.EnemyHit_Barrier;
            }
            else if (useHitCountOnly)
            {
                soundToPlay = ESFXType.EnemyHit_Star;
            }
            else if (useShieldBlock)
            {
                soundToPlay = ESFXType.EnemyHit_Shield;
            }

            SoundManager.Instance.PlayeSFX(soundToPlay);
        }

        if (currentHp <= 0)
        {
            isDead = true;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StopAllCoroutines();
            StartCoroutine(DefeatedRoutine());
            return;
        }

        if (childHitEffectObject != null)
        {
            if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine);
            hitEffectCoroutine = StartCoroutine(PlayChildHitEffectRoutine());
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (useStunOnHit && finalDamage > 0)
        {
            ApplyStun(0.1f);
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

        if (childShieldObject != null) childShieldObject.SetActive(false);
        if (childBarrierObject != null) childBarrierObject.SetActive(false);
        if (childRegenObject != null) childRegenObject.SetActive(false);
        if (childStarObject != null) childStarObject.SetActive(false);

        if (animator != null)
        {
            animator.SetTrigger("Die");
            yield return new WaitForSeconds(0.3f);

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float remainingTime = Mathf.Max(0f, stateInfo.length - 0.3f);
            yield return new WaitForSeconds(remainingTime);

            animator.speed = 0f;
        }

        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < deathDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / deathDuration;

            if (spriteRenderers != null)
            {
                foreach (var sr in spriteRenderers)
                {
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = Mathf.Lerp(1f, 0f, normalizedTime);
                        sr.color = c;
                    }
                }
            }

            transform.position = new Vector3(startPosition.x, startPosition.y - (normalizedTime * 0.4f), startPosition.z);
            yield return null;
        }

        if (animator != null)
        {
            animator.speed = 1f;
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

    public void ApplyStun(float stunDuration)
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
        if (PlayerHp.Instance != null) PlayerHp.Instance.DecreasePlayerLife(1);
        if (waveManager != null) waveManager.RemoveEnemy(gameObject);
        gameObject.SetActive(false);
    }

    public void RefreshGimmickVisual()
    {
        if (childShieldObject != null) childShieldObject.SetActive(useShieldBlock);
        if (childRegenObject != null) childRegenObject.SetActive(useRegeneration);
        if (childStarObject != null) childStarObject.SetActive(useHitCountOnly);

        if (useMagicBarrier)
        {
            currentBarrierHitCount = barrierHitCount;
            if (childBarrierObject != null) childBarrierObject.SetActive(true);
        }
        else
        {
            if (childBarrierObject != null) childBarrierObject.SetActive(false);
        }
    }

    public void TakePercentageDamage(float percent)
    {
        if (isDead) return;

        float clampedPercent = Mathf.Clamp(percent, 0f, 100f);
        float calculatedDamage = maxHp * (clampedPercent / 100f);

        Debug.Log($"{enemyName}에게 비율 데미지 {clampedPercent}% 발동! (계산된 데미지: {calculatedDamage})");

        ProcessFinalDamage(calculatedDamage, EElement.None, "비율(%) 고정 피해", false);
    }
}