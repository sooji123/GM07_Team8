using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private HashSet<ETurretType> _upgradeTurretTypes = new HashSet<ETurretType>();

    public static event Action<ETurretType> OnTurretTypeUpgraded;

    public bool IsUpgraded(ETurretType upgradeTurret)
    {
        return _upgradeTurretTypes.Contains(upgradeTurret);
    }

    public void UpgradeTurretType(ETurretType turret)
    {
        if (_upgradeTurretTypes.Add(turret))
        {
            OnTurretTypeUpgraded?.Invoke(turret);
        }
    }
}
