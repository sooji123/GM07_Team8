using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("타일 정보")]
    public int x;
    public int y;
    public EElement myElement;

    // 마우스 좌표
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private float swipeAngle;

    public void InitTile(int xPos, int yPos, EElement element)
    {
        x = xPos;
        y = yPos;
        myElement = element;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        firstTouchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        finalTouchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) < 0.4f &&
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) < 0.4f)
        {
            return;
        } // 드래그 길이가 너무 짧으면 return

        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
            finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

        MovePieces();
    }

    private void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            Debug.Log($"[{x}, {y}] 타일을 오른쪽으로 스와이프");
            // (x+1, y) 타일과 내 위치 바꾸기 함수 호출
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            Debug.Log($"[{x}, {y}] 타일을 위쪽으로 스와이프");
            // (x, y+1) 타일과 내 위치 바꾸기 함수 호출
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            Debug.Log($"[{x}, {y}] 타일을 왼쪽으로 스와이프");
            // (x-1, y) 타일과 내 위치 바꾸기 함수 호출
        }
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {
            Debug.Log($"[{x}, {y}] 타일을 아래쪽으로 스와이프");
            // (x, y-1) 타일과 내 위치 바꾸기 함수 호출
        }
    }
}