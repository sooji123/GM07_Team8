using UnityEngine;

public class UI_TurretControlWindow : MonoBehaviour
{
    private TurretBase _targetTurret;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Open(TurretBase turret)
    {
        _targetTurret = turret;
    }
}
