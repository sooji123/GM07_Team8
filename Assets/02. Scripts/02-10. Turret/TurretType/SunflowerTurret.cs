using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunflowerTurret : TurretBase
{
    [SerializeField] 
    private GameObject _missilePrefab;
    [SerializeField] 
    private Transform _shotPoint;
    [SerializeField]
    private float _doubleShotDuration = 0.3f;

    protected override GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, _enemyLayerMask);
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
        if (_missilePrefab != null && _shotPoint != null)
        {
            StartCoroutine(AttackCo(target));
        }
    }
    private IEnumerator AttackCo(GameObject target) 
    {
        int shotCount;
        switch (CurrentLevel) {
            case 1:
                shotCount = 1;
                break;
            case 2:
                shotCount = 2;
                break;
            case 3:
                shotCount = 3;
                break;
            default:
                shotCount = 1;
                break;
        }

        List<GameObject> targetEnemies = new List<GameObject>();
        if (target != null && target.activeSelf) 
        { 
            targetEnemies.Add(target);
        }
        if (shotCount > 1)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, _enemyLayerMask);
            foreach(var hit in hits)
            {
                if (targetEnemies.Count >= shotCount)
                {
                    break;
                }

                if (hit.gameObject.activeSelf && hit.gameObject != target)
                {
                    targetEnemies.Add(target.gameObject);
                }
            }
        }
        for (int i = 0; i < targetEnemies.Count; i++) 
        {
            FireMissile(targetEnemies[i]);
        }

        int remainShot = shotCount - targetEnemies.Count;
        for (int i = 0; i < remainShot; i++) 
        {
            yield return new WaitForSeconds(_doubleShotDuration);
            if (target != null && target.activeSelf) 
            {
                FireMissile(target);
            }
        }
    }

    private void FireMissile(GameObject target)
    {
        GameObject shot = PoolManager.Instance.GetGo(_missilePrefab.name);
        shot.transform.position = _shotPoint.position;
        shot.transform.rotation = Quaternion.identity;

        if(shot.TryGetComponent<Missile>(out var missile))
        {
            missile.Initialize(Damage, _element, target);
        }
    }
}
