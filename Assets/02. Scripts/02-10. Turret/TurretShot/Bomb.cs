using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] 
    private float _moveSpeed = 10f;
    [SerializeField] 
    private float _range = 1f;

    private float _damage;
    private EElement _element;
    private Vector2 _target;
    private PoolAble _poolAble;
    private SpriteRenderer _spriteRenderer;

    public void Initialize(float damage, EElement element, Vector2 targetPosition)
    {
        _damage = damage;
        _element = element;
        _target = targetPosition;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = ElementColor.GetElementColor(element);
        }
    }

    private void Update()
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (_target - currentPos).normalized;

        float distanceToTarget = Vector2.Distance(currentPos, _target);
        float moveDistance = _moveSpeed * Time.deltaTime;

        if (moveDistance >= distanceToTarget)
        {
            transform.position = _target;
            Explode();
        }
        else
        {
            transform.position += (Vector3)direction * moveDistance;
        }
    }

    private void Explode()
    {
        int enemyLayerMask = LayerMask.GetMask(nameof(ELayers.Enemy));

        Collider2D[] hits = Physics2D.OverlapCircleAll(_target, _range, enemyLayerMask);

        foreach (var hit in hits)
        {
            if(hit.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                enemy.TakeDamage(_damage, _element);
            }
        }

        //이펙트 추가 필요
        _poolAble = GetComponent<PoolAble>();
        _poolAble.ReleaseObject();
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.orange;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _range);
#endif
    }
}
