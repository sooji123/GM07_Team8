using UnityEngine;
using UnityEngine.UI;
//UI 구현을 위한 HP 임시 구현
public class PlayerHpUI : MonoBehaviour
{
    [Header("목숨 이미지")]
    [SerializeField]
    private Image[] lifeImage;

    [Header("PlayerHp 연결")]
    [SerializeField]
    private PlayerHp playerHp;

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        for(int i = 0; i < lifeImage.Length; i++)
        {
            if(i < playerHp.CurrentHp)
            {
                lifeImage[i].enabled = true;
            }
            else
            {
                lifeImage[i].enabled = false;
            }
        }
    }
}
