using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    [Header("--- HP_Bar 오브젝트 ---")]
    [SerializeField] private Transform hpBarTransform;

    private EnemyBase enemyBase;
    private Vector3 originalScale;
    private bool isInitialized = false;

    void Awake()
    {
        InitializeIfNeeded();
    }

    void OnEnable()
    {
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized) return;

        enemyBase = GetComponent<EnemyBase>();

        if (hpBarTransform != null)
        {
            originalScale = hpBarTransform.localScale;
            isInitialized = true;
        }
    }

    void Update()
    {
        if (!isInitialized || enemyBase == null || hpBarTransform == null) return;
        if (enemyBase.maxHp <= 0f) return;

        float hpRatio = Mathf.Clamp01(enemyBase.currentHp / enemyBase.maxHp);

        hpBarTransform.localScale = new Vector3(
            originalScale.x * hpRatio,
            originalScale.y,
            originalScale.z
        );

        if (hpRatio >= 1f || hpRatio <= 0f)
        {
            if (hpBarTransform.gameObject.activeSelf)
                hpBarTransform.gameObject.SetActive(false);
        }
        else
        {
            if (!hpBarTransform.gameObject.activeSelf)
                hpBarTransform.gameObject.SetActive(true);
        }
    }
}