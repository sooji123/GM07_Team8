using TMPro;
using UnityEngine;

public class UI_SpeedHUD : MonoBehaviour
{
    [Header("Text")]
    [SerializeField]
    private GameObject _speed1;
    [SerializeField]
    private GameObject _speed2;
    public void OnClickDoubleSpeed()
    {
        bool isDoubleSpeed = UI_Manager.Instance.DoubleSpeedUp();
        _speed1.SetActive(!isDoubleSpeed);
        _speed2.SetActive(isDoubleSpeed);
    }
}
