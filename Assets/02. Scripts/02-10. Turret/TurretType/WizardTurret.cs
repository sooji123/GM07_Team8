using UnityEngine;

public class WizardTurret : TurretBase
{
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _shotPoint;

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
    protected override void Attack(GameObject target)
    {
        if (_bombPrefab != null && _shotPoint != null)
        {
            GameObject shot = PoolManager.Instance.GetGo(_bombPrefab.name);
            shot.transform.position = _shotPoint.position;
            shot.transform.rotation = Quaternion.identity;

            Bomb bomb = shot.GetComponent<Bomb>();

            if (bomb != null) 
            {
                bomb.Initialize(_damage, _element, target.transform.position);
            }
        }
    }
}
