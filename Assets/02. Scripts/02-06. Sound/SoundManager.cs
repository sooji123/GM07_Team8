using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Sound Data")]
    [SerializeField] private SoundData _soundData;

    private Dictionary<EBGMType, BGMClipData> _bgmDictionary;
    private Dictionary<ESFXType, SFXClipData> _sfxDictionary;

    private Dictionary<ESFXType, float> _sfxLastPlayTime;
    private float _sfxPlayCool = 0.05f;

    private BGMClipData _currentBGMData;
    private float _masterVolume = 1f;
    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;

    public float GetBGMVolume() => _bgmVolume;
    public float GetSFXVolume() => _sfxVolume;

    protected override void Awake()
    {
        base.Awake();
        CreateAudioSources();
        InitializeDictionary();
    }
    private void CreateAudioSources()
    {
        if (_bgmSource == null)
        {
            GameObject bgmObj = new GameObject("BGM source");
            bgmObj.transform.SetParent(transform);

            _bgmSource = bgmObj.AddComponent<AudioSource>();

            _bgmSource.loop = true;
        }

        if (_sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX source");
            sfxObj.transform.SetParent(transform);

            _sfxSource = sfxObj.AddComponent<AudioSource>();

            _sfxSource.loop = false;
        }
    }
    private void InitializeDictionary()
    {
        _bgmDictionary = new Dictionary<EBGMType, BGMClipData>();
        _sfxDictionary = new Dictionary<ESFXType, SFXClipData>();
        _sfxLastPlayTime = new Dictionary<ESFXType, float>();

        if(_soundData == null) 
        {
            return; 
        }

        for (int i = 0; i < _soundData.bgmClips.Length; i++)
        {
            if (_soundData.bgmClips[i] == null) { continue; }
            if (_soundData.bgmClips[i].clip == null) { continue; }

            if (!_bgmDictionary.ContainsKey(_soundData.bgmClips[i].type))
            {
                _bgmDictionary.Add(_soundData.bgmClips[i].type, _soundData.bgmClips[i]);
            }
        }

        for (int i = 0; i < _soundData.sfxClips.Length; i++)
        {
            if (_soundData.sfxClips[i] == null) { continue; }
            if (_soundData.sfxClips[i].clip == null) { continue; }

            if (!_sfxDictionary.ContainsKey(_soundData.sfxClips[i].type))
            {
                _sfxDictionary.Add(_soundData.sfxClips[i].type, _soundData.sfxClips[i]);
            }
        }
    }

    #region BGM
    public void PlayeBGM(EBGMType type)
    {
        if (!_bgmDictionary.ContainsKey(type)) { return; }

        BGMClipData data = _bgmDictionary[type];

        if (_bgmSource.clip == data.clip) { return; }

        _currentBGMData = data;
        _bgmSource.clip = data.clip;
        _bgmSource.volume = data.volume * _bgmVolume * _masterVolume;

        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
        _currentBGMData = null;
    }

    public void PasueBGM()
    {
        _bgmSource.Pause();
    }
    public void ResumeBGM()
    {
        _bgmSource.UnPause();
    }
    #endregion

    #region SFX
    public void PlayeSFX(ESFXType type)
    {
        if (!_sfxDictionary.ContainsKey(type)) { return; }

        if (_sfxLastPlayTime.ContainsKey(type))
        {
            if(Time.time < _sfxLastPlayTime[type] + _sfxPlayCool)
            {
                return;
            }

            _sfxLastPlayTime[type] = Time.time;
        }
        else
        {
            _sfxLastPlayTime.Add(type, Time.time);
        }

        SFXClipData data = _sfxDictionary[type];

        float volume = data.volume * _sfxVolume * _masterVolume;
        _sfxSource.PlayOneShot(data.clip, volume);
    }
    #endregion

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);

    }

    private void UpdateBGMVolume()
    {
        if (_bgmSource == null) return;
        if (_currentBGMData == null)
        {
            _bgmSource.volume = _bgmVolume * _masterVolume;
            return;
        }

        _bgmSource.volume = _currentBGMData.volume * _bgmVolume * _masterVolume;
    }
}
