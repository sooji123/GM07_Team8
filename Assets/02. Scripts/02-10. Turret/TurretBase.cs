using UnityEngine;

public class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField] protected TurretData turretData;

    protected float _damage;
    protected float _attackRange;
    protected EElement _element;
    protected float _attackCool;
    protected float lastAttackTime;

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
    }
    protected virtual void Update()
    {
        GameObject target = FindTarget();
        if (target != null)
        {
            if (Time.time >= lastAttackTime + _attackCool)
            {
                Attack(target);
                lastAttackTime = Time.time;
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
        if (turretData != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, turretData.attackRange); 
        }
    }
}
