using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [Header("기본 세팅")]
    [SerializeField] private BoardFiller board;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float tileGap = 1.0f;

    [Header("속성 프리팹 세팅")]
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite electricSprite;

    public PuzzleTile[,] Tiles;
    public bool isMatching = false;

    private int movingTilesCount = 0;

    private void Start()
    {
        if (board == null)
        {
            board = GetComponent<BoardFiller>();
        }
        
        Tiles = new PuzzleTile[board.Width, board.Height];
    }

    public void GenerateBoard()
    {
        isMatching = false;
        movingTilesCount = 0;
        StopAllCoroutines(); // 상태 초기화

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                EElement element = board.GetElement(x, y);
                if (element == EElement.None) continue;

                Vector2 targetPosition = new Vector2(
                    x * tileGap + tilePrefab.transform.position.x,
                    y * tileGap + tilePrefab.transform.position.y);

                Vector2 spawnPosition = new Vector2(targetPosition.x, targetPosition.y + (board.Height * tileGap));

                GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                newTile.name = $"Tile_({x}, {y})";

                PuzzleTile puzzleTile = newTile.GetComponent<PuzzleTile>();
                puzzleTile.InitTile(x, y, element, this);
                Tiles[x, y] = puzzleTile;

                SpriteRenderer spriteRenderer = newTile.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = GetSprite(element);

                movingTilesCount++;
                puzzleTile.MoveToPosition(targetPosition, OnTileMoveComplete);
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
            _ => null
        };
    }

    public void OnTileMoveComplete()
    {
        movingTilesCount--;
    }

    public void SwapTiles(PuzzleTile tile1, int dirX, int dirY)
    {
        if (isMatching) return;
        if (tile1 == null || tile1.gameObject == null) return;

        int targetX = tile1.x + dirX;
        int targetY = tile1.y + dirY;

        if (targetX < 0 || targetX >= board.Width || targetY < 0 || targetY >= board.Height) return;

        PuzzleTile tile2 = Tiles[targetX, targetY];
        if (tile2 == null || tile2.gameObject == null) return;

        StartCoroutine(CoSwap(tile1, tile2));
    }

    private IEnumerator CoSwap(PuzzleTile tile1, PuzzleTile tile2)
    {
        isMatching = true;

        SwapData(tile1, tile2);

        Vector2 pos1 = tile1.transform.position;
        Vector2 pos2 = tile2.transform.position;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_Swap);
        }

        movingTilesCount += 2;
        tile1.MoveToPosition(pos2, OnTileMoveComplete);
        tile2.MoveToPosition(pos1, OnTileMoveComplete);

        yield return new WaitUntil(() => movingTilesCount == 0);

        HashSet<PuzzleTile> matchedTiles = FindMatch();

        if (matchedTiles.Count >= 3)
        {
            Debug.Log($"매치 성공 -> {matchedTiles.Count}개의 블록 파괴");

            DestroyMatch(matchedTiles);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ProcessGravityAndMatches());
        }
        else
        {
            Debug.Log("매치 실패 -> 리턴");
            SwapData(tile1, tile2);

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_MatchFail);
            }

            if (tile1 != null) { movingTilesCount++; tile1.MoveToPosition(pos1, OnTileMoveComplete); }
            if (tile2 != null) { movingTilesCount++; tile2.MoveToPosition(pos2, OnTileMoveComplete); }

            yield return new WaitUntil(() => movingTilesCount == 0);
            isMatching = false;
        }
    }

    private IEnumerator ProcessGravityAndMatches()
    {
        bool hasMatches = true;

        while (hasMatches)
        {
            DropTiles();
            FillEmpty();

            yield return new WaitUntil(() => movingTilesCount == 0);

            HashSet<PuzzleTile> newMatches = FindMatch();

            if (newMatches.Count >= 3)
            {
                Debug.Log($"콤보! {newMatches.Count}개의 블록 추가 파괴!");
                DestroyMatch(newMatches);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                hasMatches = false;
            }
        }

        yield return new WaitForSeconds(0.1f);
        isMatching = false;
    }

    private void DropTiles()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                if (Tiles[x, y] == null)
                {
                    for (int k = y + 1; k < board.Height; k++)
                    {
                        if (Tiles[x, k] != null)
                        {   
                            PuzzleTile tileToDrop = Tiles[x, k];

                            Tiles[x, y] = tileToDrop;
                            Tiles[x, k] = null;
                            tileToDrop.y = y;

                            Vector2 targetPos = new Vector2(
                                x * tileGap + tilePrefab.transform.position.x,
                                y * tileGap + tilePrefab.transform.position.y);

                            movingTilesCount++;
                            tileToDrop.MoveToPosition(targetPos, OnTileMoveComplete);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void FillEmpty()
    {
        for (int x = 0; x < board.Width; x++)
        {
            int missingCount = 0;

            for (int y = 0; y < board.Height; y++)
            {
                if (Tiles[x, y] == null)
                {
                    missingCount++;
                    EElement randomElement = (EElement)Random.Range(1, 5);

                    Vector2 spawnPosition = new Vector2(
                        x * tileGap + tilePrefab.transform.position.x,
                        (board.Height + missingCount) * tileGap + tilePrefab.transform.position.y);

                    GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                    newTile.name = $"Tile_({x}, {y})";

                    PuzzleTile puzzleTile = newTile.GetComponent<PuzzleTile>();
                    puzzleTile.InitTile(x, y, randomElement, this);
                    Tiles[x, y] = puzzleTile;

                    SpriteRenderer spriteRenderer = newTile.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = GetSprite(randomElement);

                    Vector2 targetPos = new Vector2(
                        x * tileGap + tilePrefab.transform.position.x,
                        y * tileGap + tilePrefab.transform.position.y);

                    movingTilesCount++;
                    puzzleTile.MoveToPosition(targetPos, OnTileMoveComplete);
                }
            }
        }
    }

    private void SwapData(PuzzleTile tile1, PuzzleTile tile2)
    {
        Tiles[tile1.x, tile1.y] = tile2;
        Tiles[tile2.x, tile2.y] = tile1;

        int tempX = tile1.x; int tempY = tile1.y;
        tile1.x = tile2.x; tile1.y = tile2.y;
        tile2.x = tempX; tile2.y = tempY;
    }

    private HashSet<PuzzleTile> FindMatch()
    {
        HashSet<PuzzleTile> match = new HashSet<PuzzleTile>();

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width - 2; x++)
            {
                PuzzleTile current = Tiles[x, y];
                if (current == null) continue;

                PuzzleTile right1 = Tiles[x + 1, y];
                PuzzleTile right2 = Tiles[x + 2, y];

                if (right1 != null && right2 != null &&
                    current.myElement == right1.myElement &&
                    current.myElement == right2.myElement)
                {
                    match.Add(current);
                    match.Add(right1);
                    match.Add(right2);
                }
            }
        }

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height - 2; y++)
            {
                PuzzleTile current = Tiles[x, y];
                if (current == null) continue;

                PuzzleTile up1 = Tiles[x, y + 1];
                PuzzleTile up2 = Tiles[x, y + 2];

                if (up1 != null && up2 != null &&
                    current.myElement == up1.myElement &&
                    current.myElement == up2.myElement)
                {
                    match.Add(current);
                    match.Add(up1);
                    match.Add(up2);
                }
            }
        }
        FindMatchCategory(match);
        
        return match;
    }

    private void FindMatchCategory(HashSet<PuzzleTile> match)
    {
        HashSet<PuzzleTile> visited = new HashSet<PuzzleTile>();

        foreach (PuzzleTile startTile in match)
        {
            if (visited.Contains(startTile)) continue;

            Queue<PuzzleTile> queue = new Queue<PuzzleTile>();
            queue.Enqueue(startTile);
            visited.Add(startTile);

            int minX = startTile.x, maxX = startTile.x;
            int minY = startTile.y, maxY = startTile.y;

            while (queue.Count > 0)
            {
                PuzzleTile current = queue.Dequeue();

                if (current.x < minX) minX = current.x;
                if (current.x > maxX) maxX = current.x;
                if (current.y < minY) minY = current.y;
                if (current.y > maxY) maxY = current.y;

                Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach (Vector2Int dir in dirs)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;

                    if (nx >= 0 && nx < board.Width && ny >= 0 && ny < board.Height)
                    {
                        PuzzleTile neighbor = Tiles[nx, ny];

                        if (neighbor != null && match.Contains(neighbor) &&
                            !visited.Contains(neighbor) && neighbor.myElement == startTile.myElement)
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            if (width >= 5 || height >= 5)
            {
                Debug.Log($"⭐⭐⭐ [5매치] {startTile.myElement} 속성 -> 15 게이지 지급");

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_MatchSP);
                }

                EnergyManager.Instance.AddEnergy(EnergyManager.Instance.match5Energy);
            }
            else if (width >= 3 && height >= 3)
            {
                Debug.Log($"⭐⭐ [L/T매치] {startTile.myElement} 속성 -> 10 게이지 지급");

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_MatchSP);
                }

                EnergyManager.Instance.AddEnergy(EnergyManager.Instance.matchTLEnergy);
            }
            else if (width >= 4 || height >= 4)
            {
                Debug.Log($"⭐ [4매치] {startTile.myElement} 속성 -> 4 게이지 지급");

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_MatchSP);
                }

                EnergyManager.Instance.AddEnergy(EnergyManager.Instance.match4Energy);
            }
            else if (width >= 3 || height >= 3)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayeSFX(ESFXType.Puzzle_Match);
                }

                EnergyManager.Instance.AddEnergy(EnergyManager.Instance.match3Energy);
            }
        }
    }

    private void DestroyMatch(HashSet<PuzzleTile> matches)
    {
        Dictionary<EElement, int> orbCount = new Dictionary<EElement, int>();

        foreach (PuzzleTile tile in matches)
        {
            if(orbCount.ContainsKey(tile.myElement))
            {
                orbCount[tile.myElement]++;
            }
            else
            {
                orbCount[tile.myElement] = 1;
            }
            Tiles[tile.x, tile.y] = null;
            tile.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(tile.gameObject);
        }

        foreach (var v in orbCount)
        {
            EElement elementType = v.Key;
            int count = v.Value;
            CurrencyManager.Instance.AddElementOrbs(elementType, count);
        }
    }
}