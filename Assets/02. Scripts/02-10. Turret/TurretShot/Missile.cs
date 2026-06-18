using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _lifeTime = 5f;

    private float _damage;
    private EElement _element;
    private Transform _target;
    private PoolAble _poolAble;
    private void Start()
    {
        _poolAble = GetComponent<PoolAble>();
    }
    public void Initialize(float damage, EElement element, Transform targetTransform)
    {
        _damage = damage;
        _element = element;
        _target = targetTransform;
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
            Destroy(gameObject); //풀링시스템 필요함
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(nameof(ETags.Enemy)))
        {
            //에너미에게 속성과 데미지 넘겨야함.

            Destroy(gameObject);
        }
    }
}
