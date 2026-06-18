using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [Header("Effect List")]
    [SerializeField] private EffectData _effectData;

    private Dictionary<EEffectType, GameObject> _effectDictionary;

    private void Initialize()
    {

    }

    
}
