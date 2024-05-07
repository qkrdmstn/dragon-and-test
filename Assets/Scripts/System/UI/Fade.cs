using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    [SerializeField] float fadeTime;
    float start, end, time;
    public Image fadePanel;

    void Start()
    {
        fadePanel = GetComponent<Image>();

        start = 1f;
        end = 0f;
        fadeTime = 2f;
    }

    public void ManageFade(int _sceneNum)
    {
        SceneInfo _sceneInfo = (SceneInfo)_sceneNum;
        if (((int)_sceneInfo) == 1)
        {
            TextMeshProUGUI[] texts = UIManager.instance.SceneUI["Start"].GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                StartCoroutine(TextFadeCoroutine(texts[i]));
            }
        }
        StartCoroutine(FadeCoroutine(_sceneInfo));
    }

    public void ManageFade(MapControl mapControl)
    {   // if Nothing in params, transition of Inner-Map
        StartCoroutine(FadeCoroutine(mapControl));
    }

    IEnumerator FadeCoroutine(SceneInfo _sceneInfo)
    {   // Image
        Color fadeColor = fadePanel.color;

        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        {
            ScenesManager.instance.ChangeScene(_sceneInfo);
        }
        yield return new WaitForSeconds(.5f);

        time = 0f;
        while (fadeColor.a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor;

            yield return null;
        }
        Time.timeScale = 1f;

        yield return null;
    }

    IEnumerator FadeCoroutine(MapControl mapControl)
    {   // 맵 내부 이동에 관련된 코루틴
        Color fadeColor = fadePanel.color;

        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        {   
            GameManager.instance.player.transform.position = mapControl.gotoPos.position;
            GameManager.instance.player.cameraManager.UpdateConfineArea(mapControl.confineColl);
            yield return new WaitForSeconds(.5f);
            GameManager.instance.player.isStateChangeable = true;
        }

        time = 0f;
        while (fadeColor.a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor;

            yield return null;
        }
        Time.timeScale = 1f;

        yield return null;
    }

    IEnumerator TextFadeCoroutine(TextMeshProUGUI text)
    {   // Text - 어두워집니다
        time = 0f;
        while (text.alpha > 0f)
        {
            time += Time.deltaTime / fadeTime;

            text.alpha = Mathf.Lerp(start, end, time); // 1 ~ 0
            //fadePanel.color = fadeColor;

            yield return null;
        }
        // - 밝아집니다
        time = 0f;
        while (text.alpha < 1f)
        {
            time += Time.deltaTime / fadeTime;

            text.alpha = Mathf.Lerp(end, start, time); // 0 ~ 1
            //fadePanel.color = fadeColor;

            yield return null;
        }
    }
}
