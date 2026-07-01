using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [Header("목숨 이미지")]
    [SerializeField]
    private Image[] lifeImage;

    [Header("PlayerHp 연결")]
    [SerializeField]
    private PlayerHp playerHp;

    private void OnEnable()
    {
        if  (PlayerHp.Instance != null)
        {
            PlayerHp.Instance.OnHpChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (PlayerHp.Instance != null)
        {
            PlayerHp.Instance.OnHpChanged -= UpdateUI;
        }
    }

    private void UpdateUI(int currentHp)
    {
        for (int i = 0; i < lifeImage.Length; i++)
        {
            if (i >= currentHp && lifeImage[i].gameObject.activeSelf)
            {
                PlayHeartLossAnimation(lifeImage[i]);
            }
        }
    }

    private void PlayHeartLossAnimation(Image heart)
    {
        RectTransform rect = heart.GetComponent<RectTransform>();

        LayoutElement layout = heart.GetComponent<LayoutElement>();
        if (layout == null) layout = heart.gameObject.AddComponent<LayoutElement>();
        layout.ignoreLayout = true; 

        rect.DOComplete();
        heart.DOComplete();

        rect.DOShakeAnchorPos(0.3f, new Vector2(15f, 0f), 20, 0);

        rect.DOAnchorPosY(rect.anchoredPosition.y - 50f, 0.4f).SetDelay(0.3f).SetEase(Ease.InQuad);

        heart.DOFade(0f, 0.4f).SetDelay(0.3f).OnComplete(() =>
        {
            heart.gameObject.SetActive(false);
            layout.ignoreLayout = false; 
        });
    }
}
