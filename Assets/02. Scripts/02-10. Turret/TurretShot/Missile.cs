using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 10f;
    [SerializeField] 
    private float _lifeTime = 5f;

    private float _damage;
    private EElement _element;
    private GameObject _target;
    private SpriteRenderer _spriteRenderer;
    private bool _isReleased = false;

    private void OnEnable()
    {
        _isReleased = false;
    }
    public void Initialize(float damage, EElement element, GameObject target)
    {
        _damage = damage;
        _element = element;
        _target = target;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) {
            _spriteRenderer.color = ElementColor.GetElementColor(element);
        }
    }

    private void Update()
    {
        if (_target != null && _target.activeSelf) 
        {
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            if (TryGetComponent<PoolAble>(out PoolAble poolAble))
            {
                poolAble.ReleaseObject();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isReleased) 
        {
            return;
        }

        if (collision.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            _isReleased = true;

            EffectManager.Instance.PlayEffect(EffectType(_element), transform.position, Quaternion.identity, 0.5f);

            enemy.TakeDamage(_damage, _element);

            if (TryGetComponent<PoolAble>(out PoolAble poolAble))
            {
                poolAble.ReleaseObject();
            }
        }
    }

    private EEffectType EffectType(EElement element)
    {
        switch (element)
        {
            case EElement.Fire:
                return EEffectType.Explosion_Fire;
            case EElement.Water:
                return EEffectType.Explosion_Water;
            case EElement.Grass:
                return EEffectType.Explosion_Grass;
            case EElement.Electric:
                return EEffectType.Explosion_Electric;
            default:
                return EEffectType.Explosion_None;
        }
    }
}
