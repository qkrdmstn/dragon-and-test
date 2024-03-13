using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public float fadeTime = 2f;
    float start, end, time;
    Image fadePanel;
    [SerializeField] bool isPlay = false;
    bool isPlaying = false;

    void Awake()
    {
        fadePanel = GetComponent<Image>();  
    }


    void Start()
    {
        start = 1f;
        end = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlay && !isPlaying)
        {
            isPlaying = true;
            StartCoroutine(FadeCoroutine());
        }
    }

    IEnumerator FadeCoroutine()
    {
        Color fadeColor = fadePanel.color;

        // 밝아집니다
        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        SceneManager.LoadScene("ChangeSceneTest");
        yield return new WaitForSeconds(1f);
        // 이 사이에 씬 이동 전환 기능 삽입 가능 -> collider의 tag로 이동되어야할 map을 구분지어 [씬 이동 전문 스크립트] 하나 생성 필요

        // 원하는 기능 수행완료시, 다시 어두워집니다
        time = 0f;
        while(fadeColor.a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor;

            yield return null;
        }

        isPlaying = false;
        isPlay = false;
        
    }
}
