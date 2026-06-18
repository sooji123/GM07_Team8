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
            //target 에너미의 컴포넌트를 가져와서 damage전달
            //이펙트만 잠깐 보여주기
        }
    }
}
