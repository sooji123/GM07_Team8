using UnityEngine;

public class TestTower : MonoBehaviour
{
    [Header("--- 총알 및 발사 위치 설정 ---")]
    [SerializeField] private GameObject _shotPrefab;
    [SerializeField] private Transform _shotPoint;

    [Header("--- 타워 능력치 설정 ---")]
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _damage = 20f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private EElement _element = EElement.None;

    private float _cooldownTimer = 0f;

    void Update()
    {
        _cooldownTimer += Time.deltaTime;

        if (_cooldownTimer >= _attackCooldown)
        {
            GameObject target = FindTarget();

            if (target != null)
            {
                Attack(target);
                _cooldownTimer = 0f;
            }
        }
    }

    private GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayerMask);
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = hit.gameObject;
            }
        }
        return nearestEnemy;
    }

    private void Attack(GameObject target)
    {
        if (_shotPrefab != null && _shotPoint != null)
        {
            GameObject shot = Instantiate(_shotPrefab, _shotPoint.position, Quaternion.identity);

            TestMissile missile = shot.GetComponent<TestMissile>();

            if (missile != null)
            {
                missile.Initialize(_damage, _element, target.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}