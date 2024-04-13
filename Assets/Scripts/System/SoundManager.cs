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

    public bool isChange = false;
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

    public void OnSceneLoaded(string sceneName)
    {
        if (sceneName == "Town_1" || sceneName == "Battle_1")
        {
            Debug.Log("asd");
            return;

        }

        for (int i = 0; i < BGMClips.Length; i++)
        {
            if (sceneName == BGMClips[i].name)
                StartCoroutine(FadeInOutSound(BGMClips[i]));
        }
    }


    IEnumerator FadeInOutSound(AudioClip _clip)
    {
        yield return null;

        if(audioSources[0].isPlaying)
        {
            fadeTimer = 0;
            while (fadeTimer <= 1)
            {
                yield return null;
                fadeTimer += Time.deltaTime / fadeDuration;
                audioSources[0].volume = Mathf.Lerp(_bgmVolume, 0, fadeTimer);

            }
        }

        isChange = false;
        ChangeBGM(_clip);
        yield return new WaitUntil(() => isChange);

        fadeTimer = 0;
        while (fadeTimer <= 1)
        {
            yield return null;
            fadeTimer += Time.deltaTime / fadeDuration; ;
            audioSources[0].volume = Mathf.Lerp(0, _bgmVolume, fadeTimer);

        }
    }


    public void ChangeBGM(AudioClip _clip)
    {
        audioSources[0].clip = _clip;
        audioSources[0].loop = true;
        audioSources[0].volume = 0;
        audioSources[0].Play();
        isChange = true;
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
