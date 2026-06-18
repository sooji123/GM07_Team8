using UnityEngine;
using UnityEngine.EventSystems;

//설치할 타워(spawnPrefab)를 생성하며, 설치할 Tile의 Layer를 구분하여 설치 여부를 확인
public class TowerBuildUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("생성할 Tower or Trap")]
    [SerializeField]
    private GameObject _spawnPrefab;

    [Header("Tower or Trap 의 Ghost")]
    [SerializeField]
    private GameObject _ghostPrefab;

    [Header("설치할 타겟 레이어")]
    [SerializeField]
    private LayerMask _targetLayer;

    private GameObject _currentGhostObject;
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
        if (_spawnPrefab != null && _ghostPrefab != null)
        {
            //프리펩 생성
            _currentGhostObject = Instantiate(_ghostPrefab);
            UpdatePosition(eventData.position);

            _isDrag = true;
        }
    }
    //드래그 진행 중일 시 마우스 위치를 따라 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (_currentGhostObject != null)
        {
            UpdatePosition(eventData.position);
        }
    }
    //드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentGhostObject == null)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(eventData.position);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, _rayDistance, _targetLayer);

        if (hit.collider != null)
        {
            SpawnTower(hit.transform);
        }
        Destroy(_currentGhostObject);

        _currentGhostObject = null;

        _isDrag = false;
    }

    private void UpdatePosition(Vector2 screenPosition)
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(screenPosition);
        _currentGhostObject.transform.position = mouseWorldPos;
    }

    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();
        //타워 건설 기능 여부 확인
        //1. 현재 타일 위치에 이미 타워가 건설되어 있으면 건설 X
        if(tile._isBuildTower == true)
        {
            return;
        }
        //타워가 건설되어 있음으로 설정
        tile._isBuildTower = true;
        //선택한 타일의 위치에 타워 건설
        Instantiate(_spawnPrefab, tileTransform.position, Quaternion.identity);
    }
}
