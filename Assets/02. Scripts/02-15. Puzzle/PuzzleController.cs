using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [SerializeField]
    private BoardCreator boardCreator;

    private void OnEnable()
    {
        boardCreator = GetComponent<BoardCreator>();
    }

    public void FirstWave()
    {
        boardCreator.GenerateBoard();
        Debug.Log("첫 보드판 출력 완료");
    }

    public void StartWave()
    {
        boardCreator.GenerateBoard();
        Debug.Log("보드판 출력 완료");
    }

    public void EndWave()
    {
        DestroyBoard();
        Debug.Log("보드 파괴");
    }

    public void DestroyBoard()
    {
        if (boardCreator.Tiles == null) return;

        boardCreator.StopAllCoroutines();
        boardCreator.isMatching = false;

        for (int x = 0; x < boardCreator.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < boardCreator.Tiles.GetLength(1); y++)
            {
                if (boardCreator.Tiles[x, y] != null)
                {
                    Destroy(boardCreator.Tiles[x, y].gameObject);
                    boardCreator.Tiles[x, y] = null;
                }
            }
        }
    }
}
