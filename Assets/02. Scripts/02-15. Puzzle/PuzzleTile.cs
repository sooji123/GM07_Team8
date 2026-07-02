using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UnityEngine.UIElements;
using DG.Tweening;

public class PuzzleTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("타일 정보")]
    public int x;
    public int y;
    public EElement myElement;

    public BoardCreator boardCreator;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private float swipeAngle;

    private Coroutine moveCoroutine;

    private SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InitTile(int xPos, int yPos, EElement element, BoardCreator creator)
    {
        x = xPos;
        y = yPos;
        myElement = element;
        boardCreator = creator;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ( boardCreator.isMatching )
        {
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        firstTouchPosition = new Vector2(worldPos.x, worldPos.y);

        transform.DOComplete();
        spriteRenderer.DOComplete();

        transform.DOScale(2.3f, 0.15f).SetEase(Ease.OutQuad);
        spriteRenderer.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.1f);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_OnClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        finalTouchPosition = new Vector2(worldPos.x, worldPos.y);
        CalculateAngle();

        transform.DOComplete();
        spriteRenderer.DOComplete();

        transform.DOScale(3.6f, 0.15f).SetEase(Ease.OutBack);
        spriteRenderer.DOColor(Color.white, 0.2f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (boardCreator.isMatching)
        {
            return;
        }

        CursorVFX.Instance.SnapToTile(transform.position);

        transform.DOComplete();
        transform.DOScale(3.6f, 0.15f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_OnMouse);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CursorVFX.Instance.HideCursor();

        transform.DOComplete();
        spriteRenderer.DOComplete();

        transform.DOScale(3f, 0.15f).SetEase(Ease.OutQuad);
        spriteRenderer.DOColor(Color.white, 0.1f);
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) < 0.5f &&
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) < 0.5f)
        {
            return;
        }

        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
        MovePieces();
    }

    private void MovePieces()
    {
        if (boardCreator.isMatching)
        {
            return;
        }

        if (swipeAngle > -45 && swipeAngle <= 45) boardCreator.SwapTiles(this, 1, 0);
        else if (swipeAngle > 45 && swipeAngle <= 135) boardCreator.SwapTiles(this, 0, 1);
        else if (swipeAngle > 135 || swipeAngle <= -135) boardCreator.SwapTiles(this, -1, 0);
        else if (swipeAngle < -45 && swipeAngle >= -135) boardCreator.SwapTiles(this, 0, -1);
    }

    public void MoveToPosition(Vector2 targetPos, Action onComplete = null)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(SwapCoroutine(targetPos, onComplete));
    }

    private IEnumerator SwapCoroutine(Vector2 targetPos, Action onComplete)
    {
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            if (this == null) yield break;

            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (this != null)
        {
            transform.position = targetPos;

            onComplete?.Invoke();
        }
    }
}