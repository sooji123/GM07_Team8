using UnityEngine;

public class MagicCircle : TrapBase
{
    [SerializeField] private float _slowAmount = 0.5f;
    [SerializeField] private float _slowDuration = 2f;

    //¿Ã∆Â∆Æ

    protected override void ActiveTrap(GameObject target)
    {
        EnemyBase enemy = target.GetComponent<EnemyBase>();
        if (enemy != null) 
        {
            enemy.ApplySlow(_slowAmount, _slowDuration);
        }
    }
}
