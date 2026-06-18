using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/TrapData")]
public class TrapData : ScriptableObject
{
    [Header("Trap Info")]
    public string trapName;
    [TextArea]
    public string description;
    public int cost;

    [Header("Trap Stat")]
    public float damage;
    public float attackCool;
    public EElement elementType;

    [Header("Shop")]
    public Sprite trapIcon;
    public GameObject trapPrefab;
}
