using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : Singleton<CursorManager>
{
    [Header("커서 스프라이트")]
    [Header("평상시 커서")]
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorHand;
    [SerializeField] private Texture2D cursorNo;

    [Header("클릭시 커서")]
    [SerializeField] private Texture2D cursorDefaultClick;
    [SerializeField] private Texture2D cursorHandClick;


    private Vector2 hotspot = Vector2.zero; // 마우스 눌리는 위치 좌표 0,0
    private ECursor currentCursorType = ECursor.Default; // 현재 커서 저장 변수

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ApplyClickCursor();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ApplyNormalCursor();
        }
    }

    public void SetDefaultCursor()
    {
        currentCursorType = ECursor.Default;
        // 마우스를 누른 채로 밖으로 나갈 수도 있으니 상태를 체크
        if (Mouse.current.leftButton.isPressed)
        {
            ApplyClickCursor();
        }
        else
        {
            ApplyNormalCursor();
        }
    }

    public void SetHandCursor()
    {
        currentCursorType = ECursor.Hand;
        if (Mouse.current.leftButton.isPressed)
        {
            ApplyClickCursor();
        }
        else
        {
            ApplyNormalCursor();
        }
    }

    public void SetNoCursor()
    {
        currentCursorType = ECursor.No;
        Cursor.SetCursor(cursorNo, hotspot, CursorMode.Auto);
    }

    private void ApplyNormalCursor()
    {
        switch (currentCursorType)
        {
            case ECursor.Default:
                Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
                break;

            case ECursor.Hand:
                Cursor.SetCursor(cursorHand, hotspot, CursorMode.Auto);
                break;
        }
    }

    private void ApplyClickCursor()
    {
        switch (currentCursorType)
        {
            case ECursor.Default:
                Cursor.SetCursor(cursorDefaultClick, hotspot, CursorMode.Auto);
                break;
            case ECursor.Hand:
                Cursor.SetCursor(cursorHandClick, hotspot, CursorMode.Auto);
                break;
        }
    }
}