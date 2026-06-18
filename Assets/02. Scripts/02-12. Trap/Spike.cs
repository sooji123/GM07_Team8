using UnityEngine;

public class Spike : TrapBase
{
    protected float _damage;

    private Animator _animator;

    public float Damage => _damage;
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }
    protected override void ActiveTrap(GameObject target)
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        EnemyBase enemyBase = target.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            //target.TakeDamage(_damage, _elementType);
        }
    }
}
