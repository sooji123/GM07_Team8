using System.Collections.Generic;
using UnityEngine;

public class BuffTurret : TurretBase
{
    [Header("Buff Settings")]
    [SerializeField] private LayerMask _turretLayerMask;
    [SerializeField] private float _buffDamageAmount;
    [SerializeField] private float _buffAttackCoolAmount;
    [SerializeField] private float _buffAttackRangeAmount;

    private List<TurretBase> _buffedTurrets = new List<TurretBase>();

    private void Start()
    {
        if(EffectManager.Instance != null && SoundManager.Instance != null)
        {
            EffectManager.Instance.PlayEffect(EEffectType.Buff, transform.position, Quaternion.identity, 0.7f);
            SoundManager.Instance.PlayeSFX(ESFXType.Buff);
        }
    }
    protected override void Update()
    {
        UpdateBuff();
    }

    private void UpdateBuff()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attckRange, _turretLayerMask);

        List<TurretBase> currentTurretsInRange = new List<TurretBase>();

        foreach (var hit in hits) { 
            if(hit.gameObject == gameObject)
            {
                continue;
            }
            if(hit.TryGetComponent<TurretBase>(out TurretBase turret))
            {
                currentTurretsInRange.Add(turret);

                if (!_buffedTurrets.Contains(turret))
                {
                    turret.AddBuff(_buffDamageAmount, _buffAttackCoolAmount, _buffAttackRangeAmount);
                    _buffedTurrets.Add(turret);
                }
            }
        }
        for (int i = _buffedTurrets.Count - 1; i >= 0; i--) { 
            TurretBase turret = _buffedTurrets[i];
            
            if(turret == null)
            {
                _buffedTurrets.RemoveAt(i);
                continue;
            }

            if (!currentTurretsInRange.Contains(turret)) { 
                turret.RemoveBuff(_buffDamageAmount, _buffAttackCoolAmount, _buffAttackRangeAmount);
                _buffedTurrets.RemoveAt(i);
            }
        }
    }

    private void OnDisable()
    {
        foreach(TurretBase turret in _buffedTurrets)
        {

            if(turret != null)
            {
                turret.RemoveBuff(_buffDamageAmount, _buffAttackCoolAmount, _buffAttackRangeAmount);
            }
        }
    }
}
