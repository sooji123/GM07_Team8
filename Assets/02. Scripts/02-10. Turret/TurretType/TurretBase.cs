using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField] protected TurretData turretData;

    protected float _damage;
    protected float _attackRange;
    protected EElement _element;
    protected float _attackCool;
    protected SpriteRenderer _spriteRenderer;
    protected LayerMask _enemyLayerMask;

    private float _lastAttackTime;

    public string TurretName => turretData.turretName;
    public float Damage => _damage;
    public float AttackRange => _attackRange;
    public int Cost => turretData.cost;
    public EElement Element => _element;

    protected void Awake()
    {
        if (turretData != null)
        {
            _damage = turretData.damage;
            _attackRange = turretData.attackRange;
            _attackCool = turretData.attackCool;
            _element = turretData.elementType;
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyLayerMask = LayerMask.GetMask("Enemy"); //layer값 가져오는 기능으로 수정 필요
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
            if(target.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (turretData != null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, turretData.attackRange);
#endif
        }
    }
}
