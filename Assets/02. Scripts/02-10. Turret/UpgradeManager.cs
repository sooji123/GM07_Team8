using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private Dictionary<ETurretType, int> _turretUpgradeLevels = new Dictionary<ETurretType,int>();

    public static event Action<ETurretType, int> OnTurretTypeUpgraded;
    public static event Action OnUpgradesReset;
    public int GetUpgradeLevel(ETurretType turret)
    {
        if(_turretUpgradeLevels.TryGetValue(turret, out int level))
        {
            return level;
        }

        return 1;
    }

    public void UpgradeTurretType(ETurretType turret)
    {
        int currentLevel = GetUpgradeLevel(turret);
        if(currentLevel >= 3)
        {
            return;
        }

        _turretUpgradeLevels[turret] = currentLevel + 1;

        OnTurretTypeUpgraded?.Invoke(turret, currentLevel + 1);
    }

    public void ResetUpgrade()
    {
        _turretUpgradeLevels.Clear();
        OnUpgradesReset?.Invoke();
    }
}
