using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] float fadeTime;
    float start, end, time;
    Image fadePanel;

    void Start()
    {
        fadePanel = GetComponent<Image>();

        start = 1f;
        end = 0f;
        fadeTime = 1.5f;
    }

    public void ManageFade(bool _scene, string _loadSceneName) {
        // bool _scene 변수는 sceneLoad 이외에 fade기능을 사용할 일이 없다면 삭제
        StartCoroutine(FadeCoroutine(_scene, _loadSceneName));
    }

    IEnumerator FadeCoroutine(bool _scene, string _loadSceneName)
    {   
        Color fadeColor = fadePanel.color;

        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        if (_scene)
        {
            ScenesManager.instance.ChangeScene(_loadSceneName);
            yield return new WaitForSeconds(.5f);
        }
        
        time = 0f;
        while(fadeColor.a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor;

            yield return null;
        }
    }
}
