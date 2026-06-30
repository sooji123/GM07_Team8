using UnityEngine;

public class FloatingTextPosition : MonoBehaviour
{
    [SerializeField] private RectTransform ParentCanvas;
    [SerializeField] private GameObject floatingTextPrefab;

    public void SpawnFloatingTextOnCanvas(string text, Color color, Vector2 localPosition)
    {
        GameObject obj = Instantiate(floatingTextPrefab, ParentCanvas);

        RectTransform objRect = obj.GetComponent<RectTransform>();
        objRect.anchoredPosition = localPosition;

        obj.GetComponent<FloatingTextUI>().TextSetting(text, color);
    }

    public void SpawnFloatingTextOnMouse(string text, Color color, Vector2 screenPosition)
    {
        GameObject obj = Instantiate(floatingTextPrefab, ParentCanvas);

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            ParentCanvas,
            screenPosition,
            Camera.main,
            out worldPoint
        );

        obj.transform.position = worldPoint;
        Vector3 localPos = obj.transform.localPosition;
        localPos.z = 0f;
        obj.transform.localPosition = localPos;

        obj.GetComponent<FloatingTextUI>().TextSetting(text, color);
    }
}
