using System;
using UnityEngine;

#region TurretLevel
[Serializable]
public class TurretLevel
{
    public string level;
    public string explanation;
    public string cost;
}
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

    /*[Header("Turret Stats")]
    public float damage;
    public float attackCool;
    public float attackRange;*/

    [Header("Shop")]
    public Sprite turretIcon;
    public GameObject turretPrefab;

    [Header("Upgrade")]
    public TurretLevel[] turretLevels;
}
