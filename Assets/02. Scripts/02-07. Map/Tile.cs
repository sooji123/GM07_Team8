using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool _isBuildTower { get; set; }
    private SpriteRenderer _spriteRenderer;
    private TowerBuildUI _towerBuildUI;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); //색상변경용
        _isBuildTower = false;
    }

    private void Start()
    {
        _towerBuildUI = FindAnyObjectByType<TowerBuildUI>();
    }

    private void Update()
    {
        _spriteRenderer.enabled = !_isBuildTower && _towerBuildUI != null && _towerBuildUI._isDrag;
    }
}
