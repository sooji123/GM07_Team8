using UnityEngine;
using UnityEngine.UI;
//UI 구현을 위한 HP 임시 구현
public class PlayerHpUI : MonoBehaviour
{
    [Header("목숨 이미지")]
    [SerializeField]
    private Image[] lifeImage;

    [SerializeField]
    private int maxHp = 3;

    private int currentHp;
    public bool IsDead => currentHp <= 0;

    private void Start()
    {
        currentHp = maxHp;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for(int i = 0; i < lifeImage.Length; i++)
        {
            if(i < currentHp)
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
