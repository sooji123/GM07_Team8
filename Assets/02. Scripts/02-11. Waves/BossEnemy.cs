using UnityEngine;
using System.Collections;

public class BossEnemy : EnemyBase
{
    [Header("--- 보스 전용 설정 : 골인 지점 ---")]
    public Transform goalTransform;

    [Header("--- 스킬 쿨다운 ---")]
    public float healCooldown = 7f;

    [Header("--- 힐 이펙트 ---")]
    public GameObject goldHealEffectObject;
    public float maxEffectScale = 15f;
    public float effectDuration = 0.8f;

    private bool isBossSetupComplete = false;
    private SpriteRenderer[] effectSprites;

    protected override void Start()
    {
        base.Start();

        currentHp = maxHp;

        if (goldHealEffectObject != null)
        {
            effectSprites = goldHealEffectObject.GetComponentsInChildren<SpriteRenderer>();
            goldHealEffectObject.SetActive(false);
        }

        if (goalTransform == null)
        {
            GameObject goalObj = GameObject.Find("WayPoints/WayPointForBossMob");
            if (goalObj != null)
            {
                goalTransform = goalObj.transform;
            }
        }

        if (goalTransform != null)
        {
            wayPoints = new Transform[] { goalTransform };
            isBossSetupComplete = true;
            Debug.Log($"{enemyName} 보스 Hp: {currentHp}/{maxHp} 세팅 완료! 목푯값 직진 시작.");
        }
        else
        {
            Debug.LogError($"[보스 에러] 'WayPoints/WayPointForBossMob' 객체를 찾을 수 없습니다!");
        }

        StartCoroutine(HealAllEnemiesRoutine());
    }

    public override void TakeDamage(float damage, EElement attackElement)
    {
        if (currentHp <= 0 || !isBossSetupComplete) return;

        float finalDamage = damage - armor;
        if (finalDamage < 1) finalDamage = 1;

        currentHp -= finalDamage;
        Debug.Log($"[보스 피격] {enemyName}이(가) {finalDamage}의 피해를 입음! (남은 HP: {currentHp}/{maxHp})");

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.EnemyHit_Normal);
        }

        Animator bossAnim = GetComponentInChildren<Animator>();
        if (bossAnim != null)
        {
            bossAnim.SetTrigger("Hit");
        }

        if (childHitEffectObject != null)
        {
            StartCoroutine(PlayBossHitEffectRoutine());
        }

        if (currentHp <= 0)
        {
            currentHp = 0;
            base.TakeDamage(99999f, EElement.None);
        }
    }

    void Update()
    {
        if (currentHp <= 0 || !isBossSetupComplete || goalTransform == null) return;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers != null)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null && sr.gameObject != goldHealEffectObject && !sr.transform.IsChildOf(goldHealEffectObject.transform))
                {
                    sr.flipX = (goalTransform.position.x > transform.position.x);
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, goalTransform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, goalTransform.position) < 0.1f)
        {
            HandleReachGoal();
        }
    }

    private IEnumerator HealAllEnemiesRoutine()
    {
        while (currentHp > 0)
        {
            yield return new WaitForSeconds(healCooldown);

            if (currentHp <= 0) break;

            if (waveManager != null)
            {
                Animator bossAnim = GetComponentInChildren<Animator>();
                if (bossAnim != null)
                {
                    bossAnim.SetTrigger("Heal");
                }

                if (goldHealEffectObject != null)
                {
                    StartCoroutine(ExpandGoldHealEffectRoutine());
                }

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayeSFX(ESFXType.Enemy_BossHeal);
                }

                Debug.Log("[보스 스킬 발동] 전장에 생존한 모든 몬스터의 HP를 100% 회복합니다!");

                EnemyBase[] allEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

                foreach (EnemyBase enemy in allEnemies)
                {
                    if (enemy != null && enemy != this && enemy.gameObject.activeSelf)
                    {
                        enemy.currentHp = enemy.maxHp;
                    }
                }
            }
        }
    }

    private IEnumerator ExpandGoldHealEffectRoutine()
    {
        goldHealEffectObject.SetActive(true);

        float elapsedTime = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = new Vector3(maxEffectScale, maxEffectScale, 1f);

        SetEffectAlpha(1f);

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / effectDuration;

            goldHealEffectObject.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
            SetEffectAlpha(Mathf.Lerp(1f, 0f, progress));

            yield return null;
        }

        goldHealEffectObject.SetActive(false);
        goldHealEffectObject.transform.localScale = Vector3.zero;
    }

    private void SetEffectAlpha(float alpha)
    {
        if (effectSprites == null) return;

        foreach (var sprite in effectSprites)
        {
            if (sprite != null)
            {
                Color c = sprite.color;
                c.a = alpha;
                sprite.color = c;
            }
        }
    }

    private IEnumerator PlayBossHitEffectRoutine()
    {
        childHitEffectObject.SetActive(true);
        yield return new WaitForSeconds(hitEffectDuration);
        childHitEffectObject.SetActive(false);
    }
}