using UnityEngine;
using UnityEngine.UI;
//설치하는 타워 종류에 따라 활성화 되는 타일을 만들기 위해 임시로 제작한 UI 버튼
public class TowerBuildUI : MonoBehaviour
{
    private Button _towerButton;
    public bool _isClick = false;

    private void Awake()
    {
        //towerButton UI 오브젝트의 Button 컴포넌트 가져오기
        _towerButton = GetComponent<Button>();
    }
    private void Start()
    {
        _towerButton.onClick.AddListener(OnClick);
    }

    //클릭 시 같은 Enum의 BuildTile이 작동
    private void OnClick()
    {
        _isClick = !_isClick;
    }
}
