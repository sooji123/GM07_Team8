using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;
    [SerializeField]
    private GameSettingWindow _settingWindow;

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    protected override void Awake()
    {
        base.Awake();

        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;
    }
    public async UniTask FadeOutAsync()
    {
        _fadeCanvasGroup.blocksRaycasts = true;
    }

    public async UniTask FadeInAsync()
    {

    }
}
