using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _lifeTime = 5f;

    private float _damage;
    private EElement _element;
    private Transform _target;
    private PoolAble _poolAble;
    private SpriteRenderer _spriteRenderer;

    public void Initialize(float damage, EElement element, Transform targetTransform)
    {
        _damage = damage;
        _element = element;
        _target = targetTransform;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) {
            _spriteRenderer.color = ElementColor.GetElementColor(element);
        }
    }

    private void Update()
    {
        if (_target != null) 
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            _poolAble = GetComponent<PoolAble>();
            _poolAble.ReleaseObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.TakeDamage(_damage, _element);

            _poolAble = GetComponent<PoolAble>();
            _poolAble.ReleaseObject();
        }
    }

}
