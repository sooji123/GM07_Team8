using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerHp : Singleton<PlayerHp>
{
    [SerializeField]
    private int maxHp = 5;
    [SerializeField]
    private GameObject _damageImg;

    private int currentHp;
    private Coroutine _damageEffectCo;
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    public event Action<int> OnHpChanged;

    protected override void Awake()
    {
        base.Awake();
        currentHp = maxHp;

        if(_damageImg != null)
        {
            _damageImg.SetActive(false);
        }
    }

    public void DecreasePlayerLife(int damage)
    {
        currentHp -= damage;
        OnHpChanged?.Invoke(currentHp);

        if (_damageImg != null) 
        {
            if (_damageEffectCo != null)
            {
                StopCoroutine(_damageEffectCo);
            }
            _damageEffectCo = StartCoroutine(DamageImage());
        }
        if (Camera.main != null)
        {
            Camera.main.transform.DOComplete();
            Camera.main.transform.DOShakePosition(0.15f, 0.2f, 20, 90, false, true);
        }

        if (currentHp <= 0)
        {
            Debug.Log("체력 0, 게임 종료됩니다.");
            UI_Manager.Instance.GameOverWindow();
        }
    }

    IEnumerator DamageImage()
    {
        _damageImg.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        _damageImg.SetActive(false);
        _damageEffectCo = null;
    }
}
