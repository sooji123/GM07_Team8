using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    [Header("Trap Data")]
    [SerializeField] protected TurretData _turretData;

    protected float _damage;
    protected float _attackCool;
    protected EElement _elementType;

    private float _lastAttackTime;

    public string TurretName => _turretData.turretName;
    public float Damage => _damage;
    public int Cost => _turretData.cost;

    protected void Awake()
    {
        if (_turretData != null)
        {
            _damage = _turretData.damage;
            _attackCool = _turretData.attackCool;
            _elementType = _turretData.elementType;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) //수정필요
        {
            Attack();
        }
    }

    protected abstract void Attack();
}
