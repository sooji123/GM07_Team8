using UnityEngine;

public class SunflowerTurret : TurretBase
{
    [SerializeField] private GameObject _shotPrefab;
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
        if (_shotPrefab != null && _shotPoint != null)
        {
            //GameObject shot = Instantiate(_shotPrefab, _shotPoint.position, Quaternion.identity);
            GameObject shot = PoolManager.Instance.GetGo(_shotPrefab.name);
            Missile missile = shot.GetComponent<Missile>();

            if (missile != null) 
            {
                missile.Initialize(_damage, _element, target.transform);
            }
        }
    }
}
