using System.Collections;
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attckRange, _enemyLayerMask);
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

    private GameObject FindSecondTarget(GameObject firstTarget)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attckRange, _enemyLayerMask);
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if(hit.gameObject== firstTarget)
            {
                continue;
            }

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
        FireMissile(target);

        if (_currentLevel == 3)
        {
            GameObject secondTarget = FindSecondTarget(target);

            if (secondTarget != null && secondTarget.activeSelf)
            {
                FireMissile(secondTarget);
            }
            else
            {
                if(target!=null&& target.activeSelf)
                {
                    yield return new WaitForSeconds(_doubleShotDuration);

                    FireMissile(target);
                }
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
