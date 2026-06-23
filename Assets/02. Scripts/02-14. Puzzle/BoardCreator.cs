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
    [SerializeField] private Sprite electricSprite;

    public PuzzleTile[,] Tiles;

    private void Start()
    {
        if (board == null)
        {
            board = GetComponent<BoardFiller>();
        }

        Tiles = new PuzzleTile[board.Width, board.Height];

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
                newTile.name = $"Tile_[{x},{y}]";

                PuzzleTile puzzleTile = newTile.GetComponent<PuzzleTile>();
                puzzleTile.InitTile(x, y, element, this);

                Tiles[x, y] = puzzleTile;

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
            EElement.Electric => electricSprite,
        };
    }

    public void SwapTiles(PuzzleTile tile1, int dirX, int dirY)
    {
        int targetX = tile1.x + dirX;
        int targetY = tile1.y + dirY;

        if (targetX < 0 || targetX >= board.Width || targetY < 0 || targetY >= board.Height)
        {
            return;
        }

        PuzzleTile tile2 = Tiles[targetX, targetY];
        if (tile2 == null)
        {
            return;
        }

        // 타일 정보 교환
        Tiles[tile1.x, tile1.y] = tile2;
        Tiles[targetX, targetY] = tile1;

        // 좌표 교환
        int tempX = tile1.x;
        int tempY = tile1.y;
        tile1.x = tile2.x;
        tile1.y = tile2.y;
        tile2.x = tempX;
        tile2.y = tempY;

        Vector2 tempPos = tile1.transform.position;
        tile1.StartCoroutine(tile1.SwapCoroutine(tile2.transform.position));
        tile2.StartCoroutine(tile2.SwapCoroutine(tempPos));

        // 매치 가능 검사 추가
    }
}