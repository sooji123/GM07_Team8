using UnityEngine;
using UnityEngine.EventSystems;

//설치할 타워(spawnPrefab)를 생성하며, 설치할 Tile의 Layer를 구분하여 설치 여부를 확인
public class TowerBuildUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("데이터는 하나만 넣어주세요")]
    [Header("생성할 Tower의 데이터")]
    [SerializeField]
    private TurretData turretData;
    [Header("생성할 Trap의 데이터")]
    [SerializeField]
    private TrapData trapData;

    [Header("Tower or Trap 의 Ghost")]
    [SerializeField]
    private GameObject ghostPrefab;

    [Header("설치할 타일 레이어")]
    [SerializeField]
    private LayerMask targetLayer;

    private GameObject currentGhostObject;
    private Camera mainCam;

    private float rayDistance = 10f;
    public static bool isDrag = false;
    public static ETile EcurrentState { get; private set; } = ETile.None;

    private void Start()
    {
        mainCam = Camera.main;
    }

    //드래그 시작 시 프리펩 생성
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (turretData != null && trapData != null)
        {
            Debug.Log("데이터는 하나만 넣어주세요!");
            return;
        }

        if ((turretData != null || trapData != null) && ghostPrefab != null)
        {
            //프리펩 생성
            currentGhostObject = Instantiate(ghostPrefab);
            UpdatePosition(eventData.position);

            EcurrentState = GetBuildableTileType();
            isDrag = true;
        }
    }

    //드래그 진행 중일 시 마우스 위치를 따라 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (currentGhostObject != null)
        {
            UpdatePosition(eventData.position);
        }
    }

    //드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentGhostObject == null)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(eventData.position);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, rayDistance, targetLayer);

        if (hit.collider != null)
        {
            RequestBuild(hit.transform);
        }
        Destroy(currentGhostObject);

        currentGhostObject = null;

        EcurrentState = ETile.None;
        isDrag = false;
    }

    private void UpdatePosition(Vector2 screenPosition)
    {
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(screenPosition);
        currentGhostObject.transform.position = mouseWorldPos;
    }

    private void RequestBuild(Transform tileTransform)
    {
        TowerBuilder builder = tileTransform.GetComponent<TowerBuilder>();

        if (builder != null)
        {
            if (turretData != null)
            {
                builder.BuildTower(turretData);
            }
            else if (trapData != null)
            {
                builder.BuildTower(trapData);
            }
            else
            {
                Debug.Log("UI에 Data를 넣어주세요");
            }
        }
        else
        {
            Debug.Log("타일에 TowerBuilder 넣으세용");
        }
    }

    private ETile GetBuildableTileType()
    {
        if (gameObject.CompareTag("Tower"))
        {
            return ETile.TowerTile;
        }
        if (gameObject.CompareTag("Trap"))
        {
            return ETile.TrapTile;
        }
        else
        {
            return ETile.None;
        }
    }
}
