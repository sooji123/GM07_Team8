using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _range = 1f;

    private float _damage;
    private EElement _element;
    private Vector2 _target;

    public void Initialize(float damage, EElement element, Vector2 targetPosition)
    {
        _damage = damage;
        _element = element;
        _target = targetPosition;
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
            //에너미에게 데미지, 속성 전달필요
        }

        //이펙트 추가 필요
        Destroy(gameObject);//풀링시스템추가필요
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.orange;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _range);
#endif
    }
}
