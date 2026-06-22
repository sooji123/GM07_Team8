using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField]
    private CanvasGroup _fadeCanvasGroup;
    [SerializeField]
    private UI_TurretControlWindow _turretwindow;

    //설정창 추후에 추가 예정

    [Header("Setting")]
    [SerializeField]
    private float _fadeDuration;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);

            _fadeCanvasGroup.alpha = 0f;
            _fadeCanvasGroup.blocksRaycasts = false;
        }
    }
    public async UniTask FadeOutAsync()
    {
        _fadeCanvasGroup.blocksRaycasts = true;
        await _fadeCanvasGroup.DOFade(1f, _fadeDuration)
            .SetUpdate(true)
            .ToUniTask();
    }

    public async UniTask FadeInAsync()
    {
        await _fadeCanvasGroup.DOFade(0f, _fadeDuration)
            .SetUpdate(true)
            .ToUniTask();
        _fadeCanvasGroup.blocksRaycasts = false;
    }
}
