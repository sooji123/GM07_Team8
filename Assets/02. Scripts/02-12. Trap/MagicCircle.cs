using UnityEngine;

public class MagicCircle : TrapBase
{
    [SerializeField] 
    private float _slowAmount = 0.5f;
    [SerializeField] 
    private float _slowDuration = 2f;

    protected override void ActiveTrap(GameObject target)
    {
        if(target.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ApplySlow(_slowDuration, _slowAmount);
        }
        if (SoundManager.Instance != null) 
        {
            SoundManager.Instance.PlayeSFX(ESFXType.MagicCircle);
        }
    }
}
