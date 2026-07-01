using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectManager : Singleton<EffectManager>
{
    [Header("Effect List")]
    [SerializeField]
    private List<EffectData> _effectData;

    private Dictionary<EEffectType, string> _effectNameDictionary = new Dictionary<EEffectType, string>();

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    private void Initialize()
    {
        if(_effectData == null )
        {
            return;
        }

        foreach (EffectData data in _effectData)
        {
            if(data.poolData == null)
            {
                continue;
            }

            PoolManager.Instance.CreatePool(data.poolData);

            if (!_effectNameDictionary.ContainsKey(data.effectType))
            {
                _effectNameDictionary.Add(data.effectType, data.poolData.objName);
            }
        }
    }

    public void PlayEffect(EEffectType effectType, Vector3 position, Quaternion rotation, float duration = 1f)
    {
        if (_effectNameDictionary.TryGetValue(effectType, out string poolKey))
        {
            GameObject effect = PoolManager.Instance.GetGo(poolKey);
            if (effect != null)
            {
                effect.transform.position = position;
                effect.transform.rotation = rotation;
                StartCoroutine(PlayEffectCoroutine(effect, duration));
            }
        }
        else
        {
            Debug.LogWarning($"EffectManager: {effectType}에 해당하는 이펙트 데이터가 등록되지 않았습니다.");
        }
    }

    private IEnumerator PlayEffectCoroutine(GameObject effect, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (effect != null && effect.activeSelf)
        {
            if (effect.TryGetComponent<PoolAble>(out PoolAble poolAble))
            {
                poolAble.ReleaseObject();
            }
            else
            {
                effect.SetActive(false);
            }
        }
    }
}
