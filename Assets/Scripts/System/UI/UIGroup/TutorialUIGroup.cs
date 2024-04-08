using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIGroup : UIGroup
{
    float[] start, end;
    float fadeTime;

    public bool isWASD, isAttack, isDash, isReload; // UI가 뜨고 플레이어가 해당 UI에 대한 key를 누르면 활성화

    public Image[] imgs;
    Color[] colors;

    private void OnEnable()
    {
        fadeTime = 2.5f;
        InitUI(childUI[0]);
        StartCoroutine(ActiveUI(0));
    }

    private void Update()
    { 
        if(!isAttack && isWASD) StartCoroutine(ActiveUI(1));
        else if(!isReload && isAttack && isDash) StartCoroutine(ActiveUI(2));
    }

    public bool InitUI(GameObject _ui)
    {
        imgs = _ui.GetComponentsInChildren<Image>();
        colors = new Color[imgs.Length];

        start = new float[imgs.Length];
        end = new float[imgs.Length];

        for (int i = 0; i < imgs.Length; i++)
        {
            start[i] = 0f;
            end[i] = 1f;
            colors[i] = imgs[i].color;
        }

        return true;
    }

    IEnumerator ActiveUI(int i)
    {
        yield return new WaitForSeconds(1f);

        float time = 0f;
        while (colors[i].a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            colors[i].a = Mathf.Lerp(start[i], end[i], time);
            imgs[i].color = colors[i];

            yield return null;
        }

        yield return new WaitUntil(() => CheckState()); 

        time = 0f;
        while (colors[i].a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            colors[i].a = Mathf.Lerp(end[i], start[i], time);
            imgs[i].color = colors[i];

            yield return null;
        }

        if(i+1 < childUI.Length)
            yield return new WaitUntil(() => InitUI(childUI[i+1]));
    }

    public bool CheckState()
    {
        if (!isWASD && Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D))
        {
            isWASD = true;
            return isWASD;
        }

        if (!isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttack = true;
        }

        if (!isDash && Input.GetKeyDown(KeyCode.Mouse1))
        {
            isDash = true;
        }

        if (isAttack && isDash) return true;

        if (!isReload && Input.GetKeyDown(KeyCode.Q))
        {
            isReload = true;
            return isReload;
        }
        return false;
    } 
}
