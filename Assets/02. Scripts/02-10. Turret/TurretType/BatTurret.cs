using UnityEngine;

public class BatTurret : TurretBase
{
    [Header("Level3 Speed Bonus Settins")]
    [Tooltip("최대 공속 비율 (0.5f = 50%)")]
    [SerializeField]
    private float _maxSpeedBonus = 0.5f;
    [Tooltip("공속 증가 비율 (0.2f = 20%)")]
    [SerializeField]
    private float _speedIncreaseAmount = 0.2f;

    private GameObject _lastTarget;
    private float _speedBonus = 1f;
    private float _nextAttackTime;

    protected override void Update()
    {
        if (_isUpgrade)
        {
            if (Time.time >= _nextAttackTime)
            {
                GameObject target = FindTarget();
                if (target != null && target.activeSelf)
                {
                    FlipToTarget(target);
                    Attack(target);

                    float coolTime = _attackCool / _speedBonus;
                    _nextAttackTime = Time.time + coolTime;
                }
                else
                {
                    _speedBonus = 1f;
                    _lastTarget = null;

                    _nextAttackTime = Time.time;
                }
            }
        }
        else
        {
            base.Update();
        }

    }

    protected override GameObject FindTarget()
    {
        if(_lastTarget != null&& _lastTarget.activeSelf)
        {
            float distance = Vector2.Distance(transform.position, _lastTarget.transform.position);
            if (distance <= AttackRange)
            {
                return _lastTarget;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, _enemyLayerMask);
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
        if(target == null)
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
            enemy.TakeDamage(Damage, _element);
            _lastTarget = target;

            Vector2 dir = (target.transform.position - transform.position).normalized;
            Vector3 pos = transform.position + (Vector3)(dir * 2f);
            float angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0,0, angle);

            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.PlayEffect(EffectType(_element), pos, rot,0.5f);
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayeSFX(ESFXType.BatHit);
            }
        }
    }

    private EEffectType EffectType(EElement element)
    {
        switch(element)
        {
            case EElement.Fire:
                return EEffectType.HitBat_Fire;
            case EElement.Water:
                return EEffectType.HitBat_Water;
            case EElement.Grass:
                return EEffectType.HitBat_Grass;
            case EElement.Electric:
                return EEffectType.HitBat_Electric;
            default:
                return EEffectType.HitBat_None;
        }
    }
}
