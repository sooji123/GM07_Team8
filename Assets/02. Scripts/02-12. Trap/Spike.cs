using UnityEngine;

public class Spike : TrapBase
{
    [SerializeField]
    private float _damage;

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

        if (target.TryGetComponent<EnemyBase>(out EnemyBase enemy)) 
        {
            enemy.TakeDamage(_damage, EElement.None);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.Spike);
        }
    }
}
