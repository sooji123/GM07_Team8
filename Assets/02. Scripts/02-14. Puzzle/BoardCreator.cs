using UnityEngine;
public class BoardCreator : MonoBehaviour
{
    [Header("기본 세팅")]
    [SerializeField] private BoardFiller board;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileGap = 1.0f; // 타일 간격 크기

    [Header("속성 프리팹 세팅")]
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite earthSprite;

    private void Start()
    {
        if (board == null)
        {
            board = GetComponent<BoardFiller>();
        }

        Debug.Log("보드 그리기 시작");
        GenerateBoard();
        Debug.Log("보드 그리기 완료!");
    }

    private void GenerateBoard()
    {
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                // 좌표에 속성을 받아옴
                EElement element = board.GetElement(x, y);
                if (element == EElement.None)
                {
                    continue;
                }

                Vector2 spawnPosition = new Vector2(
                    x * tileGap + tilePrefab.transform.position.x,
                    y * tileGap + tilePrefab.transform.position.y);

                GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                newTile.GetComponent<PuzzleTile>().InitTile(x, y, element);

                SpriteRenderer spriteRenderer = newTile.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = GetSprite(element);
            }
        }
    }

    private Sprite GetSprite(EElement element)
    {
        return element switch
        {
            EElement.Water => waterSprite,
            EElement.Fire => fireSprite,
            EElement.Grass => grassSprite,
            EElement.Earth => earthSprite,
        };
    }
}