using UnityEngine;

public class UI_SpeedHUD : MonoBehaviour
{
    public void OnClickDoubleSpeed()
    {
        UI_Manager.Instance.DoubleSpeedUp();
    }
}
