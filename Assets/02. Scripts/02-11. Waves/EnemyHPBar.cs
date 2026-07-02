using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    [Header("--- HP_Bar 오브젝트 ---")]
    [SerializeField] private Transform hpBarTransform;

    private EnemyBase enemyBase;
    private Vector3 originalScale;

    void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();

        if (hpBarTransform != null)
        {
            originalScale = hpBarTransform.localScale;
        }
    }

    void OnEnable()
    {
        if (hpBarTransform != null)
        {
            hpBarTransform.localScale = originalScale;
        }
    }

    public void UpdateHPBar()
    {
        if (enemyBase == null || hpBarTransform == null) return;

        float hpRatio = Mathf.Max(0, enemyBase.currentHp) / enemyBase.maxHp;

        hpBarTransform.localScale = new Vector3(originalScale.x * hpRatio, originalScale.y, originalScale.z);
    }
}