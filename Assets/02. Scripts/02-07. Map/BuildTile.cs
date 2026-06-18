using UnityEngine;

public class BuildTile : MonoBehaviour
{
    [Header ("UI 버튼 연결")]
    [SerializeField]
    private TowerBuildUI _buildUI;
    private SpriteRenderer _spriteRenderer;
    //private Camera mainCam; //마우스 좌표값 받기(설치용)

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); //색상변경용
    }
    /*
    private void Start()
    {
        mainCam = Camera.main;
    }
    */
    private void Update()
    {
        if (_buildUI._isDrag == true)
        {
            _spriteRenderer.enabled = true;
        }
        else
        {
            _spriteRenderer.enabled = false;
        }
    }
}