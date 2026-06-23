using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("타워 설치 가능 여부")]
    [SerializeField]
    private ETile _Etile = ETile.None;

    [SerializeField]
    private TowerBuilder towerBuilder;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //색상변경용
    }

    private void Update() //실무에서 이벤트형 변경 추천(오늘: 이벤트 추가 학습 및 공부하기)
    {
        spriteRenderer.enabled = CheckBuildTower();
    }
    //타워 설치 가능 여부를 구분하기 위한 비트플래그 enum 체크 & 설치 가능 여부 반환
    private bool CheckBuildTower()
    {
        if (towerBuilder.CurrentTurret != null)
        {
            return false;
        }

        bool result = TowerBuildUI.isDrag && (_Etile & TowerBuildUI.EcurrentState) != 0;

        if (_Etile.HasFlag(ETile.TowerTile) && TowerBuildUI.EcurrentState.HasFlag(ETile.TowerTile))
        {
            result = result && true; //타워 설치 가능
        }
        else if (_Etile.HasFlag(ETile.TrapTile) && TowerBuildUI.EcurrentState.HasFlag(ETile.TrapTile))
        {
            result = result && true; //트랩 설치 가능
        }
        return result;
    }
}
