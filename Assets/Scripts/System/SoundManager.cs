using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;     // 곡의 이름
    public AudioClip clip;  // 곡 파일
}

public class SoundManager : MonoBehaviour
{
    #region Singleton
    static public SoundManager instnace;
    void Awake()
    {
        if (instnace == null)
        {
            instnace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        playSoundName = new string[audioSourceBGM.Length + 1];  // 내가v등록한 오디오파일의 수와 맞게 초기화
    }
    #endregion Singleton
    // 실제 재생되고있는 음악 목록
    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;

    public string[] playSoundName;

    // 재생되고자 하는 음악 목록
    public Sound[] effectSounds;
    public Sound[] bgmSound;

    public void PlaySE(string _name, float volume)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name.Equals(effectSounds[i].name))
            {   // 일단 틀고자하는 파일이 존재하는가
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {   // 재생중이지 않은 사운드에 대해서
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].volume = volume;
                        audioSourceEffects[j].Play();
                        return;
                    }   // 재생을 시켜주고 함수 종료
                }
                return;
            }
        }
        Debug.Log(_name + " 사운드가 SoundManager에 등록되지 않았습니다");
    }

    public void StopAllSound()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i].Equals(_name))
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
    }
    int i = -1;
    public void PlayBGM(int sceneNum)
    {   
        i = sceneNum;
        //Todo. 바꾸기
        if (ScenesManager.instance.GetSceneNum() != 2 && !audioSourceBGM[i].isPlaying)
        {   // 재생중이지 않은 사운드에 대해서
            Debug.Log(bgmSound[i].name);
            playSoundName[audioSourceBGM.Length] = bgmSound[i].name;
            audioSourceBGM[i].clip = bgmSound[i].clip;
            //audioSourceBGM[i].PlayOneShot(bgmSound[i].clip, 0.1f);
            audioSourceBGM[i].Play();
            audioSourceBGM[i].volume = 0.1f;
            audioSourceBGM[i].loop = true;
            return;
        }   // 재생을 시켜주고 함수 종료

    }
    public float animTime = 5.5f;         // Fade 애니메이션 재생 시간 (단위:초). 
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.
    private bool isStop = false;

    private void Update()
    {
        if (isStop && i != -1)
        {
            time += Time.deltaTime / animTime;
            audioSourceBGM[i].volume = Mathf.Lerp(audioSourceBGM[i].volume, 0, time);
        }
        else if (time != 0)
        {
            time = 0;
        }

        //if (i != -1 && audioSourceBGM[i].loop && !audioSourceBGM[i].isPlaying)
        //{   
        //    PlayBGM(i);
        //}
    }
    public void VolumeOutBGM()
    {
        isStop = true;
        StartCoroutine(StopBGM());
    }

    IEnumerator StopBGM()
    {
        yield return new WaitForSeconds(animTime);
        audioSourceBGM[i].Stop();
        isStop = false;
    }

}