using UnityEngine;

public class BuildTile : MonoBehaviour
{
    //장기적인 목표를 위해 코드 수정의 필요성 있음
    [Header ("UI 버튼 연결")]
    [SerializeField]
    private TowerBuildUI _buildUI;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); //색상변경용
    }

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