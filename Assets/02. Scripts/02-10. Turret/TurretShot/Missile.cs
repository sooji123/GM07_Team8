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
        if (collision.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.TakeDamage(_damage, _element);

            if (TryGetComponent<PoolAble>(out PoolAble poolAble))
            {
                poolAble.ReleaseObject();
            }
        }
    }

}
