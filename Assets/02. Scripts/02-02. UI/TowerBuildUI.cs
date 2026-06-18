using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//설치할 타워(spawnPrefab)를 생성하며, 설치할 Tile의 Layer를 구분하여 설치 여부를 확인
public class TowerBuildUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("생성할 Tower || Trap")]
    [SerializeField]
    private GameObject _spawnPrefab;

    [Header("설치할 타겟 레이어")]
    [SerializeField]
    private LayerMask _targetLayer;

    private GameObject _currentSpawnedObject;
    private Camera mainCam;
    private float _rayDistance = 10f;
    public bool _isDrag = false;

    private void Start()
    {
        mainCam = Camera.main;
    }

    //드래그 시작 시 프리펩 생성
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_spawnPrefab != null)
        {
            //프리펩 생성
            _currentSpawnedObject = Instantiate(_spawnPrefab);
            UpdatePosition(eventData.position);

            _isDrag = true;
        }
    }
    //드래그 진행 중일 시 마우스 위치를 따라 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (_currentSpawnedObject != null)
        {
            UpdatePosition(eventData.position);
        }
    }
    //드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSpawnedObject == null)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(eventData.position);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, _rayDistance, _targetLayer);


        if (hit.collider != null)
        {
            _currentSpawnedObject.transform.position = hit.collider.transform.position;
        }
        else
        {
            Destroy(_currentSpawnedObject);
        }
        _currentSpawnedObject = null;

        _isDrag = false;
    }

    private void UpdatePosition(Vector2 screenPosition)
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(screenPosition);
        _currentSpawnedObject.transform.position = mouseWorldPos;
    }
}
