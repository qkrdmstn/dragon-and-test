using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class Fade : MonoBehaviour
{
    [SerializeField] float startFadeTime;
    [SerializeField] float endFadeTime;
    float start, end, time;
    public Image fadePanel;
    [SerializeField] VideoPlayer loadingVideo;
    void Start()
    {
        start = 1f;
        end = 0f;
        startFadeTime = 2f;
        endFadeTime = 1f;
    }

    public void ManageFade(int _sceneNum)
    {
        if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Start && _sceneNum == 1)
        {
            TextMeshProUGUI[] texts = UIManager.instance.SceneUI["Start"].GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                StartCoroutine(TextFadeCoroutine(texts[i]));
            }
        }
        StartCoroutine(FadeCoroutine(_sceneNum));
    }

    public void ManageFade(MapControl mapControl, Spawner spawner, int curMapNum)
    {   // if Nothing in params, transition of Inner-Map
        StartCoroutine(FadeCoroutine(mapControl, spawner, curMapNum));
    }

    IEnumerator FadeCoroutine(int _sceneInfo)
    {   // Image
        
        Color fadeColor = fadePanel.color;

        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / startFadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        {
            loadingVideo.gameObject.SetActive(true);
            loadingVideo.Play();
            ScenesManager.instance.ChangeScene(_sceneInfo);

            if (ScenesManager.instance.sceneInfos[_sceneInfo].loadDBcnt > 0)
                yield return new WaitUntil(() => ScenesManager.instance.IsCompletedLoadData(_sceneInfo));

            yield return new WaitForSeconds(.5f);
            loadingVideo.Pause();
            loadingVideo.gameObject.SetActive(false);
        }

        time = 0f;
        while (fadeColor.a > 0f)
        {
            time += Time.deltaTime / endFadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor; 
            yield return null;
        }
        Player.instance.isStateChangeable = true;
        Time.timeScale = 1f;
        UIManager.instance.isEndFade = true;
        
        yield return null;
    }

    IEnumerator FadeCoroutine(MapControl mapControl, Spawner spawner, int curMapNum)
    {   // 맵 내부 이동에 관련된 코루틴
        Color fadeColor = fadePanel.color;

        time = 0f;
        while (fadeColor.a < 1f)
        {
            time += Time.deltaTime / startFadeTime;

            fadeColor.a = Mathf.Lerp(end, start, time); // 0 ~ 1
            fadePanel.color = fadeColor;

            yield return null;
        }

        {   
            Player.instance.transform.position = mapControl.gotoPos.position;
            Player.instance.cameraManager.UpdateConfineArea(mapControl.confineColl);
            
            yield return new WaitForSeconds(.5f);

            Player.instance.isStateChangeable = true;
            spawner.UpdateCurBlockNumber(curMapNum);
            mapControl.flag = false;
        }
        time = 0f;
        while (fadeColor.a > 0f)
        {
            time += Time.deltaTime / endFadeTime;

            fadeColor.a = Mathf.Lerp(start, end, time); // 1 ~ 0
            fadePanel.color = fadeColor;

            yield return null;
        }
        Time.timeScale = 1f;
        UIManager.instance.isEndFade = true;
        yield return null;
    }

    IEnumerator TextFadeCoroutine(TextMeshProUGUI text)
    {   // Text - 어두워집니다
        time = 0f;
        while (text.alpha > 0f)
        {
            time += Time.deltaTime / startFadeTime;

            text.alpha = Mathf.Lerp(start, end, time); // 1 ~ 0
            yield return null;
        }

        // - 밝아집니다
        time = 0f;
        while (text.alpha < 1f)
        {
            time += Time.deltaTime / endFadeTime;

            text.alpha = Mathf.Lerp(end, start, time); // 0 ~ 1
            yield return null;
        }
    }
}
