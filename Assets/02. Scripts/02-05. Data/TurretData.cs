using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "ScriptableObjects/TurretData")]
public class TurretData : ScriptableObject
{
    [Header("Turret Info")]
    public string turretName;
    [TextArea]
    public string description;
    public int cost;
    public int upgradeCost;
    public bool isUpgrade;

    [Header("Turret Stat")]
    public float damage;
    public float attackRange;
    public float attackCool;
    public EElement elementType;

    [Header("Shop")]
    public Sprite turretIcon;
    public GameObject turretPrefab;
}
