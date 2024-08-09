using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private void Awake()
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

    public void GoToScene(int _sceneInfo)
    {
        UIManager.instance.StartFade(_sceneInfo);

        UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[0].SetActive(false);
        UIManager.instance.SceneUI["Battle_1"].SetActive(false);

        switch (_sceneInfo)
        {
            case (int)SceneInfo.Start:
                UIManager.instance.SceneUI["Start"].SetActive(true);
                break;
            case (int)SceneInfo.Town_1:
                UIManager.instance.SceneUI["Town_1"].SetActive(true);
                break;
        }
        Player.instance.ReloadPlayer();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
