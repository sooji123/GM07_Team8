using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    private BoardCreator boardCreator;

    private void OnEnable()
    {
        BoardCreator board = GetComponent<BoardCreator>();
    }

    public void FirstWave()
    {
        boardCreator.GenerateBoard();
        Debug.Log("보드판 출력 완료");

        SetBoardActive(true);
        Debug.Log("보드 활성화");
    }

    public void StartWave()
    {
        SetBoardActive(true);
        Debug.Log("보드 활성화");
    }

    public void EndWave()
    {
        SetBoardActive(false);
        Debug.Log("보드 비활성화");
    }
    public void SetBoardActive(bool active)
    {
        foreach (var tile in boardCreator.Tiles)
        {
            tile.isActive = active;
        }
    }
}
