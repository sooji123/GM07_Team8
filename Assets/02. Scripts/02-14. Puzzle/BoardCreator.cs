using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public bool isSwaping = false;

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
        if (isSwaping == true)
        {
            return;
        } // SWAP 중이면 리턴

        int targetX = tile1.x + dirX;
        int targetY = tile1.y + dirY;

        if (targetX < 0 || targetX >= board.Width || targetY < 0 || targetY >= board.Height)
        {
            return;
        } // 보드판 밖으로 SWAP 막기

        PuzzleTile tile2 = Tiles[targetX, targetY];
        if (tile2 == null)
        {
            return;
        } // 바꿀 타일 가져오기

        StartCoroutine(CoSwap(tile1, tile2));
    }

    private IEnumerator CoSwap(PuzzleTile tile1, PuzzleTile tile2)
    {
        isSwaping = true; // 스왑 잠금

        // 정보값 교환
        SwapData(tile1, tile2);

        // 타일 이동
        Vector2 pos1 = tile1.transform.position;
        Vector2 pos2 = tile2.transform.position;
        tile1.StartCoroutine(tile1.SwapCoroutine(pos2));
        tile2.StartCoroutine(tile2.SwapCoroutine(pos1));

        // 0.2초 대기
        yield return new WaitForSeconds(0.2f);

        // 이동 후 매치 검사
        HashSet<PuzzleTile> matchTiles = FindMatches();

        
        if (matchTiles.Count >= 3) // 매치 가능
        {
            Debug.Log($"매치 {matchTiles.Count} 타일");
            DestroyMatch(matchTiles);

            // 빈칸에 타일 채우기(중력 적용하는 것 처럼) - 추가해야댐

            isSwaping = false;
        }
        else // 매치 불가능
        {
            Debug.Log("매치 불가능 -> 제자리로..");
            SwapData(tile1, tile2);
            tile1.StartCoroutine(tile1.SwapCoroutine(pos1));
            tile2.StartCoroutine(tile2.SwapCoroutine(pos2));

            yield return new WaitForSeconds(0.2f); // 움직이는 시간 기다리는거
            isSwaping = false;
        }
    }

    private void SwapData(PuzzleTile tile1, PuzzleTile tile2)
    {
        Tiles[tile1.x, tile1.y] = tile2;
        Tiles[tile2.x, tile2.y] = tile1;

        int tempX = tile1.x; 
        int tempY = tile1.y;
        tile1.x = tile2.x;
        tile1.y = tile2.y;
        tile2.x = tempX;
        tile2.y = tempY;
    }

    // 매치 검사(맵 전체 검사)
    private HashSet<PuzzleTile> FindMatches()
    {
        HashSet<PuzzleTile> match = new HashSet<PuzzleTile>();

        // 가로 매치 검사
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width - 2; x++)
            {
                PuzzleTile current = Tiles[x, y];
                if (current == null) continue;

                PuzzleTile right1 = Tiles[x + 1, y];
                PuzzleTile right2 = Tiles[x + 2, y];

                if (right1 != null && right2 != null 
                    && current.myElement == right1.myElement && current.myElement == right2.myElement)
                {
                    match.Add(current);
                    match.Add(right1);
                    match.Add(right2);
                }
            }
        }

        // 세로 매치 검사
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height - 2; y++)
            {
                PuzzleTile current = Tiles[x, y];
                if (current == null) continue;

                PuzzleTile up1 = Tiles[x, y + 1];
                PuzzleTile up2 = Tiles[x, y + 2];

                if (up1 != null && up2 != null &&
                    current.myElement == up1.myElement && current.myElement == up2.myElement)
                {
                    match.Add(current);
                    match.Add(up1);
                    match.Add(up2);
                }
            }
        }
        return match;
    }

    // 폭 발
    private void DestroyMatch(HashSet<PuzzleTile> matches)
    {
        foreach (PuzzleTile tile in matches)
        {
            Tiles[tile.x, tile.y] = null;

            Destroy(tile.gameObject);
        }
    }
}