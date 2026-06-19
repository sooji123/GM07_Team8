using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [Header("Effect List")]
    [SerializeField] private List<EffectData> _effectData;

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
            if(data.prefab == null)
            {
                continue;
            }
            if (!_effectNameDictionary.ContainsKey(data.effectType))
            {
                _effectNameDictionary.Add(data.effectType, nameof(data.prefab));
            }
        }
    }

    public void PlayEffect(EEffectType effectType, Vector3 position, Quaternion rotation, float duration = 1f)
    {
        if (_effectNameDictionary.TryGetValue(effectType, out string poolKey))
        {
            GameObject effectGo = PoolManager.Instance.GetGo(poolKey);
            if (effectGo != null)
            {
                effectGo.transform.position = position;
                effectGo.transform.rotation = rotation;

                StartCoroutine(PlayEffect(effectGo, duration));
            }
        }
        else
        {
            Debug.LogWarning($"EffectManager: {effectType}에 해당하는 이펙트 데이터가 등록되지 않았습니다.");
        }
    }

    private IEnumerator PlayEffect(GameObject effectGo, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (effectGo != null && effectGo.activeSelf)
        {
            if (effectGo.TryGetComponent<PoolAble>(out var poolAble))
            {
                poolAble.ReleaseObject();
            }
            else
            {
                effectGo.SetActive(false);
            }
        }
    }
}
