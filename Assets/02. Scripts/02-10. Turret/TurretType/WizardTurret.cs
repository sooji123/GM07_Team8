using UnityEngine;

public class WizardTurret : TurretBase
{
    [SerializeField] 
    private GameObject _bombPrefab;

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
        if (target == null || !target.activeSelf)
        {
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 pos = transform.position + dir * 1f;

        if (_bombPrefab != null)
        {
            GameObject shot = PoolManager.Instance.GetGo(_bombPrefab.name);
            shot.transform.position = pos;
            shot.transform.rotation = Quaternion.identity;

            Bomb bomb = shot.GetComponent<Bomb>();

            if (bomb != null) 
            {
                bomb.Initialize(Damage, _element, target.transform.position, CurrentLevel);
            }

            SoundManager.Instance.PlayeSFX(ESFXType.WizardHit);
        }
    }
}
