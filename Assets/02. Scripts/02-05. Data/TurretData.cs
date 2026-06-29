using System;
using System.Collections.Generic;
using UnityEngine;

#region TurretLevelStat
/*[Serializable]
public class TurretLevelStat
{
    public float damage;
    public float attckRange;
    public float attackCool;
    public int upgradeCost;
}*/
#endregion

[CreateAssetMenu(fileName = "TurretData", menuName = "ScriptableObjects/TurretData")]
public class TurretData : ScriptableObject
{
    [Header("Turret Info")]
    public string turretName;
    public ETurretType turretType;
    [TextArea]
    public string description;
    public int cost;
    public EElement elementType;

    [Header("Turret Stats")]
    public float damage;
    public float attackCool;
    public float attackRange;
    /*[Header("Turret Level Stats")]
    public TurretLevelStat level1Stat;
    public TurretLevelStat level2Stat;
    public TurretLevelStat level3Stat;*/

    [Header("Shop")]
    public Sprite turretIcon;
    public GameObject turretPrefab;

    /*public TurretLevelStat GetStat(int level)
    {
        switch (level)
        {
            case 1:
                return level1Stat;
            case 2:
                return level2Stat;
            case 3:
                return level3Stat;
            default:
                return level1Stat;
        }
    }*/
}
