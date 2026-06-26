using System;
using UnityEngine;

[Serializable]
public class BGMClipData
{
    public EBGMType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}

[Serializable]
public class SFXClipData
{
    public ESFXType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}