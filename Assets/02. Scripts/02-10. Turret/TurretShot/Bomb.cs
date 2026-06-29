using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Setting")]
    [SerializeField] 
    private float _moveSpeed = 10f;
    [SerializeField] 
    private float _range = 1f;
    [Header("Level 3 Bomb Setting")]
    [SerializeField]
    private int _childBombCount = 3;
    [SerializeField]
    private float _spreadRadius = 1.5f;
    [SerializeField]
    private float _childDamageRatio = 0.5f;
    [SerializeField]
    private float _childRange = 0.6f;
    [SerializeField]
    private float _childSize = 0.7f;

    private float _damage;
    private EElement _element;
    private Vector2 _target;
    private SpriteRenderer _spriteRenderer;

    private bool _isLevel3;
    private bool _isChild;

    public void Initialize(float damage, EElement element, Vector2 targetPosition, bool isLevel3, bool isChild = false)
    {
        _damage =  damage;
        _element = element;
        _target = targetPosition;
        _isLevel3 = isLevel3;
        _isChild = isChild;

        transform.localScale = isChild ? new Vector3(_childSize, _childSize, _childSize) : Vector3.one;

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
        float range = _isChild ? _childRange : _range;
        float damage = _isChild ? _damage * _childDamageRatio : _damage;
        int enemyLayerMask = LayerMask.GetMask(nameof(ELayers.Enemy));

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayerMask);

        foreach (var hit in hits)
        {
            if(hit.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                enemy.TakeDamage(damage, _element);
            }
        }

        if (!_isChild && _isLevel3)
        {
            SpawnChildBomb();
        }

        EffectManager.Instance.PlayEffect(EffectType(_element), transform.position, Quaternion.identity, 0.5f);

        if (TryGetComponent<PoolAble>(out PoolAble poolAble))
        {
            poolAble.ReleaseObject();
        }
    }

    private void SpawnChildBomb()
    {
        float angleStep = 360f / _childBombCount;
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < _childBombCount; i++)
        {
            GameObject shot = PoolManager.Instance.GetGo("Bomb");
            if (shot == null) continue;

            shot.transform.position = transform.position;
            shot.transform.rotation = Quaternion.identity;

            if (shot.TryGetComponent<Bomb>(out Bomb childBomb))
            {
                float currentAngle = startAngle + (angleStep * i);

                float radian = currentAngle * Mathf.Deg2Rad;

                Vector2 spreadOffset = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * _spreadRadius;
                Vector2 childTargetPos = (Vector2)transform.position + spreadOffset;

                childBomb.Initialize(_damage, _element, childTargetPos, _isLevel3, true );
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


    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.orange;
        float currentRange = _isChild ? _childRange : _range;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, currentRange);
#endif
    }
}
