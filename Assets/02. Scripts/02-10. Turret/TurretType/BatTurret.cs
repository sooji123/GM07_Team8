using UnityEngine;

public class BatTurret : TurretBase
{
    [Header("타워 기믹 업그레이드 - 레벨2")]
    [Tooltip("최대 공속 비율 (0.5f = 50%)")]
    [SerializeField]
    private float _maxSpeedBonus2 = 0.5f;
    [Tooltip("공속 증가 비율 (0.2f = 20%)")]
    [SerializeField]
    private float _speedIncreaseAmount2 = 0.15f;
    [Header("타워 기믹 업그레이드 - 레벨3")]
    [Tooltip("최대 공속 비율 (0.5f = 50%)")]
    [SerializeField]
    private float _maxSpeedBonus3 = 1f;
    [Tooltip("공속 증가 비율 (0.2f = 20%)")]
    [SerializeField]
    private float _speedIncreaseAmount3 = 0.35f;

    private GameObject _lastTarget;
    private float _speedBonus = 1f;
    private float _nextAttackTime;

    protected override void Update()
    {
        if (CurrentLevel>=2)
        {
            if (Time.time >= _nextAttackTime)
            {
                GameObject target = FindTarget();
                if (target != null && target.activeSelf)
                {
                    FlipToTarget(target);
                    Attack(target);

                    float coolTime = AttackCool / _speedBonus;
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

        if(CurrentLevel >= 2)
        {
            float maxSpeed = (CurrentLevel >= 3)? _maxSpeedBonus3 : _maxSpeedBonus2;
            float amount = (CurrentLevel >= 3) ? _speedIncreaseAmount3 : _speedIncreaseAmount2;

            if (_lastTarget == target)
            {
                _speedBonus = Mathf.Min(_speedBonus + amount, 1 + maxSpeed);
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
