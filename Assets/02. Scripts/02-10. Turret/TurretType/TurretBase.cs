using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField] protected TurretData _turretData;

    protected float _damage;
    protected float _attackRange;
    protected EElement _element;
    protected float _attackCool;
    protected SpriteRenderer _spriteRenderer;
    protected LayerMask _enemyLayerMask;

    private float _lastAttackTime;

    public string TurretName => _turretData.turretName;
    public float Damage => _damage;
    public float AttackRange => _attackRange;
    public int Cost => _turretData.cost;
    public EElement Element => _element;
    public float AttackCool => _attackCool;

    protected void Awake()
    {
        if (_turretData != null)
        {
            _damage = _turretData.damage;
            _attackRange = _turretData.attackRange;
            _attackCool = _turretData.attackCool;
            _element = _turretData.elementType;
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyLayerMask = LayerMask.GetMask(nameof(ELayers.Enemy));

        _lastAttackTime = -_attackCool;
    }
    protected virtual void Update()
    {
        if (Time.time >= _lastAttackTime + _attackCool)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                FlipToTarget(target);
                Attack(target);

                _lastAttackTime = Time.time;
            }
        }

    }

    protected abstract GameObject FindTarget();

    protected abstract void Attack(GameObject target);

    private void FlipToTarget(GameObject target)
    {
        if (_spriteRenderer != null)
        {
            if (target.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }
    }

    public virtual void Upgrade()
    {
        // TowerBuilder에서 업그레이드가 필요해 만들어두었습니다.
    }

    public virtual void GetElement(EElement element)
    {
        // TowerBuilder에서 속성부여가 필요해 만들어두었습니다.
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (_turretData != null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _turretData.attackRange);
#endif
        }
    }
}
