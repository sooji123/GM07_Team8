using UnityEngine;
using DG.Tweening;

public class CursorVFX : Singleton<CursorVFX>
{
    [Header("스냅 커서 프리팹")]
    public GameObject snapPrefab;

    private GameObject currentSnap;

    protected override void Awake()
    {
        base.Awake();

        if (snapPrefab != null)
        {
            currentSnap = Instantiate(snapPrefab);
            currentSnap.SetActive(false);
        }
    }
    
    public void SnapToTile(Vector3 targetPosition)
    {
        if (currentSnap == null) return;

        currentSnap.SetActive(true);

        currentSnap.transform.DOComplete();

        currentSnap.transform.DOMove(targetPosition, 0.1f).SetEase(Ease.OutQuad);

        currentSnap.transform.localScale = Vector3.one * 1.2f;
        currentSnap.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce);
    }

    public void HideCursor()
    {
        if (currentSnap != null)
        {
            currentSnap.SetActive(false);
        }
    }
}