using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemySlot : MonoBehaviour
{
    [Header("정보 연결")]
    [SerializeField] private Image enemyImage; //이미지
    [SerializeField] private TMP_Text nameText; //이름
    [SerializeField] private Image barrierIcon; //베리어 여부
    [SerializeField] private Image regenIcon; //재생 여부
    [SerializeField] private TMP_Text countText; //등장 수

    public void Setup(Sprite sprite, string enemyNameText, bool barrieer, bool regen, int count)
    {
        enemyImage.sprite = sprite;
        nameText.text = enemyNameText;
        barrierIcon.enabled = barrieer;
        regenIcon.enabled = regen;
        countText.text = count.ToString();
    }
}
