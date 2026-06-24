using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PuzzleTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("타일 정보")]
    public int x;
    public int y;
    public EElement myElement;

    public BoardCreator boardCreator;

    // 마우스 좌표
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private float swipeAngle;

    public void InitTile(int xPos, int yPos, EElement element, BoardCreator creator)
    {
        x = xPos;
        y = yPos;
        myElement = element;
        boardCreator = creator;
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

        MovePuzzle();
    }

    private void MovePuzzle()
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            boardCreator.SwapTiles(this, 1, 0);
            Debug.Log($"[{x}, {y}] 타일을 오른쪽으로 스와이프");
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            boardCreator.SwapTiles(this, 0, 1);
            Debug.Log($"[{x}, {y}] 타일을 위쪽으로 스와이프");
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            boardCreator.SwapTiles(this, -1, 0);
            Debug.Log($"[{x}, {y}] 타일을 왼쪽으로 스와이프");
        }
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {
            boardCreator.SwapTiles(this, 0, -1);
            Debug.Log($"[{x}, {y}] 타일을 아래쪽으로 스와이프");
        }
    }

    public IEnumerator SwapCoroutine(Vector2 targetPos)
    {
        Vector2 startPos = transform.position;
        float swapTime = 0.0f;
        float duration = 0.2f;

        while (swapTime < duration)
        {
            if (this == null)
            {
                yield break;
            } // 매치되어 부서졌다면 브레이크

            transform.position = Vector2.Lerp(startPos, targetPos, swapTime / duration);
            swapTime += Time.deltaTime;
            yield return null;
        }

        if (this != null)
        { 
            transform.position = targetPos; 
        } // 부서지지 않았을 때만 pos 고정
    }
}