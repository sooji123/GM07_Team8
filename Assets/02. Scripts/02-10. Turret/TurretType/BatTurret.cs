using UnityEngine;

public class BatTurret : TurretBase
{
    [SerializeField] private Transform _shotPoint;

    protected override GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayerMask);
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = hit.gameObject;
            }
        }
        return nearestEnemy;
    }
    protected override void Attack(GameObject target)
    {
        if (_shotPoint != null) 
        { 
            if(target.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                //enemy.TakeDamage(_damage, _element);
            }
        }
    }
}
