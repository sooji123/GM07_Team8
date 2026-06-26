using UnityEngine;

public class UISetActive : MonoBehaviour
{
    public GameObject targetUI;

    public void Toggle()
    {
        targetUI.SetActive(!targetUI.activeSelf);
    }
}
