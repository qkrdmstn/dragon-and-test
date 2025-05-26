using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneUI : MonoBehaviour
{
    [Header("Data")]
    public Sprite[] curSceneImages;
    public int numOfImages;
    public int curIdx;

    [Header("UI")]
    public Image cutSceneUI;
    public Button nextBtn;
    public Button prevBtn;
    public Button skipBtn;
    
    // Start is called before the first frame update
    void Awake()
    {
        numOfImages = curSceneImages.Length;
        curIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextCutScene(bool isNxt)
    {
        if (isNxt)
            curIdx++;
        else
            curIdx--;
        UIUpdate();
    }

    public void SkipCutScene()
    {
        UIManager.instance.StartFade((int)SceneInfo.Town_1);
    }

    public void UIUpdate()
    {
        if(curIdx < numOfImages)
            cutSceneUI.sprite = curSceneImages[curIdx];
        if(curIdx == 0)
            prevBtn.gameObject.SetActive(false);
        else if(curIdx == numOfImages)
            UIManager.instance.StartFade((int)SceneInfo.Town_1);

        else
        {
            prevBtn.gameObject.SetActive(true);
            nextBtn.gameObject.SetActive(true);
        }
    }
    IEnumerator IsLoadedStartData()
    {
        Debug.Log("!!!1");
        yield return new WaitUntil(() => ScenesManager.instance.IsCompletedLoadData(0));
        Debug.Log("!!!");
        UIManager.instance.StartFade((int)SceneInfo.Town_1);
    }
}
