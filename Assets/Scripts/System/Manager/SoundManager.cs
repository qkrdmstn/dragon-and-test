using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SoundType
{
    Player, Monster, UI, Puzzle, Boss, NPC
}
public enum PlayerSfx
{
    Walk, Breath, Dash, Avoid
}

public enum MonsterSfx
{
    Damage, Spawn, Dead,            // 공통
    nearChase, nearAttack,          // 참새 1
    dashAttack,                     // 참새 2
    farChase, farAttack             // 매 1
}
public enum UISfx
{
    Snap, mopo, doorOpen, doorClose, Jokbo, Click,
    BuyShop, GetCoin, GetDragonFruit, GetHwatu, Dialogue, Select,

}

public enum BossSfx
{
    JanBasicAttack, JanFootStep, JanWingBomb, JanWingsCharge, JanWingsAttack
}

public enum PuzzleSfx
{
    LeverR, LeverL, Clear, Choose, Select, FailDamage
}

public enum NPCSfx
{   // InteractioData의 Sequence int로 사용
    FriendHi, FriendBye,    // 0 : 친구 1
    HunjangHi, HunjangBye,  // 1 : 훈장님 
    TwinAHi, TwinABye,      // 2 : 마을주민(집근처)
    TwinBHi, TwinBBye       // 3 : 마을주민(포탈)
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
        public float sfxVolume;
        public AudioClip[] effectClips;
    }

    public List<ClipType> effects;
    #endregion

    [Header("----- AudioState")]
    public float _bgmVolume;
    public float fadeDuration;
    public float fadeTimer;

    [Header("----- AudioSources")]
    AudioSource BGMSource;
    AudioSource[] walkSources;
    AudioSource[][] effectSources;

    public GameObject[] effectSlots;
    public int channels;
    int[] channelIndex;
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

        effectSources = new AudioSource[effectSlots.Length][];
        channelIndex = new int[effectSlots.Length];
        // Effect
        for (int i=0; i<effectSlots.Length; i++)
        {
            effectSources[i] = new AudioSource[channels];
            for (int j = 0; j < channels; j++)
            {
                effectSources[i][j] = effectSlots[i].AddComponent<AudioSource>();
                effectSources[i][j].playOnAwake = false;
                effectSources[i][j].volume = effects[i].sfxVolume;
            }
        }

        walkSources = new AudioSource[channels];
        for (int i = 0; i < channels; i++)
        {
            walkSources[i] = transform.GetChild(2).gameObject.AddComponent<AudioSource>();
            walkSources[i].playOnAwake = false;
            walkSources[i].volume = effects[(int)SoundType.Player].sfxVolume;
            walkSources[i].clip = effects[(int)SoundType.Player].effectClips[(int)PlayerSfx.Walk];
        }
    }
    // sceneNum과 BGMClips 배열의 순서동일
    // sceneNum : BGM clip
    // 0 : start
    // 1 : town, tutorial
    // 2 : battle, puzzle
    // 3 : boss

    public bool SetBGMClip(SceneInfo sceneNum)
    {
        int _sceneNum = (int)sceneNum;
        // BGM
        if (_sceneNum == 2) // tutorial
            _sceneNum = 1;
        else if (_sceneNum == 7) // boss
            _sceneNum = 3;
        else if (_sceneNum >= 3) // battle, puzzle
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

        PlayEffectSound((int)_type, effects[(int)_type].effectClips[clipIdx]);
    }

    public void PlayEffectSound(int effectSlotIdx, AudioClip _clip)
    {
        for (int audioIdx = 0; audioIdx < channels; audioIdx++)
        {
            int loopIdx = (audioIdx + channelIndex[effectSlotIdx]) % channels;
            if (effectSources[effectSlotIdx][loopIdx].isPlaying) continue;

            channelIndex[effectSlotIdx] = loopIdx;
            effectSources[effectSlotIdx][loopIdx].clip = _clip;

            effectSources[effectSlotIdx][loopIdx].Play();
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

    // for inspector setting
    public void PlayClickEffect()
    {
        SetEffectSound(SoundType.UI, UISfx.Click);
    }

    public void PlayUIEffeect(int uISfx)
    {   // click : 5, select : 11
        SetEffectSound(SoundType.UI, (UISfx)uISfx);
    }
}
