using System.Collections.Generic;
using UnityEngine;

public class BoardFiller : MonoBehaviour
{
    [SerializeField] protected int width = 7;
    [SerializeField] protected int height = 10;

    public int Width => width;
    public int Height => height;

    private EElement[,] board;

    private readonly EElement[] defaultElement = new EElement[]
    {
        EElement.Water, EElement.Fire, EElement.Grass, EElement.Electric
    };

    private void Awake()
    {
        Debug.Log("보드 생성 시작");
        board = new EElement[width, height];
        FillBoard();
        Debug.Log("보드 생성 완료");
    }

    private void FillBoard()
    {
        // 보드 채울 때 3매치 자동으로 이어지는 걸 피하기 위해 리스트 생성
        List<EElement> selectedElement = new List<EElement>();
        Debug.Log("리스트 생성..");

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                selectedElement.Clear();
                selectedElement.AddRange(defaultElement);

                if (x >= 2 && board[x - 1, y] == board[x - 2, y])
                {
                    selectedElement.Remove(board[x - 1, y]);
                } // 채울 칸 X -2, -1의 위치에 있는 속성이 같다면 지움 (가로 매치 방지)

                if (y >= 2 && board[x, y - 1] == board[x, y - 2])
                {
                    selectedElement.Remove(board[x, y - 1]);
                } // 채울 칸 Y -2, -1의 위치에 있는 속성이 같다면 지움 (세로 매치 방지)

                // 남은 속성들 중에서 랜덤 굴리기
                int randomIndex = Random.Range(0, selectedElement.Count);
                board[x, y] = selectedElement[randomIndex];
            }
        }
    }

    public EElement GetElement(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return board[x, y];
        }

        return EElement.None;
    }
}
