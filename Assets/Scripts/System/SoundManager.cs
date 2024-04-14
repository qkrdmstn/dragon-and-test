using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource[] audioSources; //0: BGM, 1: Effect
    public AudioClip[] BGMClips;
    public AudioClip[] effectClips;

    public float _bgmVolume = 0.6f;
    public float _effectVolume = 0.6f;
    public float fadeDuration;
    public float fadeTimer;

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
    }

    // sceneNum과 BGMClips 배열의 순서동일
    // sceneNum : BGM clip
    // 0 : start
    // 1 : tutorial
    // 2 : town
    // 3 : battle

    public void ManageSound(int _sceneNum)
    {
        if(_sceneNum > 1) 
            _sceneNum = 1;

        StartCoroutine(FadeSound(BGMClips[_sceneNum]));
    }


    IEnumerator FadeSound(AudioClip _clip)
    {
        yield return StartCoroutine(FadeOutSound());

        yield return new WaitUntil(() => ChangeBGM(_clip));

        StartCoroutine(FadeInSound());
    }

   public IEnumerator FadeOutSound()
    {
        if (audioSources[0].isPlaying)
        {   // 음악이 서서히 작아집니다
            fadeTimer = 0;
            while (fadeTimer <= 1)
            {
                fadeTimer += Time.deltaTime / fadeDuration;
                audioSources[0].volume = Mathf.Lerp(_bgmVolume, 0, fadeTimer);
                yield return null;
            }
            audioSources[0].mute = true;
        }
    }
    public IEnumerator FadeInSound()
    {
        audioSources[0].mute = false;
        fadeTimer = 0;

        while (fadeTimer <= 1)
        {   // 음악이 서서히 커집니다
            yield return null;
            fadeTimer += Time.deltaTime / fadeDuration; ;
            audioSources[0].volume = Mathf.Lerp(0, _bgmVolume, fadeTimer);
        }
    }

    public bool ChangeBGM(AudioClip _clip)
    {
        //audioSources[0].loop = true;
        //audioSources[0].volume = 0;
        audioSources[0].clip = _clip;
        audioSources[0].Play();

        return true;
    }

    public void SetEffectSound(string _clipName)
    {
        int index = 0;
        switch (_clipName)
        {
            case "Click":
                index = 0;
                break;
            case "Jump":
                index = 1;
                break;
            case "Hit":
                index = 2;
                break;
            case "ShootWire":
                index = 3;
                break;
            case "WireJump":
                index = 4;
                break;
            case "Lose":
                index = 5;
                break;
            default:
                Debug.LogWarning("OutOfRange in SoundClips");
                return;
        }
        PlayEffectSound(effectClips[index]);
    }

    public void PlayEffectSound(AudioClip _clip)
    {
        audioSources[1].clip = _clip;
        audioSources[1].loop = false;
        audioSources[1].volume = _effectVolume;
        audioSources[1].Play();
    }
}
