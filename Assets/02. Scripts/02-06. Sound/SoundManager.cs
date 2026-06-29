using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("BGM List")]
    [SerializeField] private BGMClipData[] _bgmClips;
    [Header("SFX List")]
    [SerializeField] private SFXClipData[] _sfxClips;

    private Dictionary<EBGMType, BGMClipData> _bgmDictionary;
    private Dictionary<ESFXType, SFXClipData> _sfxDictionary;

    private Dictionary<ESFXType, float> _sfxLastPlayTime;
    private float _sfxPlayCool = 0.05f;

    private BGMClipData _currentBGMData;
    private float _masterVolume = 1f;
    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;

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

        for (int i = 0; i < _bgmClips.Length; i++)
        {
            if (_bgmClips[i] == null) { continue; }

            if (_bgmClips[i].clip == null) { continue; }

            if (!_bgmDictionary.ContainsKey(_bgmClips[i].type))//같은 타입이 아직 없으면
            {
                _bgmDictionary.Add(_bgmClips[i].type, _bgmClips[i]);
            }
        }

        for (int i = 0; i < _sfxClips.Length; i++)
        {
            if (_sfxClips[i] == null) { continue; }

            if (_sfxClips[i].clip == null) { continue; }

            if (!_sfxDictionary.ContainsKey(_sfxClips[i].type))//같은 타입이 아직 없으면
            {
                _sfxDictionary.Add(_sfxClips[i].type, _sfxClips[i]);
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

    public void PasueBGM() //일시정지
    {
        _bgmSource.Pause();
    }
    public void ResumeBGM() //일시정지한거 다시 재생
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
