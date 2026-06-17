using UnityEngine;

public class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField] protected TurretData turretData;

    protected float _damage;
    protected float _attackRange;
    protected EElement _element;
    protected float _attackCool;
    protected SpriteRenderer _spriteRenderer;
    
    private float _lastAttackTime;

    public string TurretName => turretData.turretName;
    public float Damage => turretData.damage;
    public float AttackRange => turretData.attackRange;
    public int Cost => turretData.cost;
    public EElement Element => turretData.elementType;

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
    }
    protected virtual void Update()
    {
        GameObject target = FindTarget();
        if (target != null)
        {
            if (Time.time >= _lastAttackTime + _attackCool)
            {
                Attack(target);
                _lastAttackTime = Time.time;
            }
        }
    }

    protected virtual GameObject FindTarget()
    {
        return null;
    }

    protected virtual void Attack(GameObject target)
    {

    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (turretData != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, turretData.attackRange); 
        }
    }
}
