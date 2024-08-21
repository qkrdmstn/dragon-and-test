using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SoundType
{
    Player,
    Monster,
    UI
}
public enum PlayerSfx
{
    Walk,
    Breath,
    Dash
}
public enum MonsterSfx
{
    Damage,
    birdAttack
}
public enum UISfx
{
    Snap
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    #region SoundClipDatas
    [Header("----- AudioClipData")]
    public AudioClip[] BGMClips;

    [System.Serializable]
    public class ClipType
    {
        public SoundType myType;
        public AudioClip[] effectClips;
    }

    public List<ClipType> effects;
    #endregion

    [Header("----- AudioState")]
    public float _bgmVolume;
    public float _effectVolume;
    public float fadeDuration;
    public float fadeTimer;

    [Header("----- AudioSources")]
    AudioSource BGMSource;
    AudioSource[] walkSources;
    AudioSource[] effectSources;
    public int channels;
    int channelIndex;
    int walkChannelIndex;

    public bool isFadeOut = false;

    //Singleton
    void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지

        InitAudio();
    }

    void InitAudio()
    {   // BGM
        BGMSource = transform.GetChild(0).gameObject.AddComponent<AudioSource>();
        BGMSource.playOnAwake = false;
        BGMSource.volume = _bgmVolume;
        BGMSource.loop = true;

        // Effect
        effectSources = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            effectSources[i] = transform.GetChild(1).gameObject.AddComponent<AudioSource>();
            effectSources[i].playOnAwake = false;
            effectSources[i].volume = _effectVolume;
        }

        walkSources = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            walkSources[i] = transform.GetChild(2).gameObject.AddComponent<AudioSource>();
            walkSources[i].playOnAwake = false;
            walkSources[i].volume = _effectVolume;
            walkSources[i].clip = effects[(int)SoundType.Player].effectClips[(int)PlayerSfx.Walk];
        }
    }
    // sceneNum과 BGMClips 배열의 순서동일
    // sceneNum : BGM clip
    // 0 : start
    // 1 : tutorial
    // 2 : town 아직 없어서 배틀이 2
    // 2 : battle

    public bool SetBGMClip(SceneInfo sceneNum)
    {
        int _sceneNum = (int)sceneNum;
        // BGM
        if (_sceneNum == 2)
            _sceneNum = 1;
        else if (_sceneNum >= 3)
            _sceneNum = 2;

        BGMSource.volume = _bgmVolume;
        BGMSource.clip = BGMClips[_sceneNum];
        return true;
    }

    public IEnumerator FadeOutSound()
    {
        if (BGMSource.isPlaying)
        {   // 음악이 서서히 작아집니다
            isFadeOut = true;
            fadeTimer = 0;
            while (fadeTimer <= 1)
            {
                fadeTimer += Time.deltaTime / fadeDuration;
                BGMSource.volume = Mathf.Lerp(_bgmVolume, 0, fadeTimer);
                yield return null;
            }
            BGMSource.Stop();
            isFadeOut = false;
        }
    }
    public IEnumerator FadeInSound(SceneInfo sceneInfo)
    {
        yield return new WaitUntil(() => !isFadeOut);

        yield return new WaitUntil(() => SetBGMClip(sceneInfo));
        BGMSource.Play();

        //while (fadeTimer <= 1)
        //{   // 음악이 서서히 커집니다
        //    fadeTimer += Time.deltaTime / fadeDuration; ;
        //    BGMSource.volume = Mathf.Lerp(0, _bgmVolume, fadeTimer);
        //    yield return null;
        //}
    }

    public void SetEffectSound<T>(SoundType _type, T clip) where T : Enum
    {
        int clipIdx = (int)Enum.Parse(clip.GetType(), clip.ToString());

        PlayEffectSound(effects[(int)_type].effectClips[clipIdx]);
    }

    public void PlayEffectSound(AudioClip _clip)
    {
        for (int audioIdx = 0; audioIdx < channels; audioIdx++)
        {
            int loopIdx = (audioIdx + channelIndex) % channels;
            if (effectSources[loopIdx].isPlaying) continue;

            channelIndex = loopIdx;
            effectSources[loopIdx].clip = _clip;
            effectSources[loopIdx].Play();
            break;
        }
    }

    public void PlayWalkEffect()
    {
        for (int audioIdx = 0; audioIdx < channels; audioIdx++)
        {
            int loopIdx = (audioIdx + walkChannelIndex) % channels;
            if (walkSources[loopIdx].isPlaying) break;

            walkChannelIndex = loopIdx;
            walkSources[loopIdx].Play();
            break;
        }
    }
}
