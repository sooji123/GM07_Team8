using UnityEngine;

public class BatTurret : TurretBase
{
    [SerializeField] 
    private Transform _shotPoint;
    [Header("Level3 Sped Bonus Settins")]
    [Tooltip("최대 공속 비율 (0.5f = 50%)")]
    [SerializeField]
    private float _maxSpeedBonus = 0.5f;
    [Tooltip("공속 증가 비율 (0.2f = 20%)")]
    [SerializeField]
    private float _speedIncreaseAmount = 0.2f;

    private GameObject _lastTarget;
    private float _speedBonus = 1f;
    private float _attackTime;

    protected override void Update()
    {
        if (Time.time >= _attackTime) 
        {
            GameObject target = FindTarget();
            if (target!= null && target.activeSelf)
            {
                FlipToTarget(target);
                Attack(target);

                float coolTime = _attackCool / _speedBonus;
                _attackTime = Time.time + coolTime;
            }
            else
            {
                _speedBonus = 1f;
                _lastTarget = null;
                _attackTime = Time.time + _attackCool;
            }
        }
    }

    protected override GameObject FindTarget()
    {
        if(_lastTarget != null&& _lastTarget.activeSelf)
        {
            float distance = Vector2.Distance(transform.position, _lastTarget.transform.position);
            if (distance <= _attckRange)
            {
                return _lastTarget;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attckRange, _enemyLayerMask);
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
    protected override void Attack(GameObject target)
    {
        if(target == null || _shotPoint == null)
        {
            return;
        }

        if(_currentLevel == 3)
        {
            if (_lastTarget == target)
            {
                _speedBonus = Mathf.Min(_speedBonus + _speedIncreaseAmount, 1 + _maxSpeedBonus);
            }
            else
            {
                _speedBonus = 1.0f;
            }
        }

        if (target.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.TakeDamage(_damage, _element);
            _lastTarget = target;
        }
    }
}
